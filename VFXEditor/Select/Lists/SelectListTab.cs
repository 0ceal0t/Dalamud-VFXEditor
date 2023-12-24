using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.Numerics;

namespace VfxEditor.Select.Lists {
    public class SelectListTab {
        protected readonly SelectDialog Dialog;
        protected readonly List<SelectResult> Items;
        protected readonly string Name;

        public SelectListTab( SelectDialog dialog, string name, List<SelectResult> items ) {
            Dialog = dialog;
            Name = name;
            Items = items;
        }

        public void Draw() {
            if( Items == null ) return;

            using var _ = ImRaii.PushId( Name );

            using var tabItem = ImRaii.TabItem( Name );
            if( !tabItem ) return;

            using var style = ImRaii.PushStyle( ImGuiStyleVar.WindowPadding, new Vector2( 0, 0 ) );
            using var child = ImRaii.Child( "Child", new Vector2( -1, -1 ), true );

            if( ImGui.BeginTable( "Table", 3, ImGuiTableFlags.RowBg | ImGuiTableFlags.SizingFixedFit ) ) {
                ImGui.TableSetupColumn( "##Column1", ImGuiTableColumnFlags.WidthFixed, 25 );
                ImGui.TableSetupColumn( "##Column2" );
                ImGui.TableSetupColumn( "##Column3", ImGuiTableColumnFlags.WidthStretch );

                var idx = 0;
                foreach( var item in Items ) {
                    if( item.Type == SelectResultType.Local && !Dialog.ShowLocal ) continue;

                    ImGui.TableNextRow();
                    if( DrawRow( item, idx ) ) break;

                    idx++;
                }

                ImGui.EndTable();
            }
        }

        protected bool DrawRow( SelectResult item, int idx ) {
            using var _ = ImRaii.PushId( idx );

            ImGui.TableNextColumn();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() + 6 );
            if( Dialog.DrawFavorite( item ) ) return true;

            ImGui.TableNextColumn();
            ImGui.TextDisabled( "[" + item.Type.ToString().ToUpper().Replace( "GAME", "" ) + "]" );

            ImGui.TableNextColumn();
            ImGui.Selectable( string.IsNullOrEmpty( item.Name ) ? item.DisplayString.Split( "]" )[^1] : item.Name, false, ImGuiSelectableFlags.SpanAllColumns );

            if( PostRow( item, idx ) ) return true;

            return false;
        }

        protected virtual bool PostRow( SelectResult item, int idx ) {
            if( ImGui.IsMouseDoubleClicked( ImGuiMouseButton.Left ) && ImGui.IsItemHovered() ) {
                Dialog.Invoke( item );
                return true;
            }

            return false;
        }
    }
}
