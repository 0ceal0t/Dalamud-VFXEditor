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

namespace VfxEditor.Utils {
    public static class GltfUtils {
        public static void ExportModel( AvfxModel model, string path ) {
            var mesh = new MeshBuilder<VertexPositionNormalTangent, VertexColor1Texture2>( "mesh" );
            var material = new MaterialBuilder( "material" );

            var Verts = new GLTFVert[model.Vertexes.Vertexes.Count];
            var idx = 0;
            foreach( var v in model.Vertexes.Vertexes ) {
                Verts[idx] = GetVert( v );
                idx++;
            }

            foreach( var tri in model.Indexes.Indexes ) {
                var v1 = Verts[tri.I1];
                var v2 = Verts[tri.I2];
                var v3 = Verts[tri.I3];
                mesh.UsePrimitive( material ).AddTriangle( (v1.Pos, v1.Tex), (v2.Pos, v2.Tex), (v3.Pos, v3.Tex) );
            }

            var scene = new SceneBuilder();
            scene.AddRigidMesh( mesh, Matrix4x4.Identity );
            scene.ToGltf2().SaveGLTF( path );
            PluginLog.Log( "Saved GLTF to: " + path );
        }

        private static GLTFVert GetVert( AvfxVertex vert ) {
            var Pos = new Vector3( vert.Position[0], vert.Position[1], vert.Position[2] );
            var Normal = Vector3.Normalize( new Vector3( vert.Normal[0], vert.Normal[1], vert.Normal[2] ) );
            var Tangent = Vector4.Normalize( new Vector4( vert.Tangent[0], vert.Tangent[1], vert.Tangent[2], 1 ) );
            var _Pos = new VertexPositionNormalTangent( Pos, Normal, Tangent );

            var Color = new Vector4( vert.Color[0], vert.Color[1], vert.Color[2], vert.Color[3] ); // 255
            var UV1 = new Vector2( vert.UV1[0], vert.UV1[1] ); // this gets replicated -> 1: uv1.x uv1.y uv1.x uv1.y    2: uv1.x uv1.y uv2.x uv2.y
            var UV2 = new Vector2( vert.UV2[2], vert.UV2[3] );
            var _Tex = new VertexColor1Texture2( Color, UV1, UV2 );

            var ret = new GLTFVert {
                Pos = _Pos,
                Tex = _Tex
            };
            return ret;
        }

        private static AvfxVertex GetAvfxVert( Vector3 pos, Vector3 normal, Vector4 tangent, Vector4 color, Vector2 tex1, Vector2 tex2 ) {
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

            ret.UV1 = new float[] { tex1.X, tex1.Y, tex1.X, tex1.Y };
            ret.UV2 = new float[] { tex1.X, tex1.Y, tex2.X, tex2.Y };

            return ret;
        }

        public static bool ImportModel( string localPath, out List<AvfxVertex> vertexesOut, out List<AvfxIndex> indexesOut ) {
            vertexesOut = new List<AvfxVertex>();
            indexesOut = new List<AvfxIndex>();
            var model = SharpGLTF.Schema2.ModelRoot.Load( localPath );
            PluginLog.Log( "Importing GLTF from: " + localPath );

            if( model.LogicalMeshes.Count > 0 ) {
                var mesh = model.LogicalMeshes[0];
                if( mesh.Primitives.Count > 0 ) {
                    var primitive = mesh.Primitives[0];

                    var properties = primitive.VertexAccessors;
                    var hasColor = properties.ContainsKey( "COLOR_0" );
                    var hasUv2 = properties.ContainsKey( "TEXCOORD_1" );
                    PluginLog.Log( $"Color:{hasColor} UV2:{hasUv2}" );

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

                        vertexesOut.Add( GetAvfxVert( positions[i], normals[i], tangents[i], color, tex1s[i], uv2 ) );
                    }
                    foreach( var (A, B, C) in triangles ) {
                        indexesOut.Add( new AvfxIndex( A, B, C ) );
                    }
                    return true;
                }
            }
            return false;
        }

        private struct GLTFVert {
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
