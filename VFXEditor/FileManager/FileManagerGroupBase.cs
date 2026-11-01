using Dalamud.Interface.Windowing;
using VfxEditor.FileManager.Interfaces;

namespace VfxEditor.FileManager {
    public abstract class FileManagerGroupBase {
        public readonly string FormatName;
        public readonly string Title;
        public readonly string Extension;
        public readonly string WorkspaceKey;
        public readonly string WorkspacePath;

        public readonly WindowSystem WindowSystem = new();

        public int WindowId { get; protected set; } = 0;
        public int NewWindowId => WindowId++;

        protected FileManagerGroupBase( string title, string formatName, string extension, string workspaceKey, string workspacePath ) {
            Title = title;
            Extension = extension;
            WorkspaceKey = workspaceKey;
            WorkspacePath = workspacePath;
            FormatName = formatName;
        }

        public string GetId() => FormatName;

        public abstract FormatCategory Category { get; }

        public string GetName() => FormatName.ToLower();

        public abstract void SetLastFocusedManager( FileManagerBase manager );

        public abstract void AddDefaultDocument();

        public abstract void Draw();

        public abstract void NewWindow();

        public abstract void ToNewWindow( FileManagerBase currentManager, IFileDocument document );

        public abstract void DrawCloseWindow( FileManagerBase manager );

        public abstract bool DrawDragDrop( FileManagerBase manager, IFileDocument document, string documentName );
    }
}
