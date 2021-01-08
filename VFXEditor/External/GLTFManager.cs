using AVFXLib.Models;
using Dalamud.Plugin;
using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor
{
    public class GLTFManager {
        public static void ExportModel(AVFXModel model, string path) {
            var mesh = new MeshBuilder<VertexPositionNormalTangent, VertexColor1Texture2>( "mesh" );
            var material = new MaterialBuilder( "material" );

            GLTFVert[] Verts = new GLTFVert[model.Vertices.Count];
            int idx = 0;
            foreach(var v in model.Vertices ) {
                Verts[idx] = GetVert( v );
                idx++;
            }

            foreach(var tri in model.Indexes ) {
                var v1 = Verts[tri.I1];
                var v2 = Verts[tri.I2];
                var v3 = Verts[tri.I3];
                mesh.UsePrimitive( material ).AddTriangle((v1.Pos, v1.Tex), (v2.Pos, v2.Tex), (v3.Pos, v3.Tex));
            }

            var scene = new SceneBuilder();
            scene.AddRigidMesh( mesh, Matrix4x4.Identity );
            scene.ToGltf2().SaveGLTF(path);
            PluginLog.Log( "Saved GLTF to: " + path );
        }
        public static GLTFVert GetVert(Vertex vert ) {
            Vector3 Pos = new Vector3( vert.Position[0], vert.Position[1], vert.Position[2] );
            Vector3 Normal = Vector3.Normalize(new Vector3( vert.Normal[0], vert.Normal[1], vert.Normal[2] ));
            Vector4 Tangent = Vector4.Normalize(new Vector4( vert.Tangent[0], vert.Tangent[1], vert.Tangent[2], 1 ));
            VertexPositionNormalTangent _Pos = new VertexPositionNormalTangent(Pos, Normal, Tangent);

            Vector4 Color = new Vector4(vert.Color[0], vert.Color[1], vert.Color[2], vert.Color[3]); // 255
            Vector2 UV1 = new Vector2(vert.UV1[0], vert.UV1[1]); // this gets replicated -> 1: uv1.x uv1.y uv1.x uv1.y    2: uv1.x uv1.y uv2.x uv2.y
            Vector2 UV2 = new Vector2(vert.UV2[2], vert.UV2[3]);
            VertexColor1Texture2 _Tex = new VertexColor1Texture2(Color, UV1, UV2);

            GLTFVert ret = new GLTFVert();
            ret.Pos = _Pos;
            ret.Tex = _Tex;
            return ret;
        }
        public static Vertex GetAVFXVert(Vector3 pos, Vector3 normal, Vector4 tangent, Vector4 color, Vector2 tex1, Vector2 tex2) {
            var ret = new Vertex();
            color = color * 255;
            normal = normal * 128;
            tangent = tangent * 128;
            ret.Position = new float[] { pos.X, pos.Y, pos.Z, 1 };
            ret.Normal = new int[] { (int)normal.X, (int)normal.Y, (int)normal.Z, -1 };
            ret.Tangent = new int[] { (int)tangent.X, (int)tangent.Y, (int)tangent.Z, -1 };
            ret.Color = new int[] { (int)color.X, (int)color.Y, (int)color.Z, (int)color.W };

            ret.UV1 = new float[] { tex1.X, tex1.Y, tex1.X, tex1.Y };
            ret.UV2 = new float[] { tex1.X, tex1.Y, tex2.X, tex2.Y };

            return ret;
        }
        public static bool ImportModel(string path, out List<Vertex> V, out List<Index> I ) {
            V = new List<Vertex>();
            I = new List<Index>();
            var model = SharpGLTF.Schema2.ModelRoot.Load( path );
            PluginLog.Log( "Imported GLTF from: " + path );

            if(model.LogicalMeshes.Count > 0 ) {
                var mesh = model.LogicalMeshes[0];
                if(mesh.Primitives.Count > 0 ) {
                    var primitive = mesh.Primitives[0];

                    var positions = primitive.GetVertices( "POSITION" ).AsVector3Array();
                    var normals = primitive.GetVertices( "NORMAL" ).AsVector3Array();
                    var tangents = primitive.GetVertices( "TANGENT" ).AsVector4Array();
                    var colors = primitive.GetVertices( "COLOR_0" ).AsVector4Array();
                    var tex1s = primitive.GetVertices( "TEXCOORD_0" ).AsVector2Array();
                    var tex2s = primitive.GetVertices( "TEXCOORD_1" ).AsVector2Array();

                    var triangles = primitive.GetTriangleIndices();

                    for(int i = 0; i < positions.Count; i++ ) {
                        V.Add( GetAVFXVert( positions[i], normals[i], tangents[i], colors[i], tex1s[i], tex2s[i] ) );
                    }
                    foreach(var t in triangles ) {
                        var i_ = new Index();
                        i_.I1 = t.A;
                        i_.I2 = t.B;
                        i_.I3 = t.C;
                        I.Add( i_ );
                    }

                    return true;
                }
            }
            return false;
        }
    }
    public struct GLTFVert {
        public VertexPositionNormalTangent Pos;
        public VertexColor1Texture2 Tex;
    }
}
