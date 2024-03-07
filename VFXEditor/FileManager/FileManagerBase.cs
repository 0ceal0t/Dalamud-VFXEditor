using Dalamud.Interface.Windowing;
using VfxEditor.Data.Copy;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Select;
using VfxEditor.Ui;

namespace VfxEditor.FileManager {
    public abstract class FileManagerBase : DalamudWindow, IFileManagerSelect {
        public readonly string Id;
        public readonly string Title;
        public readonly string Extension;
        public readonly string WorkspaceKey;
        public readonly string WorkspacePath;

        public readonly ManagerConfiguration Configuration;

        public readonly CopyManager Copy = new();

        public readonly WindowSystem WindowSystem = new();

        public abstract string NewWriteLocation { get; }

        public SelectDialog SourceSelect { get; protected set; }
        public SelectDialog ReplaceSelect { get; protected set; }

        protected FileManagerBase( string title, string id, string extension, string workspaceKey, string workspacePath ) : base( title, true, new( 800, 1000 ), Plugin.WindowSystem ) {
            Title = title;
            Extension = extension;
            WorkspaceKey = workspaceKey;
            WorkspacePath = workspacePath;
            Id = id;

            Configuration = Plugin.Configuration.GetManagerConfig( Id );
        }

        public ManagerConfiguration GetConfig() => Configuration;

        public void ShowSource() => SourceSelect?.Show();

        public void ShowReplace() => ReplaceSelect?.Show();

        public abstract void SetSource( SelectResult result );

        public abstract void SetReplace( SelectResult result );

        public string GetId() => Id;

        public string GetName() => Id.ToLower();

        public WindowSystem GetWindowSystem() => WindowSystem;
    }
}
