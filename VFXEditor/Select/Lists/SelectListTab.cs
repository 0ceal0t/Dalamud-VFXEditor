using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;
using VfxEditor.Select.Base;

namespace VfxEditor.Select.Lists {
    public class SelectListTab : SelectTabBase {
        protected readonly List<SelectResult> Items;

        public SelectListTab( SelectDialog dialog, string name, List<SelectResult> items ) : base( dialog, name ) {
            Items = items;
        }

        protected override int NumColumns() => 3;

        protected override void DrawTable() {
            ImGui.TableSetupColumn( "##Favorite", ImGuiTableColumnFlags.WidthFixed, 25 );
            ImGui.TableSetupColumn( "##Type" );
            ImGui.TableSetupColumn( "##Name", ImGuiTableColumnFlags.WidthStretch );

            if( Items == null ) return;

            var idx = 0;
            foreach( var item in Items ) {
                if( item.Type == SelectResultType.Local && !Dialog.ShowLocal ) continue;
                if( !( string.IsNullOrEmpty( SearchInput ) ||
                    item.Path.Contains( SearchInput, System.StringComparison.CurrentCultureIgnoreCase ) ||
                    ( string.IsNullOrEmpty( item.Name ) ? item.DisplayString : item.Name ).Contains( SearchInput, System.StringComparison.CurrentCultureIgnoreCase )
                ) ) continue;

                ImGui.TableNextRow();
                if( DrawRow( item, idx ) ) break;

                idx++;
            }
        }

        protected bool DrawRow( SelectResult item, int idx ) {
            using var _ = ImRaii.PushId( idx );

            ImGui.TableNextColumn();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() + 6 );
            if( Dialog.DrawFavorite( item ) ) return true;

            ImGui.TableNextColumn();
            ImGui.TextDisabled( "[" + item.Type.ToString().ToUpperInvariant().Replace( "GAME", "" ) + "]" );

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