using VfxEditor.Data;

namespace VfxEditor.FileManager.Interfaces {
    public interface IFileManagerSelect {
        public string GetId();

        public CopyManager GetCopyManager();

        public CommandManager GetCommandManager();

        public ManagerConfiguration GetConfig();
    }
}
