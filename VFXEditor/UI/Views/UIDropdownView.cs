using AVFXLib.Models;
using Dalamud.Interface;
using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VFXEditor.UI.VFX
{
    public abstract class UIDropdownView<T> : UIBase where T : UINode {
        public string Id;
        public string defaultText;
        public UIMain Main;
        public AVFXBase AVFX;
        public UINodeGroup<T> Group;
        public T Selected = null;
        public string DefaultPath;

        public bool AllowNew;
        public bool AllowDelete;

        public UIDropdownView( UIMain main, AVFXBase avfx, string _id, string _defaultText, bool allowNew = true, bool allowDelete = true, string defaultPath = "" ) {
            Main = main;
            AVFX = avfx;
            Id = _id;
            defaultText = _defaultText;
            AllowNew = allowNew;
            AllowDelete = allowDelete;
            DefaultPath = Path.Combine( Plugin.TemplateLocation, "Files", defaultPath );
        }

        public abstract void OnDelete( T item );
        public abstract byte[] OnExport( T item );
        public virtual void OnSelect( T item ) { }
        public abstract T OnImport( AVFXLib.AVFX.AVFXNode node, bool has_dependencies = false );

        public override void Draw( string parentId = "" ) {
            ImGui.PushStyleColor( ImGuiCol.ChildBg, new Vector4( 0.18f, 0.18f, 0.22f, 0.4f ) );
            ImGui.SetCursorPos( ImGui.GetCursorPos() - new Vector2( 5, 5 ) );
            ImGui.BeginChild( "Child" + Id, new Vector2( ImGui.GetWindowWidth() - 0, 33 ) );
            ImGui.SetCursorPos( ImGui.GetCursorPos() + new Vector2( 5, 5 ) );
            ImGui.PopStyleColor();
            ViewSelect();

            ImGui.PushFont( UiBuilder.IconFont );
            if( AllowNew ) {
                ImGui.SameLine();
                if( ImGui.Button( $"{( char )FontAwesomeIcon.Plus}" + Id ) ) {
                    ImGui.OpenPopup( "New_Popup" + Id );
                }
            }
            if( Selected != null && AllowDelete ) {
                ImGui.SameLine();
                if( ImGui.Button( $"{( char )FontAwesomeIcon.Save}" + Id ) ) {
                    ImGui.OpenPopup( "Save_Popup" + Id );
                }
                ImGui.SameLine();
                if( UIUtils.RemoveButton( $"{( char )FontAwesomeIcon.Trash}" + Id ) ) {
                    Group.Remove( Selected );
                    Selected.DeleteNode();
                    OnDelete( Selected );
                    Selected = null;
                }
            }
            ImGui.PopFont();
            // ===== NEW =====
            if( ImGui.BeginPopup( "New_Popup" + Id ) ) {
                if( ImGui.Selectable( "Create" + Id ) ) {
                    Main.ImportData( DefaultPath );
                }
                if( ImGui.Selectable( "Import" + Id ) ) {
                    Main.ImportDialog();
                }
                ImGui.EndPopup();
            }
            // ==== SAVE =====
            if( ImGui.BeginPopup( "Save_Popup" + Id ) ) {
                if( ImGui.Selectable( "Simple" + Id ) ) {
                    Main.ExportDialog( Selected );
                }
                if( ImGui.Selectable("Advanced Export" + Id ) ) {
                    Main.ExportMultiple( Selected );
                }
                ImGui.EndPopup();
            }

            ImGui.Separator();
            ImGui.EndChild();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            // ====================
            if( Selected != null ) {
                Selected.DrawBody( Id );
            }
        }
        // ========================
        public void ViewSelect() {
            var selectedString = (Selected != null) ? Selected.GetText() : defaultText;
            if( ImGui.BeginCombo( "Select" + Id, selectedString ) ) {
                for( int idx = 0; idx < Group.Items.Count; idx++ ) {
                    var item = Group.Items[idx];
                    if( ImGui.Selectable( item.GetText() + Id, Selected == item ) ) {
                        Selected = item;
                        OnSelect( item );
                    }
                }
                ImGui.EndCombo();
            }
        }
    }
}
