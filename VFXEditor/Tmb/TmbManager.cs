using VFXEditor.FileManager;
using VFXSelect;
using VFXSelect.VFX;

namespace VFXEditor.Tmb {
    public partial class TmbManager : FileManager<TmbDocument, WorkspaceMetaTmb, TmbFile> {
        public static TmbSelectDialog SourceSelect { get; private set; }
        public static TmbSelectDialog ReplaceSelect { get; private set; }

        public static void Setup() {
            SourceSelect = new TmbSelectDialog(
                "Tmb Select [SOURCE]",
                null,
                true,
                SetSourceGlobal
            );

            ReplaceSelect = new TmbSelectDialog(
                "Tmb Select [TARGET]",
                null,
                false,
                SetReplaceGlobal
            );
        }

        public static void SetSourceGlobal( SelectResult result ) {
            Plugin.TmbManager?.SetSource( result );
        }

        public static void SetReplaceGlobal( SelectResult result ) {
            Plugin.TmbManager?.SetReplace( result );
        }

        public static readonly string PenumbraPath = "Tmb";

        public TmbManager() : base( title: "Tmb Editor", id: "Tmb", tempFilePrefix: "TmbTemp", extension: "tmb", penumbaPath: PenumbraPath ) { }

        protected override TmbDocument GetNewDocument() => new( LocalPath );

        protected override TmbDocument GetImportedDocument( string localPath, WorkspaceMetaTmb data ) => new( LocalPath, localPath, data.Source, data.Replace );

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
