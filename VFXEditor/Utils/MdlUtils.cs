using Lumina.Data.Files;
using Lumina.Models.Models;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.AvfxFormat;

namespace VfxEditor.Utils {
    // WIP, Not currently being used

    public static class MdlUtils {
        public static bool ImportModel( string localPath, out List<AvfxVertex> vertexesOut, out List<AvfxIndex> indexesOut ) {
            vertexesOut = [];
            indexesOut = [];
            if( !Dalamud.DataManager.FileExists( localPath ) ) return false;

            Dalamud.Log( "Importing MDL from: " + localPath );

            var file = Dalamud.DataManager.GameData.GetFileFromDisk<MdlFile>( localPath );
            var mdl = new Model( file );

            foreach( var mesh in mdl.GetMeshesByType( Mesh.MeshType.Main ) ) {
                var idxStart = indexesOut.Count; // accounts for multiple meshes
                foreach( var v in mesh.Vertices ) {
                    var pos = v.Position;
                    var normal = v.Normal;
                    var tangent = v.Tangent1;
                    var color = v.Color;
                    var uv = v.UV;

                    if( pos == null || normal == null || tangent == null || color == null || uv == null ) {
                        Dalamud.Error( "Missing model data" );
                        return false;
                    }

                    vertexesOut.Add( GetAvfxVert(
                        pos.Value,
                        normal.Value,
                        tangent.Value,
                        color.Value,
                        new Vector2( uv.Value.X, uv.Value.Y ),
                        new Vector2( uv.Value.Z, uv.Value.W )
                    ) );
                }
                if( mesh.Indices.Length % 3 != 0 ) {
                    Dalamud.Error( "Indices not multiples of 3" );
                    return false;
                }
                for( var triangleIdx = 0; triangleIdx < ( mesh.Indices.Length / 3 ); triangleIdx++ ) {
                    var idx = triangleIdx * 3;
                    indexesOut.Add( new AvfxIndex(
                        idxStart + mesh.Indices[idx],
                        idxStart + mesh.Indices[idx + 1],
                        idxStart + mesh.Indices[idx + 2]
                   ) );
                }
                Dalamud.Log( "Imported MDL mesh" );
            }
            return true;
        }

        private static AvfxVertex GetAvfxVert( Vector4 pos, Vector3 normal, Vector4 tangent, Vector4 color, Vector2 uv1, Vector2 uv2 ) {
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
            ret.Uv3 = uv1;
            ret.Uv4 = uv2;

            return ret;
        }
    }
}
