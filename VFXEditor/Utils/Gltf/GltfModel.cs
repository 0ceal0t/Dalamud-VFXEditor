using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Memory;
using SharpGLTF.Scenes;
using System;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.AvfxFormat;

namespace VfxEditor.Utils.Gltf {
    public static class GltfModel {
        public static void ExportModel( AvfxModel model, string path ) {
            var mesh = new MeshBuilder<VertexPositionNormalTangent, VertexColor1Texture4>( "mesh" );
            var material = new MaterialBuilder( "material" );

            var vertexes = new List<(VertexPositionNormalTangent, VertexColor1Texture4)>();
            foreach( var v in model.Vertexes.Vertexes ) vertexes.Add( GetVertex( v ) );

            foreach( var tri in model.Indexes.Indexes ) {
                var v1 = vertexes[tri.I1];
                var v2 = vertexes[tri.I2];
                var v3 = vertexes[tri.I3];
                mesh.UsePrimitive( material ).AddTriangle( v1, v2, v3 );
            }

            var scene = new SceneBuilder();
            scene.AddRigidMesh( mesh, Matrix4x4.Identity );
            scene.ToGltf2().SaveGLTF( path );
            Dalamud.Log( $"Saved GLTF to: {path}" );
        }

        private static (VertexPositionNormalTangent, VertexColor1Texture4) GetVertex( AvfxVertex vertex ) {
            var pos = new Vector3( vertex.Position[0], vertex.Position[1], vertex.Position[2] );
            var normal = Vector3.Normalize( new Vector3( vertex.Normal[0], vertex.Normal[1], vertex.Normal[2] ) );
            var tangent = Vector4.Normalize( new Vector4( vertex.Tangent[0], vertex.Tangent[1], vertex.Tangent[2], 1 ) );
            var combinedPos = new VertexPositionNormalTangent( pos, normal, tangent );

            var color = new Vector4( vertex.Color[0] / 255f, vertex.Color[1] / 255f, vertex.Color[2] / 255f, vertex.Color[3] / 255f ); // 255
            var combinedTexture = new VertexColor1Texture4( color, vertex.Uv1, vertex.Uv2, vertex.Uv3, vertex.Uv4 );

            return (combinedPos, combinedTexture);
        }

        private static AvfxVertex GetAvfxVertex( Vector3 pos, Vector3 normal, Vector4 tangent, Vector4 color, Vector2 uv1, Vector2 uv2, Vector2 uv3, Vector2 uv4 ) {
            var ret = new AvfxVertex();
            color *= 255;
            normal *= 128;
            tangent *= 128;
            ret.Position = new( pos.X, pos.Y, pos.Z, 1 );

            var normalAdjusted = Vector3.Normalize( new Vector3( normal.X, normal.Y, normal.Z ) ) * 127f;
            var tangentAdjusted = Vector3.Normalize( new Vector3( tangent.X, tangent.Y, tangent.Z ) ) * 127f;

            ret.Normal = [( int )normalAdjusted.X, ( int )normalAdjusted.Y, ( int )normalAdjusted.Z, -1];
            ret.Tangent = [( int )tangentAdjusted.X, ( int )tangentAdjusted.Y, ( int )tangentAdjusted.Z, -1];
            ret.Color = [( int )color.X, ( int )color.Y, ( int )color.Z, ( int )color.W];

            ret.Uv1 = uv1;
            ret.Uv2 = uv2;
            ret.Uv3 = uv3;
            ret.Uv4 = uv4;

            return ret;
        }

        private static AvfxVertexNumber GetAvfxNumber( int i )
        {
            var ret = new AvfxVertexNumber();
            ret.Order = new( "##Order", i );
            return ret;
        }

        private static AvfxEmitVertex GetAvfxEmitVertex( Vector3 pos, Vector3 normal, Vector4 color )
        {
            var ret = new AvfxEmitVertex();
            ret.Position = new( "##Position", pos );

            var normalAdjusted = Vector3.Normalize( new Vector3( normal.X, normal.Y, normal.Z ) );

            ret.Normal = new( "##Normal", normalAdjusted );
            ret.Color = new( "##Color", color );
            return ret;
        }

