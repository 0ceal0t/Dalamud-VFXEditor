using Dalamud.Interface;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX {
    public abstract class UIWorkspaceItem : UIItem {
#nullable enable
        public string? Renamed;
#nullable disable
        private string RenamedTemp;
        private bool CurrentlyRenaming = false;

        public override string GetText() {
            return string.IsNullOrEmpty( Renamed ) ? GetDefaultText() : Renamed;
        }

        // ====== GETTING DATA FROM WORKSPACE META
        public abstract string GetWorkspaceId();

        public void PopulateWorkspaceMeta( Dictionary<string, string> RenameDict ) {
            var path = GetWorkspaceId();
            if( !string.IsNullOrEmpty( Renamed ) ) {
                RenameDict[path] = Renamed;
            }
            PopulateWorkspaceMetaChildren(RenameDict);
        }
        public virtual void PopulateWorkspaceMetaChildren( Dictionary<string, string> RenameDict ) { }

        public void ReadWorkspaceMeta( Dictionary<string, string> RenameDict ) {
            var path = GetWorkspaceId();
            if( RenameDict.TryGetValue( path, out var renamed ) ) {
                Renamed = renamed;
            }
            ReadWorkspaceMetaChildren( RenameDict );
        }
        public virtual void ReadWorkspaceMetaChildren( Dictionary<string, string> RenameDict ) { }

        public void DrawRename( string _id ) {
            var id = _id + "/Rename";
            if( CurrentlyRenaming ) {
                ImGui.InputText( "Name" + id, ref RenamedTemp, 255 );

                ImGui.PushFont( UiBuilder.IconFont );

                ImGui.SameLine();
                if( ImGui.Button( $"{(char) FontAwesomeIcon.Check}" + id ) ) {
                    if( string.IsNullOrEmpty( RenamedTemp ) || Renamed == GetDefaultText() ) {
                        Renamed = null;
                    }
                    else {
                        Renamed = RenamedTemp;
                    }
                    CurrentlyRenaming = false;
                }

                ImGui.SameLine();
                ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
                if( UIUtils.RemoveButton( $"{( char )FontAwesomeIcon.Times}" + id ) ) {
                    CurrentlyRenaming = false;
                }

                ImGui.PopFont();

                ImGui.SameLine();
                ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
                if( ImGui.Button( "Reset" + id ) ) {
                    Renamed = null;
                    CurrentlyRenaming = false;
                }
            }
            else {
                var currentText = string.IsNullOrEmpty( Renamed ) ? GetDefaultText() : Renamed;
                ImGui.InputText( "Name" + id, ref currentText, 255, ImGuiInputTextFlags.ReadOnly );

                ImGui.PushFont( UiBuilder.IconFont );

                ImGui.SameLine();
                if( ImGui.Button( $"{( char )FontAwesomeIcon.Edit}" + id ) ) {
                    CurrentlyRenaming = true;
                    RenamedTemp = currentText;
                }

                ImGui.PopFont();
            }
        }
    }
}
