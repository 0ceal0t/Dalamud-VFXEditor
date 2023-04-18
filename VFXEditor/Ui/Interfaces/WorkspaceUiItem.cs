using Dalamud.Interface;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
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

        public static void DrawRenameBox( IWorkspaceUiItem item, string parentId, ref string renamed, ref string renamedTemp, ref bool renaming ) {
            var id = parentId + "/Rename";
            var inputSize = UiUtils.GetOffsetInputSize( FontAwesomeIcon.Check );
            ImGui.PushStyleVar( ImGuiStyleVar.ItemSpacing, new Vector2( ImGui.GetStyle().ItemInnerSpacing.X, ImGui.GetStyle().ItemSpacing.Y ) );

            if( renaming ) {
                ImGui.SetNextItemWidth( inputSize );
                ImGui.InputText( $"{id}-Input", ref renamedTemp, 255 );

                ImGui.PushFont( UiBuilder.IconFont );

                ImGui.SameLine();
                if( ImGui.Button( $"{( char )FontAwesomeIcon.Check}" + id ) ) {
                    if( string.IsNullOrEmpty( renamedTemp ) || renamed == item.GetDefaultText() ) renamed = null;
                    else renamed = renamedTemp;
                    renaming = false;
                }

                ImGui.SameLine();
                if( UiUtils.RemoveButton( $"{( char )FontAwesomeIcon.Times}" + id ) ) renaming = false;

                ImGui.PopFont();

                ImGui.SameLine();
                if( ImGui.Button( "Reset" + id ) ) {
                    renamed = null;
                    renaming = false;
                }
            }
            else {
                var currentText = string.IsNullOrEmpty( renamed ) ? item.GetDefaultText() : renamed;
                ImGui.PushStyleVar( ImGuiStyleVar.Alpha, 0.8f );
                ImGui.SetNextItemWidth( inputSize );
                ImGui.InputText( $"{id}-Input", ref currentText, 255, ImGuiInputTextFlags.ReadOnly );
                ImGui.PopStyleVar( 1 );

                ImGui.PushFont( UiBuilder.IconFont );

                ImGui.SameLine();
                if( ImGui.Button( $"{( char )FontAwesomeIcon.PencilAlt}" + id ) ) {
                    renaming = true;
                    renamedTemp = currentText;
                }

                ImGui.PopFont();
            }

            ImGui.PopStyleVar( 1 );
        }
    }
}
