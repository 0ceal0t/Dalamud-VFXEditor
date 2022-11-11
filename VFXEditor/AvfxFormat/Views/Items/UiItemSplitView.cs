using Dalamud.Interface;
using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat.Vfx {
    public abstract class UiItemSplitView<T> : UiGenericSplitView where T : UiItem {
        public readonly List<T> Items;
        private T Selected = null;

        public UiItemSplitView( List<T> items ) : base( true, true ) {
            Items = items;
            UpdateIdx();
        }

        public virtual void OnSelect( T item ) { }

        public abstract T CreateNewAvfx();
        public abstract void RemoveFromAvfx( T item );
        public abstract void AddToAvfx( T item, int idx );

        public override void DrawControls( string parentId ) {
            ImGui.PushFont( UiBuilder.IconFont );
            if( AllowNew ) {
                if( ImGui.Button( $"{( char )FontAwesomeIcon.Plus}" + parentId ) ) {
                    CommandManager.Avfx.Add( new UiItemSplitViewAddCommand<T>( this, Items ) );
                }
            }
            if( Selected != null && AllowDelete ) {
                ImGui.SameLine();
                if( UiUtils.RemoveButton( $"{( char )FontAwesomeIcon.Trash}" + parentId ) ) {
                    CommandManager.Avfx.Add( new UiItemSplitViewRemoveCommand<T>( this, Items, Selected ) );
                }
            }
            ImGui.PopFont();
        }

        public override void DrawLeftCol( string parentId ) {
            foreach( var item in Items ) {
                if( ImGui.Selectable( $"{item.GetText()}{parentId}", Selected == item ) ) {
                    OnSelect( item );
                    Selected = item;
                }
            }
        }

        public override void DrawRightCol( string parentId ) => Selected?.DrawInline( parentId );

        public void UpdateIdx() { for( var i = 0; i < Items.Count; i++ ) Items[i].Idx = i; }

        public void ClearSelected() { Selected = null; }
    }
}
