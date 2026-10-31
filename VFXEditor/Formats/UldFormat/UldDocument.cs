using System.Collections.Generic;
using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.UldFormat {
    public class UldDocument : FileManagerRenamedDocument<UldFile> {
        public override string Id => "Uld";
        public override string Extension => "uld";

        public UldDocument( UldManager manager, string writeLocation ) : base( manager, writeLocation ) { }

        public UldDocument( UldManager manager, string writeLocation, string localPath, WorkspaceMetaRenamed data ) : base( manager, writeLocation, localPath, data ) { }

        protected override UldFile FileFromReader( BinaryReader reader, bool verify ) => new( reader, verify );

        // Redrawing the player does not reload UI layouts, the game only requests the .uld again when the window is created
        protected override List<string> GetPreviewWarnings() {
            var ret = base.GetPreviewWarnings();
            if( File != null && !string.IsNullOrEmpty( ReplacePath ) ) ret.Add( "After updating, close and reopen the in-game window that uses this layout to see your changes" );
            return ret;
        }
    }
}
