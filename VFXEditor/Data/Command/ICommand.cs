namespace VfxEditor.Data.Command {
    public interface ICommand {
        public void Undo();

        public void Redo();
    }
}