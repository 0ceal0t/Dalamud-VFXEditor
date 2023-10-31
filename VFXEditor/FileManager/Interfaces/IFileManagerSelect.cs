using Dalamud.Interface.Windowing;

namespace VfxEditor.FileManager.Interfaces {
    public interface IFileManagerSelect {
        public string GetId();

        public ManagerConfiguration GetConfig();

        public WindowSystem GetWindowSystem();
    }
}
