using System;
using System.Collections.Generic;
using System.IO;

using AVFXLib.Models;

using VFXEditor.Helper;
using VFXEditor.Textools;

namespace VFXEditor.Document {
    public partial class DocumentManager {
        // ========== PENUMBRA ==============

        public void PenumbraExport( string modFolder, bool exportAll ) {
            if( exportAll ) {
                foreach( var doc in Documents ) {
                    PenumbraExport( doc.Main?.AVFX, doc.Replace.Path, modFolder );
                }
            }
            else {
                PenumbraExport( ActiveDocument.Main?.AVFX, ActiveDocument.Replace.Path, modFolder );
            }
        }

        private static void PenumbraExport( AVFXBase avfx, string path, string modFolder ) {
            if( string.IsNullOrEmpty( path ) || avfx == null ) return;

            var data = avfx.ToAVFX().ToBytes();
            PenumbraHelper.WriteBytes( data, modFolder, path );
        }

        // ======== TEXTOOLS ============

        public void TextoolsExport( BinaryWriter writer, bool exportAll, List<TTMPL_Simple> simpleParts, ref int modOffset ) {
            if( exportAll ) {
                foreach( var doc in Documents ) {
                    TextoolsExport( doc.Main?.AVFX, doc.Replace.Path, writer, simpleParts, ref modOffset );
                }
            }
            else {
                TextoolsExport( ActiveDocument.Main?.AVFX, ActiveDocument.Replace.Path, writer, simpleParts, ref modOffset );
            }
        }

        private static void TextoolsExport( AVFXBase avfx, string path, BinaryWriter writer, List<TTMPL_Simple> simpleParts, ref int modOffset ) {
            if( string.IsNullOrEmpty( path ) || avfx == null ) return;

            var modData = TextoolsHelper.CreateType2Data( avfx.ToAVFX().ToBytes() );
            simpleParts.Add( TextoolsHelper.CreateModResource( path, modOffset, modData.Length ) );
            writer.Write( modData );
            modOffset += modData.Length;
        }

        // ======== WORKSPACE ===========

        public WorkspaceMetaDocument[] WorkspaceExport( string saveLocation ) {
            var vfxRootPath = Path.Combine( saveLocation, "VFX" );
            Directory.CreateDirectory( vfxRootPath );

            var docId = 0;
            List<WorkspaceMetaDocument> docMeta = new();
            foreach( var entry in Documents ) {
                var newPath = "";
                if( entry.Main != null ) {
                    newPath = $"VFX_{docId++}.avfx";
                    var newFullPath = Path.Combine( vfxRootPath, newPath );
                    File.WriteAllBytes( newFullPath, entry.Main.AVFX.ToAVFX().ToBytes() );
                }
                docMeta.Add( new WorkspaceMetaDocument {
                    Source = entry.Source,
                    Replace = entry.Replace,
                    RelativeLocation = newPath,
                    Renaming = ( entry.Main == null ) ? new Dictionary<string, string>() : entry.Main.GetRenamingMap()
                } );
            }
            return docMeta.ToArray();
        }
    }
}
