using Dalamud.Interface;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.Utils;

namespace VfxEditor.ScdFormat {
    public class ScdSimpleSplitView<T> where T : class, IScdSimpleUiBase {
        protected readonly List<T> Items;
        protected T Selected = null;
        private readonly string ItemName;
        private readonly bool AllowNew;

        public ScdSimpleSplitView( string itemName, List<T> items, bool allowNew = false ) {
            Items = items;
            ItemName = itemName;
            AllowNew = allowNew;
        }

        public void Draw( string id ) {
            ImGui.BeginChild( $"{id}/Child", new Vector2( -1, -1 ), false );
            ImGui.Columns( 2, $"{id}/ChildCols", true );

            // Left column
            ImGui.BeginChild( $"{id}/Left" );
            if( AllowNew ) {
                ImGui.PushFont( UiBuilder.IconFont );
                if( ImGui.Button( $"{( char )FontAwesomeIcon.Plus}{id}" ) ) OnNew();
                if( Selected != null ) {
                    ImGui.SameLine();
                    if( UiUtils.RemoveButton( $"{( char )FontAwesomeIcon.Trash}{id}" ) ) {
                        OnDelete( Selected );
                        Selected = null;
                    }
                }
                ImGui.PopFont();
            }

            var selectedIndex = Selected == null ? -1 : Items.IndexOf( Selected );
            for( var i = 0; i < Items.Count; i++ ) {
                if( ImGui.Selectable( $"{ItemName} {i}{id}{i}", Items[i] == Selected ) ) {
                    Selected = Items[i];
                    selectedIndex = i;
                }
            }
            if( selectedIndex == -1 ) Selected = null;

            ImGui.EndChild();
            ImGui.SetColumnWidth( 0, 150 );

            // Right column

            ImGui.NextColumn();
            ImGui.BeginChild( $"{id}/Right" );

            if( Selected != null ) Selected.Draw( $"{id}{selectedIndex}" );
            else ImGui.Text( "Select an item" );

            ImGui.EndChild();

            ImGui.Columns( 1 );
            ImGui.EndChild();
        }

        protected virtual void OnNew() { }

        protected virtual void OnDelete( T item ) { }
    }
}
