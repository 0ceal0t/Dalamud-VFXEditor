using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.Numerics;

namespace VfxEditor.Select.Lists {
    public class SelectListTab {
        protected readonly SelectDialog Dialog;
        protected readonly List<SelectResult> Items;
        protected readonly string Name;

        protected Vector2 DefaultWindowPadding = new();
        protected string SearchInput = "";

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

            DefaultWindowPadding = ImGui.GetStyle().WindowPadding;

            ImGui.InputTextWithHint( "##Search", "Search", ref SearchInput, 255 );

            using var style = ImRaii.PushStyle( ImGuiStyleVar.CellPadding, new Vector2( 4, 3 ) );
            using var padding = ImRaii.PushStyle( ImGuiStyleVar.WindowPadding, new Vector2( 0, 0 ) );
            using var child = ImRaii.Child( "Child", new Vector2( -1, -1 ), true );
            using var table = ImRaii.Table( "Table", 3, ImGuiTableFlags.RowBg | ImGuiTableFlags.SizingFixedFit );
            if( !table ) return;
            padding.Dispose();

            ImGui.TableSetupColumn( "##Column1", ImGuiTableColumnFlags.WidthFixed, 25 );
            ImGui.TableSetupColumn( "##Column2" );
            ImGui.TableSetupColumn( "##Column3", ImGuiTableColumnFlags.WidthStretch );

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
