using Dalamud.Interface.Windowing;
using VfxEditor.Data;
using VfxEditor.Data.Copy;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Select;
using VfxEditor.Ui;

namespace VfxEditor.FileManager {
    public abstract class FileManagerBase : DalamudWindow, IFileManagerSelect {
        public readonly FileManagerGroupBase Group;
        public string FormatName => Group.FormatName;
        public string Title => Group.Title;
        public string Extension => Group.Extension;

        public readonly int WindowId;

        public readonly ManagerConfiguration Configuration;

        public readonly CopyManager Copy = new();

        public readonly WindowSystem WindowSystem = new();

        public abstract string NewWriteLocation { get; }

        public SelectDialog SourceSelect { get; protected set; }
        public SelectDialog ReplaceSelect { get; protected set; }

        // The id (everything after "###") must match what FileManager.DrawBody() later assigns to
        // WindowName ($"...###{Title}-{WindowId}"), otherwise this window is born under one ImGui
        // id and permanently switches to a different one the first time it draws - two entirely
        // separate windows as far as ImGui/its ini persistence are concerned, so anything applied
        // to the constructor's id (including SetMeta()'s restored size/position) gets abandoned.
        protected FileManagerBase( FileManagerGroupBase group ) :
            base( $"{group.Title}###{group.Title}-{group.WindowId}", true, new( 800, 1000 ), group.WindowSystem, isMainWindow: true ) {

            Group = group;
            WindowId = group.NewWindowId;
            Configuration = Plugin.Configuration.GetManagerConfig( FormatName );
        }

        public ManagerConfiguration GetConfig() => Configuration;

        public void ShowSource() => SourceSelect?.Show();

        public void ShowReplace() => ReplaceSelect?.Show();

        public abstract void SetSource( SelectResult result );

        public abstract void SetReplace( SelectResult result );

        public string GetId() => FormatName;

        public WindowSystem GetWindowSystem() => WindowSystem;

        public int GetWindowId() => WindowId;
    }
}
