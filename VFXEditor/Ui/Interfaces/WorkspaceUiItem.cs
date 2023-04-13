using Dalamud.Interface;
using ImGuiNET;
using System;
using System.Collections.Generic;
using VfxEditor.Utils;

namespace VfxEditor.Ui.Interfaces {
    public interface IWorkspaceUiItem : ISelectableUiItem {
        public string GetWorkspaceId();
        public string GetRenamed();
        public void SetRenamed( string renamed );
        public void DrawRename( string parentId );

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

        public static void DrawRenameBox( IWorkspaceUiItem item, string parentId, ref string renamed, ref string renamedTemp, ref bool currentRenaming ) {
            var id = parentId + "/Rename";
            if( currentRenaming ) {
                ImGui.InputText( $"{id}-Input", ref renamedTemp, 255 );

                ImGui.PushFont( UiBuilder.IconFont );

                ImGui.SameLine();
                ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
                if( ImGui.Button( $"{( char )FontAwesomeIcon.Check}" + id ) ) {
                    if( string.IsNullOrEmpty( renamedTemp ) || renamed == item.GetDefaultText() ) renamed = null;
                    else renamed = renamedTemp;
                    currentRenaming = false;
                }

                ImGui.SameLine();
                ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
                if( UiUtils.RemoveButton( $"{( char )FontAwesomeIcon.Times}" + id ) ) {
                    currentRenaming = false;
                }

                ImGui.PopFont();

                ImGui.SameLine();
                ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
                if( ImGui.Button( "Reset" + id ) ) {
                    renamed = null;
                    currentRenaming = false;
                }
            }
            else {
                var currentText = string.IsNullOrEmpty( renamed ) ? item.GetDefaultText() : renamed;
                ImGui.PushStyleVar( ImGuiStyleVar.Alpha, 0.8f );
                ImGui.InputText( $"{id}-Input", ref currentText, 255, ImGuiInputTextFlags.ReadOnly );
                ImGui.PopStyleVar();

                ImGui.PushFont( UiBuilder.IconFont );

                ImGui.SameLine();
                ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
                if( ImGui.Button( $"{( char )FontAwesomeIcon.PencilAlt}" + id ) ) {
                    currentRenaming = true;
                    renamedTemp = currentText;
                }

                ImGui.PopFont();
            }
        }
    }
}
