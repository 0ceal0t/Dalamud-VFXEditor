using Dalamud.Logging;
using Lumina.Data.Files;
using Lumina.Models.Models;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.AvfxFormat;

namespace VfxEditor.Utils {
    public static class MdlUtils {
        public static bool ImportModel( string localPath, out List<AvfxVertex> vertexesOut, out List<AvfxIndex> indexesOut ) {
            vertexesOut = new List<AvfxVertex>();
            indexesOut = new List<AvfxIndex>();
            //var d = File.ReadAllBytes( localPath );
            if( !Plugin.DataManager.FileExists( localPath ) ) return false;

            PluginLog.Log( "Importing MDL from: " + localPath );

            var file = Plugin.DataManager.GameData.GetFileFromDisk<MdlFile>( localPath );
            var mdl = new Model( file );

            foreach( var mesh in mdl.GetMeshesByType( Mesh.MeshType.Main ) ) {
                var idxStart = indexesOut.Count; // accounts for multiple meshes
                foreach( var v in mesh.Vertices ) {
                    var pos = v.Position;
                    var normal = v.Normal;
                    var tangent = v.Tangent1;
                    var color = v.Color;
                    var uv = v.UV;

                    if (pos == null || normal == null || tangent == null || color == null || uv == null) {
                        PluginLog.Error( "Missing model data" );
                        return false;
                    }

                    vertexesOut.Add( GetAvfxVert(
                        pos.Value,
                        normal.Value,
                        tangent.Value,
                        color.Value,
                        new Vector2(uv.Value.X, uv.Value.Y),
                        new Vector2(uv.Value.Z, uv.Value.W)
                    ) );
                }
                if (mesh.Indices.Length % 3 != 0) {
                    PluginLog.Error( "Indices not multiples of 3" );
                    return false;
                }
                for (var triangleIdx = 0; triangleIdx < (mesh.Indices.Length / 3); triangleIdx++ ) {
                    var idx = triangleIdx * 3;
                    indexesOut.Add( new AvfxIndex( 
                        idxStart + mesh.Indices[idx],
                        idxStart + mesh.Indices[idx + 1],
                        idxStart + mesh.Indices[idx + 2] 
                   ) );
                }
                PluginLog.Log( "Imported MDL mesh" );
            }
            return true;
        }

        private static AvfxVertex GetAvfxVert( Vector4 pos, Vector3 normal, Vector4 tangent, Vector4 color, Vector2 tex1, Vector2 tex2 ) {
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
    }
}
