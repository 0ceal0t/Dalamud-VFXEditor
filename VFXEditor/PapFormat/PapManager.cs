using VFXEditor.FileManager;
using VFXEditor.Select.PAP;

namespace VFXEditor.PapFormat {
    public partial class PapManager : FileManager<PapDocument, WorkspaceMetaPap, PapFile> {
        public static PAPSelectDialog SourceSelect { get; private set; }
        public static PAPSelectDialog ReplaceSelect { get; private set; }
        public static PapSelectIndexDialog IndexDialog { get; private set; }

        public static void Setup() {
            SourceSelect = new PAPSelectDialog(
                "Pap Select [SOURCE]",
                VfxEditor.Configuration.RecentSelectsPAP,
                true,
                SetSourceGlobal
            );

            ReplaceSelect = new PAPSelectDialog(
                "Pap Select [TARGET]",
                VfxEditor.Configuration.RecentSelectsPAP,
                false,
                SetReplaceGlobal
            );

            IndexDialog = new PapSelectIndexDialog();
        }

        public static void SetSourceGlobal( SelectResult result ) {
            VfxEditor.PapManager?.SetSource( result );
            VfxEditor.Configuration.AddRecent( VfxEditor.Configuration.RecentSelectsPAP, result );
        }

        public static void SetReplaceGlobal( SelectResult result ) {
            VfxEditor.PapManager?.SetReplace( result );
            VfxEditor.Configuration.AddRecent( VfxEditor.Configuration.RecentSelectsPAP, result );
        }

        public static readonly string PenumbraPath = "Pap";

        // =====================

        public PapManager() : base( title: "Pap Editor", id: "Pap", tempFilePrefix: "PapTemp", extension: "pap", penumbaPath: PenumbraPath ) { }

        protected override PapDocument GetNewDocument() => new( LocalPath );

        protected override PapDocument GetImportedDocument( string localPath, WorkspaceMetaPap data ) => new( LocalPath, localPath, data.Source, data.Replace );

        protected override void DrawMenu() {
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
