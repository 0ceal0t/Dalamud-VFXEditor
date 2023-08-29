namespace VfxEditor.TmbFormat.Utils {
    public class TmbRefreshIdsCommand : CompoundCommand {
        private readonly TmbFile File;

        public TmbRefreshIdsCommand( TmbFile file ) : base() {
            File = file;
        }

        public override void Execute() {
            base.Execute();
            File.RefreshIds();
        }

        public override void Redo() {
            base.Redo();
            File.RefreshIds();
        }

        public override void Undo() {
            base.Undo();
            File.RefreshIds();
        }
    }
}
