using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.Ui.Components.SplitViews {
    public class CommandSplitView<T> : UiSplitView<T> where T : class, IUiItem {
        private readonly Func<T, int, string> GetTextAction;
        private readonly Func<T> NewAction;
        private readonly Func<CommandManager> CommandAction;
        private readonly Action<T> OnChangeAction;

        public CommandSplitView( string id, List<T> items, bool allowReorder, Func<T, int, string> getTextAction, Func<T> newAction, Func<CommandManager> commandAction, Action<T> onChangeAction = null ) :
            base( id, items, true, allowReorder ) {

            GetTextAction = getTextAction;
            NewAction = newAction;
            CommandAction = commandAction;
            OnChangeAction = onChangeAction;
        }

        protected override bool DrawLeftItem( T item, int idx ) {
            using( var _ = ImRaii.PushId( idx ) ) {
                if( ImGui.Selectable( GetText( item, idx ), item == Selected ) ) Selected = item;
            }

            if( AllowReorder && UiUtils.DrawDragDrop( Items, item, GetText( item, Items.IndexOf( item ) ), ref DraggingItem, $"{Id}-SPLIT", CommandAction.Invoke() ) ) return true;
            return false;
        }

        protected override void OnNew() {
            CommandAction.Invoke().Add( new GenericAddCommand<T>( Items, NewAction.Invoke(), OnChangeAction ) );
        }

        protected override void OnDelete( T item ) {
            CommandAction.Invoke().Add( new GenericRemoveCommand<T>( Items, item, OnChangeAction ) );
        }

        protected override string GetText( T item, int idx ) => GetTextAction == null ? base.GetText( item, idx ) : GetTextAction.Invoke( item, idx );

        public T GetSelected() => Selected;

        public void SetSelected( T item ) => Selected = item;
    }
}
