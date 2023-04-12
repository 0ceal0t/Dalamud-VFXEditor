using ImGuiNET;
using System;
using System.Collections.Generic;

namespace VfxEditor.Ui.Components {
    public abstract class SplitView<T> where T : class {
        protected bool AllowNew;
        protected bool DrawOnce = false;

        protected readonly List<T> Items;
        protected T Selected = null;

        public SplitView( List<T> items, bool allowNew ) {
            Items = items;
            AllowNew = allowNew;
        }

        protected virtual void DrawControls( string id ) { }

        protected virtual void DrawLeftColumn( string id ) {
            if( Selected != null && !Items.Contains( Selected ) ) Selected = null;
            for( var idx = 0; idx < Items.Count; idx++ ) {
                DrawLeftItem( Items[idx], idx, id );
            }
        }

        protected virtual void DrawRightColumn( string id ) {
            if( Selected == null ) ImGui.TextDisabled( "Select an item..." );
            else DrawSelected( id );
        }

        protected abstract void DrawLeftItem( T item, int idx, string id );

        protected abstract void DrawSelected( string id );

        public void Draw( string id ) {
            ImGui.Columns( 2, id + "/Cols", true );
            if( AllowNew ) DrawControls( id );

            ImGui.BeginChild( id + "/Left" );
            DrawLeftColumn( id );
            ImGui.EndChild();

            if( !DrawOnce ) {
                ImGui.SetColumnWidth( 0, 200 );
                DrawOnce = true;
            }
            ImGui.NextColumn();

            ImGui.BeginChild( id + "/Right" );
            DrawRightColumn( id );
            ImGui.EndChild();

            ImGui.Columns( 1 );
        }
    }
}
