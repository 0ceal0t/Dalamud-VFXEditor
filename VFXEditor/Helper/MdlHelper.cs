using Dalamud.Logging;
using Lumina.Data.Files;
using Lumina.Models.Models;
using System.Collections.Generic;
using System.Numerics;

using VfxVertex = AVFXLib.Models.Vertex;
using VfxIndex = AVFXLib.Models.Index;


namespace VFXEditor.Helper {
    public static class MdlHelper {
       public static bool ImportModel( string localPath, out List<VfxVertex> vOut, out List<VfxIndex> iOut ) {
            vOut = new List<VfxVertex>();
            iOut = new List<VfxIndex>();
            if( !Plugin.DataManager.FileExists( localPath ) ) return false;

            PluginLog.Log( "Importing MDL from: " + localPath );

            var file = Plugin.DataManager.GameData.GetFileFromDisk<MdlFile>( localPath );
            var mdl = new Model( file );

            foreach( var mesh in mdl.GetMeshesByType(Mesh.MeshType.Main) ) {
                foreach( var v in mesh.Vertices ) {
                    
                }
                foreach( var i in mesh.Indices) {

                }
            }

            return true;
        }

        private static VfxVertex GetAVFXVert( Vector4 pos, Vector4 normal, Vector4 tangent, Vector4 color, Vector2 tex1, Vector2 tex2 ) {
            var ret = new VfxVertex();
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
