using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Select.Formats;
using VfxEditor.Utils;

namespace VfxEditor.ScdFormat {
    public class ScdManager : FileManager<ScdDocument, ScdFile, WorkspaceMetaBasic> {
        public static string ConvertWav => Path.Combine( Plugin.Configuration.WriteLocation, $"temp_out.wav" ).Replace( '\\', '/' );
        public static string ConvertOgg => Path.Combine( Plugin.Configuration.WriteLocation, $"temp_out.ogg" ).Replace( '\\', '/' );

        public ScdManager() : base( "Scd Editor", "Scd" ) {
            SourceSelect = new ScdSelectDialog( "Scd Select [LOADED]", this, true );
            ReplaceSelect = new ScdSelectDialog( "Scd Select [REPLACED]", this, false );
        }

        protected override ScdDocument GetNewDocument() => new( this, NewWriteLocation );

        protected override ScdDocument GetWorkspaceDocument( WorkspaceMetaBasic data, string localPath ) => new( this, NewWriteLocation, localPath, data );

        public override void Reset() {
            File?.Dispose();
            base.Reset();
            ScdUtils.Cleanup();
        }
    }
}
