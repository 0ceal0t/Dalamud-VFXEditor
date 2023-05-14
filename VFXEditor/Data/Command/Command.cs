namespace VfxEditor {
    public interface ICommand {
        void Execute();

        void Undo();

        void Redo();
    }
}