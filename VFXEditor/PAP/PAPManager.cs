using VFXEditor.FileManager;
using VFXSelect;
using VFXSelect.PAP;

namespace VFXEditor.PAP {
    public partial class PAPManager : FileManager<PAPDocument, WorkspaceMetaPap, PAPFile> {
        public static PAPSelectDialog SourceSelect { get; private set; }
        public static PAPSelectDialog ReplaceSelect { get; private set; }
        public static PAPSelectIndexDialog IndexDialog { get; private set; }

        public static void Setup() {
            SourceSelect = new PAPSelectDialog(
                "Pap Select [SOURCE]",
                null,
                true,
                SetSourceGlobal
            );

            ReplaceSelect = new PAPSelectDialog(
                "Pap Select [TARGET]",
                null,
                false,
                SetReplaceGlobal
            );
            IndexDialog = new PAPSelectIndexDialog();
        }

        public static void SetSourceGlobal( SelectResult result ) {
            Plugin.PapManager?.SetSource( result );
        }

        public static void SetReplaceGlobal( SelectResult result ) {
            Plugin.PapManager?.SetReplace( result );
        }

        public static readonly string PenumbraPath = "Pap";

        // =====================

        public PAPManager() : base( title: "Pap Editor", id: "Pap", tempFilePrefix: "PapTemp", extension: "pap", penumbaPath: PenumbraPath ) { }

        protected override PAPDocument GetNewDocument() => new( LocalPath );

        protected override PAPDocument GetImportedDocument( string localPath, WorkspaceMetaPap data ) => new( LocalPath, localPath, data.Source, data.Replace );

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
