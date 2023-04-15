using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.AvfxFormat {
    public class AvfxDisplaySplitView<T> : AvfxGenericSplitView<T> where T : AvfxItem {
        public AvfxDisplaySplitView( List<T> items ) : base( items, false, false ) {
        }

        protected override void DrawControls( string id ) { }

        protected override void DrawLeftColumn( string id ) {
            var idx = 0;
            foreach( var item in Items.Where( x => x.IsAssigned() ) ) {
                if( item is AvfxOptional assignable ) AvfxBase.AssignedCopyPaste( assignable, assignable.GetDefaultText() );
                if( ImGui.Selectable( $"{item.GetText()}{id}{idx}", Selected == item ) ) {
                    Selected = item;
                }
                idx++;
            }

            // not assigned, can be added
            foreach( var item in Items.Where( x => !x.IsAssigned() ) ) item.Draw( id );
        }

        protected override bool DrawLeftItem( T item, int idx, string id ) => false;

        protected override void DrawSelected( string id ) {
            if( Selected.IsAssigned() ) Selected.Draw( id );
        }
    }
}
