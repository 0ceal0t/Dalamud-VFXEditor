using AVFXLib.AVFX;
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
    public abstract class UIDropdownView<T> : UIBase, UINodeView<T> where T : UINode {
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
        public abstract T OnImport( AVFXNode node, bool has_dependencies = false );

        public override void Draw( string parentId = "" ) {
            ImGui.PushStyleColor( ImGuiCol.ChildBg, new Vector4( 0.18f, 0.18f, 0.22f, 0.4f ) );
            ImGui.SetCursorPos( ImGui.GetCursorPos() - new Vector2( 5, 5 ) );
            ImGui.BeginChild( "Child" + Id, new Vector2( ImGui.GetWindowWidth() - 0, 33 ) );
            ImGui.SetCursorPos( ImGui.GetCursorPos() + new Vector2( 5, 5 ) );
            ImGui.PopStyleColor();
            ViewSelect();

            if(AllowNew ) ImGui.SameLine();
            UINodeView<T>.DrawControls(this, Main, Selected, Group, AllowNew, AllowDelete, Id);

            ImGui.Separator();
            ImGui.EndChild();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( Selected != null ) {
                Selected.DrawBody( Id );
            }
        }

        public void ViewSelect() {
            var selectedString = (Selected != null) ? Selected.GetText() : defaultText;
            if( ImGui.BeginCombo( "Select" + Id, selectedString ) ) {
                foreach(var item in Group.Items ) {
                    if( ImGui.Selectable( item.GetText() + Id, Selected == item ) ) {
                        Selected = item;
                        OnSelect( item );
                    }
                }
                ImGui.EndCombo();
            }
        }

        public void ControlDelete() {
            Selected = null;
        }

        public void ControlCreate() {
            Main.ImportData( DefaultPath );
        }
    }
}
