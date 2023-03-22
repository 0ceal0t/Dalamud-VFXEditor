using ImGuiNET;
using VfxEditor.Data;
using VfxEditor.FileManager;
using VfxEditor.Select.Pap;
using VfxEditor.Utils;

namespace VfxEditor.PapFormat {
    public partial class PapManager : FileManagerWindow<PapDocument, PapFile, WorkspaceMetaBasic> {
        public readonly PapSelectIndexDialog IndexDialog;

        public PapManager() : base( "Pap Editor", "Pap", "pap", "Pap", "Pap" ) {
            SourceSelect = new PapSelectDialog( "Pap Select [LOADED]", this, true );
            ReplaceSelect = new PapSelectDialog( "Pap Select [REPLACED]", this, false );
            IndexDialog = new PapSelectIndexDialog();
        }

        protected override PapDocument GetNewDocument() => new( this, LocalPath );

        protected override PapDocument GetWorkspaceDocument( WorkspaceMetaBasic data, string localPath ) =>
            new( this, LocalPath, WorkspaceUtils.ResolveWorkspacePath( data.RelativeLocation, localPath ), data.Source, data.Replace );

        public override void DrawBody() {
            IndexDialog.Draw();
            base.DrawBody();
        }

        public override void Dispose() {
            IndexDialog.Hide();
            base.Dispose();
        }
    }
}
