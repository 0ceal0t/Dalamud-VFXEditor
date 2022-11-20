using ImGuiNET;
using NAudio.Wave;
using VfxEditor.FileManager;
using VfxEditor.Select.ScdSelect;

namespace VfxEditor.ScdFormat {
    // TODO: workspace import
    public class ScdManager : FileManager<ScdDocument, WorkspaceMetaScd, ScdFile> {
        public static ScdSelectDialog SourceSelect { get; private set; }
        public static ScdSelectDialog ReplaceSelect { get; private set; }

        public static void Setup() {
            SourceSelect = new ScdSelectDialog(
               "Scd Select [ORIGINAL]",
               Plugin.Configuration.RecentSelectsScd,
               true,
               SetSourceGlobal
            );

            ReplaceSelect = new ScdSelectDialog(
               "Scd Select [MODIFIED]",
               Plugin.Configuration.RecentSelectsScd,
               false,
               SetReplaceGlobal
            );
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
        }

        public override void DrawBody() {
            SourceSelect.Draw();
            ReplaceSelect.Draw();
            base.DrawBody();
        }
    }
}
