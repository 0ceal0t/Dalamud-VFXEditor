using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace VfxEditor.Select.Lists {
    public abstract class SelectListTab<T> {
        protected readonly SelectDialog Dialog;
        protected readonly List<T> Items;
        protected readonly string Name;

        public SelectListTab( SelectDialog dialog, string name, List<T> items ) {
            Dialog = dialog;
            Name = name;
            Items = items;
        }

        protected abstract SelectResult ItemToSelectResult( T item );

        public void Draw( string parentId ) {
            if( Items == null ) return;

            var id = $"{parentId}/{Name}";
            if( !ImGui.BeginTabItem( $"{Name}{id}" ) ) return;

            ImGui.PushStyleVar( ImGuiStyleVar.WindowPadding, new Vector2( 0, 2 ) );
            ImGui.BeginChild( id + "/Child", new Vector2( -1, -1 ), true );

            if( ImGui.BeginTable( $"{id}/Table", 2, ImGuiTableFlags.RowBg ) ) {
                ImGui.TableSetupColumn( $"{id}/Col1", ImGuiTableColumnFlags.WidthFixed, 20 );
                ImGui.TableSetupColumn( $"{id}/Col2", ImGuiTableColumnFlags.WidthStretch );

                var idx = 0;
                foreach( var item in Items ) {
                    var result = ItemToSelectResult( item );
                    if( result.Type == SelectResultType.Local && !Dialog.IsSource ) continue;

                    ImGui.TableNextRow();

                    ImGui.TableNextColumn();
                    if( DrawFavorite( item, result ) ) break;

                    ImGui.TableNextColumn();
                    if( DrawSelect( item, result, $"{id}{idx}" ) ) break;

                    idx++;
                }

                ImGui.EndTable();
            }

            ImGui.EndChild();
            ImGui.PopStyleVar();

            ImGui.EndTabItem();
        }

        protected virtual bool DrawFavorite( T item, SelectResult result ) {
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() + 4 );
            if( Dialog.DrawFavorite( result ) ) return true;
            return false;
        }

        protected virtual bool DrawSelect( T item, SelectResult result, string id ) {
            ImGui.Selectable( $"{result.DisplayString}{id}", false, ImGuiSelectableFlags.SpanAllColumns );

            if( ImGui.IsMouseDoubleClicked( ImGuiMouseButton.Left ) && ImGui.IsItemHovered() ) {
                Dialog.Invoke( result );
                return true;
            }
            return false;
        }
    }
}
