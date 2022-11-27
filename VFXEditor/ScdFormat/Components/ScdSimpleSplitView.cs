using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;

namespace VfxEditor.ScdFormat {
    public class ScdSimpleSplitView<T> where T : class, IScdSimpleUiBase {
        private readonly List<T> Items;
        private T Selected = null;

        public ScdSimpleSplitView( List<T> items ) {
            Items = items;
        }

        public void Draw( string id ) {
            ImGui.BeginChild( $"{id}/Child", new Vector2( -1, -1 ), false );
            ImGui.Columns( 2, $"{id}/ChildCols", true );

            // Left column
            ImGui.BeginChild( $"{id}/Left" );

            var selectedIndex = Selected == null ? -1 : Items.IndexOf( Selected );
            for( var i = 0; i < Items.Count; i++ ) {
                if( ImGui.Selectable( $"Entry {i}{id}{i}", Items[i] == Selected ) ) {
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
    }
}
