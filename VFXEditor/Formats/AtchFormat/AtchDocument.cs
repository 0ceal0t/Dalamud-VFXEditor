using System.Collections.Generic;
using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Interop.Penumbra;
using VfxEditor.Utils;

namespace VfxEditor.Formats.AtchFormat {
    public class AtchDocument : FileManagerBasicDocument<AtchFile> {
        public override string Id => "Atch";
        public override string Extension => "atch";

        public AtchDocument( FileManagerBase manager, string writeLocation ) : base( manager, writeLocation ) { }

        public AtchDocument( FileManagerBase manager, string writeLocation, string localPath, WorkspaceMetaBasic data ) : base( manager, writeLocation, localPath, data ) { }

        protected override AtchFile FileFromReader( BinaryReader reader, bool verify ) => new( reader );

        // Penumbra never loads .atch files from mods, edits have to be stored as ATCH meta manipulations instead
        public override void PenumbraExport( string modFolder, string groupOption, Dictionary<string, string> filesOut ) { }

        public override void PenumbraExportManipulations( List<object> manipulationsOut ) {
            if( File == null || string.IsNullOrEmpty( ReplacePath ) ) return;
            manipulationsOut.AddRange( PenumbraMeta.ToJson( ReplacePath, File ) );
        }

        protected override List<string> GetPreviewWarnings() {
            var ret = base.GetPreviewWarnings();
            ret.AddRange( AtchPenumbraSupport.GetUnsupported( ReplacePath, File ) );
            return ret;
        }
    }
}