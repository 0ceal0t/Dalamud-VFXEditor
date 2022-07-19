using Dalamud.Logging;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.Helper;

namespace VFXEditor.NodeLibrary {
    [Serializable]
    public class VFXNodeLibraryItem {
        public string Name;
        public string Id;
        public string Path;
        public string Description;
        public Vector4 Color;

        [NonSerialized]
        private bool Editing = false;

        public VFXNodeLibraryItem( string name, string id, string path, string description, Vector4 color ) {
            Name = name;
            Id = id;
            Path = path;
            Description = description;
            Color = color;
        }

        public bool Draw( VFXNodeLibrary library ) {
            var id = $"##NodeLibrary{Id}";
            var uniqueId = $"###NodeLibrary{Id}";
            var listModified = false;
            // return true if list modified
            ImGui.PushStyleColor( ImGuiCol.Header, Color );
            if( ImGui.CollapsingHeader( $"{Name}{uniqueId}" ) ) {
                DragDrop( library, ref listModified );
                ImGui.Indent();

                if( ImGui.Button( $"Import{id}" ) ) {
                    Plugin.AvfxManager.Import( Path );
                }

                ImGui.SameLine();
                if( ImGui.Button( Editing ? $"Save{id}" : $"Edit{id}" ) ) {
                    Editing = !Editing;
                    if (!Editing ) { // done editing
                        VFXNodeLibrary.Save();
                        listModified = true;
                    }
                }
                ImGui.SameLine();
                if( UIHelper.RemoveButton( $"Delete{id}" ) ) {
                    Delete();
                    library.Delete( this );
                    listModified = true;
                }

                if (!Editing) {
                    if( string.IsNullOrEmpty( Description ) ) {
                        ImGui.TextDisabled( "No description" );
                    }
                    else {
                        ImGui.TextWrapped( Description );
                    }
                }
                else {
                    var preX = ImGui.GetCursorPosX();
                    ImGui.InputText( $"Name{id}", ref Name, 255 );
                    var w = ImGui.GetCursorPosX() - preX;
                    ImGui.ColorEdit4( $"Color{id}", ref Color, ImGuiColorEditFlags.DisplayHex | ImGuiColorEditFlags.AlphaPreviewHalf | ImGuiColorEditFlags.AlphaBar );
                    ImGui.InputTextMultiline( $"Description{id}", ref Description, 1000, new Vector2( w, 100 ) );
                }

                ImGui.Unindent();
            }
            else {
                DragDrop( library, ref listModified );
            }
            ImGui.PopStyleColor();

            return listModified;
        }

        private void DragDrop( VFXNodeLibrary library, ref bool listModified ) {
            if( ImGui.BeginDragDropSource( ImGuiDragDropFlags.None ) ) {
                library.StartDragging( this );
                ImGui.EndDragDropSource();
            }
            if( ImGui.BeginDragDropTarget() ) {
                if( library.StopDragging( this ) ) listModified = true;
                ImGui.EndDragDropTarget();
            }
        }

        public bool Matches( string input ) {
            if( string.IsNullOrEmpty( input ) ) return true;
            if( Name.ToLower().Contains( input.ToLower() ) ) return true;
            return false;
        }

        public void Delete() {
            if( File.Exists( Path ) ) {
                File.Delete( Path );
            }
        }
    }
}
