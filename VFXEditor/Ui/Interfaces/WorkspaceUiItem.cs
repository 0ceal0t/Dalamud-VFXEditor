using Dalamud.Interface;
using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.Utils;

namespace VfxEditor.Ui.Interfaces {
    public interface IWorkspaceUiItem : ISelectableUiItem {
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

        public static void DrawRenameBox( IWorkspaceUiItem item, ref string renamed, ref string renamedTemp, ref bool renaming ) {
            var inputSize = UiUtils.GetOffsetInputSize( FontAwesomeIcon.Check );
            using var _ = ImRaii.PushId( "Rename" );
            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( ImGui.GetStyle().ItemInnerSpacing.X, ImGui.GetStyle().ItemSpacing.Y ) );

            if( renaming ) {
                ImGui.SetNextItemWidth( inputSize );
                ImGui.InputText( "##Rename", ref renamedTemp, 255 );

                using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                    ImGui.SameLine();
                    if( ImGui.Button( FontAwesomeIcon.Check.ToIconString() ) ) {
                        renamed = ( string.IsNullOrEmpty( renamedTemp ) || renamed == item.GetDefaultText() ) ? null : renamedTemp;
                        renaming = false;
                    }

                    ImGui.SameLine();
                    if( UiUtils.RemoveButton( FontAwesomeIcon.Times.ToIconString() ) ) renaming = false;
                }

                ImGui.SameLine();
                if( ImGui.Button( "Reset" ) ) {
                    renamed = null;
                    renaming = false;
                }
            }
            else {
                var currentText = string.IsNullOrEmpty( renamed ) ? item.GetDefaultText() : renamed;
                using( var disabled = ImRaii.PushStyle( ImGuiStyleVar.Alpha, 0.8f ) ) {
                    ImGui.SetNextItemWidth( inputSize );
                    ImGui.InputText( "", ref currentText, 255, ImGuiInputTextFlags.ReadOnly );
                }

                using var font = ImRaii.PushFont( UiBuilder.IconFont );
                ImGui.SameLine();
                if( ImGui.Button( FontAwesomeIcon.PencilAlt.ToIconString() ) ) {
                    renaming = true;
                    renamedTemp = currentText;
                }
            }
        }
    }
}
