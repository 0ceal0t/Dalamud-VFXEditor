using ImGuiNET;
using VfxEditor.Data;
using VfxEditor.FileManager;
using VfxEditor.Select.PapSelect;

namespace VfxEditor.PapFormat {
    public partial class PapManager : FileManagerWindow<PapDocument, PapFile, WorkspaceMetaBasic> {
        public static PapSelectDialog SourceSelect { get; private set; }
        public static PapSelectDialog ReplaceSelect { get; private set; }
        public static PapSelectIndexDialog IndexDialog { get; private set; }
        public static CopyManager Copy { get; private set; } = new();

        public static void Setup() {
            SourceSelect = new PapSelectDialog( "Pap Select [LOADED]", Plugin.Configuration.RecentSelectsPAP, true, SetSourceGlobal );
            ReplaceSelect = new PapSelectDialog( "Pap Select [REPLACED]", Plugin.Configuration.RecentSelectsPAP, false, SetReplaceGlobal );
            IndexDialog = new PapSelectIndexDialog();
        }

        public static void SetSourceGlobal( SelectResult result ) {
            Plugin.PapManager?.SetSource( result );
            Plugin.Configuration.AddRecent( Plugin.Configuration.RecentSelectsPAP, result );
        }

        public static void SetReplaceGlobal( SelectResult result ) {
            Plugin.PapManager?.SetReplace( result );
            Plugin.Configuration.AddRecent( Plugin.Configuration.RecentSelectsPAP, result );
        }

        public static readonly string PenumbraPath = "Pap";

        // =====================

        public PapManager() : base( title: "Pap Editor", id: "Pap", extension: "pap", penumbaPath: PenumbraPath ) { }

        protected override PapDocument GetNewDocument() => new( LocalPath );

        protected override PapDocument GetImportedDocument( string localPath, WorkspaceMetaBasic data ) => new( LocalPath, localPath, data.Source, data.Replace );

        protected override void DrawMenu() {
            if( CurrentFile == null ) return;

            if( ImGui.BeginMenu( "Edit##Menu" ) ) {
                CopyManager.Pap.Draw();
                CommandManager.Pap.Draw();
                ImGui.EndMenu();
            }
        }

        public override void Dispose() {
            base.Dispose();
            SourceSelect.Hide();
            ReplaceSelect.Hide();
            IndexDialog.Hide();
        }

        public override void DrawBody() {
            SourceSelect.Draw();
            ReplaceSelect.Draw();
            IndexDialog.Draw();
            base.DrawBody();
        }
    }
}
