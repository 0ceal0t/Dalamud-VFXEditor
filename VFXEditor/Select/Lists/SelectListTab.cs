using ImGuiNET;
using System;
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

        public void Draw( string parentId ) {
            if( Items == null ) return;

            var id = $"{parentId}/{Name}";
            if( !ImGui.BeginTabItem( $"{Name}{id}" ) ) return;

            ImGui.PushStyleVar( ImGuiStyleVar.WindowPadding, new Vector2( 0, 0 ) );
            ImGui.BeginChild( id + "/Child", new Vector2( -1, -1 ), true );

            if( ImGui.BeginTable( $"{id}/Table", 2, ImGuiTableFlags.RowBg ) ) {
                ImGui.TableSetupColumn( $"{id}/Col1", ImGuiTableColumnFlags.WidthFixed, 20 );
                ImGui.TableSetupColumn( $"{id}/Col2", ImGuiTableColumnFlags.WidthStretch );

                var idx = 0;
                foreach( var item in Items ) {
                    if( item.Type == SelectResultType.Local && !Dialog.IsSource ) continue;

                    ImGui.TableNextRow();
                    if( DrawRow( item, id, idx ) ) break;

                    idx++;
                }

                ImGui.EndTable();
            }

            ImGui.EndChild();
            ImGui.PopStyleVar( 1 );

            ImGui.EndTabItem();
        }

        protected bool DrawRow( SelectResult item, string id,  int idx ) {
            ImGui.TableNextColumn();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() + 4 );
            if( Dialog.DrawFavorite( item ) ) return true;

            ImGui.TableNextColumn();
            ImGui.Selectable( $"{item.DisplayString}{id}{idx}", false, ImGuiSelectableFlags.SpanAllColumns );

            if( PostRow( item, id, idx ) ) return true;

            return false;
        }

        protected virtual bool PostRow( SelectResult item, string id, int idx ) {
            if( ImGui.IsMouseDoubleClicked( ImGuiMouseButton.Left ) && ImGui.IsItemHovered() ) {
                Dialog.Invoke( item );
                return true;
            }

            return false;
        }
    }
}
