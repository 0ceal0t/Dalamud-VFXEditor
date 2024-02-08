namespace VfxEditor {
    public interface ICommand {
        public void Undo();

        public void Redo();
    }
}