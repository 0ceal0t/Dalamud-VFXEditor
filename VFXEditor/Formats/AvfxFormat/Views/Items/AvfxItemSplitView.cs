using Dalamud.Interface;
using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat {
    public abstract class AvfxItemSplitView<T> : AvfxGenericSplitView<T> where T : class, IIndexUiItem {
        public AvfxItemSplitView( string id, List<T> items ) : base( id, items, true, true ) {
            UpdateIdx();
        }

        public virtual void OnChange() { }

        public abstract T CreateNewAvfx();

        public virtual void OnSelect( T item ) { }

        public virtual void Enable( T item ) { }

        public virtual void Disable( T item ) { }

        protected override void DrawControls() {
            using var font = ImRaii.PushFont( UiBuilder.IconFont );
            if( ShowControls && ImGui.Button( FontAwesomeIcon.Plus.ToIconString() ) ) {
                CommandManager.Add( new AvfxItemSplitViewAddCommand<T>( this, Items ) );
            }

            if( Selected != null && AllowDelete ) {
                if( ShowControls ) ImGui.SameLine();
                if( UiUtils.RemoveButton( FontAwesomeIcon.Trash.ToIconString() ) ) {
                    CommandManager.Add( new AvfxItemSplitViewRemoveCommand<T>( this, Items, Selected ) );
                }
            }
        }

        protected override bool DrawLeftItem( T item, int idx ) {
            using var _ = ImRaii.PushId( idx );

            if( ImGui.Selectable( item.GetText(), Selected == item ) ) {
                OnSelect( item );
                Selected = item;
            }
            return false;
        }

        protected override void DrawSelected() => Selected.Draw();

        public void UpdateIdx() { for( var i = 0; i < Items.Count; i++ ) Items[i].SetIdx( i ); }

        public void ClearSelected() { Selected = null; }
    }
}
