using System.Collections.Generic;
using System.IO;

using VfxEditor.Utils;
using VfxEditor.TexTools;

namespace VfxEditor.PapFormat {
    public partial class PapDocument {
        public override void PenumbraExport( string modFolder ) {
            var path = Replace.Path;
            if( string.IsNullOrEmpty( path ) || CurrentFile == null ) return;

            var data = CurrentFile.ToBytes();
            PenumbraUtils.WriteBytes( data, modFolder, path );
        }

        public override void TextoolsExport( BinaryWriter writer, List<TTMPL_Simple> simpleParts, ref int modOffset ) {
            var path = Replace.Path;
            if( string.IsNullOrEmpty( path ) || CurrentFile == null ) return;

            var modData = TexToolsUtils.CreateType2Data( CurrentFile.ToBytes() );
            simpleParts.Add( TexToolsUtils.CreateModResource( path, modOffset, modData.Length ) );
            writer.Write( modData );
            modOffset += modData.Length;
        }

        public override void WorkspaceExport( List<WorkspaceMetaPap> papMeta, string rootPath, string newPath ) {
            if( CurrentFile != null ) {
                var newFullPath = Path.Combine( rootPath, newPath );
                File.WriteAllBytes( newFullPath, CurrentFile.ToBytes() );
                papMeta.Add( new WorkspaceMetaPap() {
                    RelativeLocation = newPath,
                    Replace = Replace,
                    Source = Source
                } );
            }
        }
    }
}
