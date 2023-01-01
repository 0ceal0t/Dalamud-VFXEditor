using Dalamud.Interface;
using ImGuiNET;
using System;
using System.Collections.Generic;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat {
    public abstract class UiItemSplitView<T> : UiGenericSplitView where T : class, IUiSelectableItem {
        public readonly List<T> Items;
        private T Selected = null;

        public UiItemSplitView( List<T> items ) : base( true, true ) {
            Items = items;
            UpdateIdx();
        }

        public virtual void OnSelect( T item ) { }

        public abstract T CreateNewAvfx();
        public abstract void Enable( T item );
        public abstract void Disable( T item );

        public override void DrawControls( string parentId ) {
            ImGui.PushFont( UiBuilder.IconFont );
            if( AllowNew ) {
                if( ImGui.Button( $"{( char )FontAwesomeIcon.Plus}" + parentId ) ) {
                    CommandManager.Avfx.Add( new UiItemSplitViewAddCommand<T>( this, Items ) );
                }
            }
            if( Selected != null && AllowDelete ) {
                if( AllowNew ) ImGui.SameLine();
                if( UiUtils.RemoveButton( $"{( char )FontAwesomeIcon.Trash}" + parentId ) ) {
                    CommandManager.Avfx.Add( new UiItemSplitViewRemoveCommand<T>( this, Items, Selected ) );
                }
            }
            ImGui.PopFont();
        }

        public override void DrawLeftCol( string parentId ) {
            for( var idx = 0; idx < Items.Count; idx++ ) {
                var item = Items[idx];
                if( ImGui.Selectable( $"{item.GetText()}{parentId}{idx}", Selected == item ) ) {
                    OnSelect( item );
                    Selected = item;
                }
            }
        }

        public override void DrawRightCol( string parentId ) => Selected?.Draw( parentId );

        public void UpdateIdx() { for( var i = 0; i < Items.Count; i++ ) Items[i].SetIdx( i ); }

        public void ClearSelected() { Selected = null; }
    }
}