        public static bool ImportEmitterModel( string localPath, out List<AvfxEmitVertex> vertexesOut , out List<AvfxVertexNumber> numbersOut )
        {
            numbersOut = [];
            vertexesOut = [];
            var model = SharpGLTF.Schema2.ModelRoot.Load( localPath );
            Dalamud.Log( "Importing GLTF model emitters from: " + localPath );

            var count = 0;

            foreach( var mesh in model.LogicalMeshes )
            {
                foreach( var primitive in mesh.Primitives )
                {
                    var properties = primitive.VertexAccessors;
                    var hasNormal = properties.ContainsKey( "NORMAL" ); // loose verts don't have normals
                    var hasColor = properties.ContainsKey( "COLOR_0" );
                    var positions = primitive.VertexAccessors["POSITION"].AsVector3Array();
                    var normals = hasNormal ? primitive.VertexAccessors["NORMAL"].AsVector3Array() : new Vector3Array();
                    var colors = hasColor ? primitive.VertexAccessors["COLOR_0"].AsVector4Array() : new Vector4Array();
                    for( var i = 0; i < positions.Count; i++ )
                    {
                        var normal = hasNormal ? normals[i] : new Vector3( 0, 1, 0 ); // default up
                        var color = hasColor ? colors[i] : new Vector4( 1f, 1f, 1f, 1f ); // default white
                        vertexesOut.Add( GetAvfxEmitVertex( positions[i], normal, color ) );
                        numbersOut.Add( GetAvfxNumber( i ) );
                    }

                    count += positions.Count;
                }
            }

            return count > 0;
        }

        public static bool ImportModel( string localPath, out List<AvfxVertex> vertexesOut, out List<AvfxIndex> indexesOut ) {
            vertexesOut = [];
            indexesOut = [];
            var model = SharpGLTF.Schema2.ModelRoot.Load( localPath );
            Dalamud.Log( "Importing GLTF model from: " + localPath );

            var count = 0;

            foreach( var mesh in model.LogicalMeshes ) {
                foreach( var primitive in mesh.Primitives ) {
                    var properties = primitive.VertexAccessors;
                    var hasColor = properties.ContainsKey( "COLOR_0" );

                    var hasUv2 = properties.ContainsKey( "TEXCOORD_1" );
                    var hasUv3 = properties.ContainsKey( "TEXCOORD_2" );
                    var hasUv4 = properties.ContainsKey( "TEXCOORD_3" );

                    Dalamud.Log( $"Color:{hasColor} Uv2:{hasUv2} Uv3:{hasUv3} Uv4:{hasUv4}" );

                    if( !properties.ContainsKey( "TANGENT" ) ) Dalamud.Error( "Tangents are missing" );

                    var positions = primitive.VertexAccessors["POSITION"].AsVector3Array();
                    var normals = primitive.VertexAccessors["NORMAL"].AsVector3Array();
                    var tangents = primitive.VertexAccessors["TANGENT"].AsVector4Array();
                    var colors = hasColor ? primitive.VertexAccessors["COLOR_0"].AsVector4Array() : new Vector4Array();
                    var uv1s = primitive.VertexAccessors["TEXCOORD_0"].AsVector2Array();
                    var uv2s = hasUv2 ? primitive.VertexAccessors["TEXCOORD_1"].AsVector2Array() : new Vector2Array();
                    var uv3s = hasUv3 ? primitive.VertexAccessors["TEXCOORD_2"].AsVector2Array() : new Vector2Array();
                    var uv4s = hasUv4 ? primitive.VertexAccessors["TEXCOORD_3"].AsVector2Array() : new Vector2Array();

                    var triangles = primitive.GetTriangleIndices();

                    for( var i = 0; i < positions.Count; i++ ) {
                        var color = hasColor ? colors[i] : new Vector4( 1f, 1f, 1f, 1f ); // default white
                        var uv2 = hasUv2 ? uv2s[i] : uv1s[i];
                        var uv3 = hasUv3 ? uv3s[i] : uv1s[i];
                        var uv4 = hasUv4 ? uv4s[i] : uv1s[i];

                        vertexesOut.Add( GetAvfxVertex( positions[i], normals[i], tangents[i], color, uv1s[i], uv2, uv3, uv4 ) );
                    }
                    foreach( var (i1, i2, i3) in triangles ) {
                        indexesOut.Add( new AvfxIndex( count + i1, count + i2, count + i3 ) );
                    }

                    count += positions.Count;
                }
            }

            return count > 0;
        }

        public static int ColorToInt( Vector4 color ) {
            var data = new byte[] { ( byte )color.X, ( byte )color.Y, ( byte )color.Z, ( byte )color.W };
            return BitConverter.ToInt32( data );
        }

        public static Vector4 IntToColor( int color ) {
            var colors = BitConverter.GetBytes( color );
            return new Vector4( colors[0], colors[1], colors[2], colors[3] );
        }
    }
}
