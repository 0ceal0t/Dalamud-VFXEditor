using Dalamud.Logging;
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
            var mesh = new MeshBuilder<VertexPositionNormalTangent, VertexColor1Texture2>( "mesh" );
            var material = new MaterialBuilder( "material" );

            var vertexes = new GltfVertex[model.Vertexes.Vertexes.Count];
            var idx = 0;
            foreach( var v in model.Vertexes.Vertexes ) {
                vertexes[idx] = GetVertex( v );
                idx++;
            }

            foreach( var tri in model.Indexes.Indexes ) {
                var v1 = vertexes[tri.I1];
                var v2 = vertexes[tri.I2];
                var v3 = vertexes[tri.I3];

                mesh.UsePrimitive( material ).AddTriangle(
                    (v1.Pos, v1.Tex),
                    (v2.Pos, v2.Tex),
                    (v3.Pos, v3.Tex)
                );
            }

            var scene = new SceneBuilder();
            scene.AddRigidMesh( mesh, Matrix4x4.Identity );
            scene.ToGltf2().SaveGLTF( path );
            PluginLog.Log( $"Saved GLTF to: {path}" );
        }

        private static GltfVertex GetVertex( AvfxVertex vertex ) {
            var pos = new Vector3( vertex.Position[0], vertex.Position[1], vertex.Position[2] );
            var normal = Vector3.Normalize( new Vector3( vertex.Normal[0], vertex.Normal[1], vertex.Normal[2] ) );
            var tangent = Vector4.Normalize( new Vector4( vertex.Tangent[0], vertex.Tangent[1], vertex.Tangent[2], 1 ) );
            var combinedPos = new VertexPositionNormalTangent( pos, normal, tangent );

            var color = new Vector4( vertex.Color[0] / 255f, vertex.Color[1] / 255f, vertex.Color[2] / 255f, vertex.Color[3] / 255f ); // 255
            var uv1 = new Vector2( vertex.Uv1[0], vertex.Uv1[1] ); // this gets replicated -> 1: uv1.x uv1.y uv1.x uv1.y    2: uv1.x uv1.y uv2.x uv2.y
            var uv2 = new Vector2( vertex.Uv2[2], vertex.Uv2[3] );
            var combinedTexture = new VertexColor1Texture2( color, uv1, uv2 );

            var ret = new GltfVertex {
                Pos = combinedPos,
                Tex = combinedTexture
            };
            return ret;
        }

        private static AvfxVertex GetAvfxVertex( Vector3 pos, Vector3 normal, Vector4 tangent, Vector4 color, Vector2 uv1, Vector2 uv2 ) {
            var ret = new AvfxVertex();
            color *= 255;
            normal *= 128;
            tangent *= 128;
            ret.Position = new float[] { pos.X, pos.Y, pos.Z, 1 };

            var normalAdjusted = Vector3.Normalize( new Vector3( normal.X, normal.Y, normal.Z ) ) * 127f;
            var tangentAdjusted = Vector3.Normalize( new Vector3( tangent.X, tangent.Y, tangent.Z ) ) * 127f;

            ret.Normal = new int[] { ( int )normalAdjusted.X, ( int )normalAdjusted.Y, ( int )normalAdjusted.Z, -1 };
            ret.Tangent = new int[] { ( int )tangentAdjusted.X, ( int )tangentAdjusted.Y, ( int )tangentAdjusted.Z, -1 };
            ret.Color = new int[] { ( int )color.X, ( int )color.Y, ( int )color.Z, ( int )color.W };

            ret.Uv1 = new float[] { uv1.X, uv1.Y, uv1.X, uv1.Y };
            ret.Uv2 = new float[] { uv1.X, uv1.Y, uv2.X, uv2.Y };

            return ret;
        }

        public static bool ImportModel( string localPath, out List<AvfxVertex> vertexesOut, out List<AvfxIndex> indexesOut ) {
            vertexesOut = new();
            indexesOut = new();
            var model = SharpGLTF.Schema2.ModelRoot.Load( localPath );
            PluginLog.Log( "Importing GLTF model from: " + localPath );

            var count = 0;

            foreach( var mesh in model.LogicalMeshes ) {
                foreach( var primitive in mesh.Primitives ) {
                    var properties = primitive.VertexAccessors;
                    var hasColor = properties.ContainsKey( "COLOR_0" );
                    var hasUv2 = properties.ContainsKey( "TEXCOORD_1" );
                    PluginLog.Log( $"Color: {hasColor} UV2: {hasUv2}" );

                    var positions = primitive.GetVertices( "POSITION" ).AsVector3Array();
                    var normals = primitive.GetVertices( "NORMAL" ).AsVector3Array();
                    var tangents = primitive.GetVertices( "TANGENT" ).AsVector4Array();
                    var colors = hasColor ? primitive.GetVertices( "COLOR_0" ).AsVector4Array() : new Vector4Array();
                    var tex1s = primitive.GetVertices( "TEXCOORD_0" ).AsVector2Array();
                    var tex2s = hasUv2 ? primitive.GetVertices( "TEXCOORD_1" ).AsVector2Array() : new Vector2Array();

                    var triangles = primitive.GetTriangleIndices();

                    for( var i = 0; i < positions.Count; i++ ) {
                        var color = hasColor ? colors[i] : new Vector4( 1f, 1f, 1f, 1f ); // default white
                        var uv2 = hasUv2 ? tex2s[i] : tex1s[i]; // default uv1;

                        vertexesOut.Add( GetAvfxVertex( positions[i], normals[i], tangents[i], color, tex1s[i], uv2 ) );
                    }
                    foreach( var (i1, i2, i3) in triangles ) {
                        indexesOut.Add( new AvfxIndex( count + i1, count + i2, count + i3 ) );
                    }

                    count += positions.Count;
                }
            }

            return count > 0;
        }

        private struct GltfVertex {
            public VertexPositionNormalTangent Pos;
            public VertexColor1Texture2 Tex;
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
