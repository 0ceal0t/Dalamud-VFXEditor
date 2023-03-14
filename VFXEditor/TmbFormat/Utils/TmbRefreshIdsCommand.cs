namespace VfxEditor.TmbFormat.Utils {
    public class TmbRefreshIdsCommand : ICommand {
        private readonly TmbFile File;

        public TmbRefreshIdsCommand( TmbFile file ) {
            File = file;
        }

        public void Execute() => File.RefreshIds();

        public void Redo() => File.RefreshIds();

        public void Undo() => File.RefreshIds();
    }
}
