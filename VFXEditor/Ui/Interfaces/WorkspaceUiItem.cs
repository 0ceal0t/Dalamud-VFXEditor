using ImGuiNET;
using System.Collections.Generic;

namespace VfxEditor.Ui.Interfaces {
    public interface IWorkspaceUiItem : IIndexUiItem {
        public string GetWorkspaceId();

        public string GetRenamed();

        public void SetRenamed( string renamed );

        public void DrawRename();

        public void GetChildrenRename( Dictionary<string, string> RenameDict );

        public void SetChildrenRename( Dictionary<string, string> RenameDict );

        public static void GetRenamingMap( IWorkspaceUiItem item, Dictionary<string, string> renameDict ) {
            if( !string.IsNullOrEmpty( item.GetRenamed() ) ) renameDict[item.GetWorkspaceId()] = item.GetRenamed();
            item.GetChildrenRename( renameDict );
        }

        public static void ReadRenamingMap( IWorkspaceUiItem item, Dictionary<string, string> renameDict ) {
            if( renameDict.TryGetValue( item.GetWorkspaceId(), out var renamed ) ) item.SetRenamed( renamed );
            item.SetChildrenRename( renameDict );
        }

        public static void DrawRenameInput( IWorkspaceUiItem item, ref string renamed ) => ImGui.InputTextWithHint( "Name##Rename", item.GetDefaultText(), ref renamed, 255 );
    }
}
