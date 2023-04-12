using Dalamud.Interface;
using ImGuiNET;
using System;
using System.Collections.Generic;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat {
    public abstract class UiItemSplitView<T> : AvfxGenericSplitView<T> where T : class, ISelectableUiItem {
        public UiItemSplitView( List<T> items ) : base( items, true, true ) {
            UpdateIdx();
        }

        public virtual void OnSelect( T item ) { }

        public abstract T CreateNewAvfx();

        public abstract void Enable( T item );

        public abstract void Disable( T item );

        protected override void DrawControls( string id ) {
            ImGui.PushFont( UiBuilder.IconFont );
            if( AllowNew ) {
                if( ImGui.Button( $"{( char )FontAwesomeIcon.Plus}{id}" ) ) {
                    CommandManager.Avfx.Add( new UiItemSplitViewAddCommand<T>( this, Items ) );
                }
            }
            if( Selected != null && AllowDelete ) {
                if( AllowNew ) ImGui.SameLine();
                if( UiUtils.RemoveButton( $"{( char )FontAwesomeIcon.Trash}{id}" ) ) {
                    CommandManager.Avfx.Add( new UiItemSplitViewRemoveCommand<T>( this, Items, Selected ) );
                }
            }
            ImGui.PopFont();
        }

        protected override void DrawLeftItem( T item, int idx, string id ) {
            if( ImGui.Selectable( $"{item.GetText()}{id}{idx}", Selected == item ) ) {
                OnSelect( item );
                Selected = item;
            }
        }

        protected override void DrawSelected( string id ) => Selected.Draw( id );

        public void UpdateIdx() { for( var i = 0; i < Items.Count; i++ ) Items[i].SetIdx( i ); }

        public void ClearSelected() { Selected = null; }
    }
}
