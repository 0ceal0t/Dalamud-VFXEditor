using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX {
    public abstract class UIWorkspaceItem : UIItem {
        public string? Renamed;
        private string RenamedTemp;
        private bool CurrentlyRenaming = false;

        public override string GetText() {
            return string.IsNullOrEmpty( Renamed ) ? GetDefaultText() : Renamed;
        }

        // ====== GETTING DATA FROM WORKSPACE META
        public void PopulateWorkspaceMeta( string parentPath, Dictionary<string, string> RenameDict ) {
            var path = parentPath + "/" + GetDefaultText();
            if( !string.IsNullOrEmpty( Renamed ) ) {
                RenameDict[path] = Renamed;
            }
            PopulateWorkspaceMetaChildren( path );
        }
        public virtual void PopulateWorkspaceMetaChildren( string path ) { }

        public void ReadWorkspaceMeta( string parentPath, Dictionary<string, string> RenameDict ) {
            var path = parentPath + "/" + GetDefaultText();
            if( RenameDict.TryGetValue( path, out var renamed ) ) {
                Renamed = renamed;
            }
            ReadWorkspaceMetaChildren( path );
        }
        public virtual void ReadWorkspaceMetaChildren( string path ) { }

        public void DrawRename( string _id ) {
            var id = _id + "/Rename";
            if( CurrentlyRenaming ) {
                ImGui.InputText( "Name" + id, ref RenamedTemp, 255 );
                ImGui.SameLine();
                if( ImGui.Button( "Ok" + id ) ) {
                    if( string.IsNullOrEmpty( RenamedTemp ) || Renamed == GetDefaultText() ) {
                        Renamed = null;
                    }
                    else {
                        Renamed = RenamedTemp;
                    }
                    CurrentlyRenaming = false;
                }
                ImGui.SameLine();
                if( ImGui.Button( "Cancel" + id ) ) {
                    CurrentlyRenaming = false;
                }
                ImGui.SameLine();
                if( ImGui.Button( "Reset" + id ) ) {
                    Renamed = null;
                    CurrentlyRenaming = false;
                }
            }
            else {
                var currentText = string.IsNullOrEmpty( Renamed ) ? GetDefaultText() : Renamed;
                ImGui.InputText( "Name" + id, ref currentText, 255, ImGuiInputTextFlags.ReadOnly );
                ImGui.SameLine();
                if( ImGui.Button( "Edit" + id ) ) {
                    CurrentlyRenaming = true;
                    RenamedTemp = currentText;
                }
            }
        }
    }
}
