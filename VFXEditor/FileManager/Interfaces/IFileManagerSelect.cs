using Dalamud.Interface.Windowing;
using VfxEditor.Data;

namespace VfxEditor.FileManager.Interfaces {
    public interface IFileManagerSelect {
        public string GetId();

        public int GetWindowId();

        public ManagerConfiguration GetConfig();

        public WindowSystem GetWindowSystem();
    }
}
