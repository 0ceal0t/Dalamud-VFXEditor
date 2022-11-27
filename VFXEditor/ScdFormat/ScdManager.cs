using ImGuiNET;
using NAudio.Wave;
using System.IO;
using VfxEditor.Data;
using VfxEditor.FileManager;
using VfxEditor.Select.ScdSelect;

namespace VfxEditor.ScdFormat {
    public class ScdManager : FileManagerWindow<ScdDocument, WorkspaceMetaScd, ScdFile> {
        public static ScdSelectDialog SourceSelect { get; private set; }
        public static ScdSelectDialog ReplaceSelect { get; private set; }
        public static CopyManager Copy { get; private set; } = new();

        public static string ConvertWav => Path.Combine( Plugin.Configuration.WriteLocation, $"temp_out.wav" ).Replace( '\\', '/' );
        public static string ConvertOgg => Path.Combine( Plugin.Configuration.WriteLocation, $"temp_out.ogg" ).Replace( '\\', '/' );

        public static void Setup() {
            SourceSelect = new ScdSelectDialog( "Scd Select [ORIGINAL]", Plugin.Configuration.RecentSelectsScd, true, SetSourceGlobal );
            ReplaceSelect = new ScdSelectDialog( "Scd Select [MODIFIED]", Plugin.Configuration.RecentSelectsScd, false, SetReplaceGlobal );
        }

        public static void SetSourceGlobal( SelectResult result ) {
            Plugin.ScdManager?.SetSource( result );
            Plugin.Configuration.AddRecent( Plugin.Configuration.RecentSelectsScd, result );
        }

        public static void SetReplaceGlobal( SelectResult result ) {
            Plugin.ScdManager?.SetReplace( result );
            Plugin.Configuration.AddRecent( Plugin.Configuration.RecentSelectsScd, result );
        }

        public static readonly string PenumbraPath = "Scd";

        public ScdManager() : base( title: "Scd Editor", id: "Scd", tempFilePrefix: "ScdTemp", extension: "scd", penumbaPath: PenumbraPath ) { }

        protected override ScdDocument GetNewDocument() => new( LocalPath );

        protected override ScdDocument GetImportedDocument( string localPath, WorkspaceMetaScd data ) => new( LocalPath, localPath, data.Source, data.Replace );

        protected override void DrawMenu() { }

        public override void Dispose() {
            base.Dispose();
            SourceSelect.Hide();
            ReplaceSelect.Hide();
            CurrentFile?.Dispose();
            ScdUtils.Cleanup();
        }

        public override void DrawBody() {
            SourceSelect.Draw();
            ReplaceSelect.Draw();
            base.DrawBody();
        }
    }
}
