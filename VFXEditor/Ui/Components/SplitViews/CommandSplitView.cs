using System;
using System.Collections.Generic;
using VfxEditor.Data.Command.ListCommands;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Ui.Components.SplitViews {
    public class CommandSplitView<T> : UiSplitView<T> where T : class, IUiItem {
        private readonly Func<T, int, string> GetTextAction;
        private readonly Func<T> NewAction;
        private readonly Action<T, bool> OnChangeAction;

        public CommandSplitView( string id, List<T> items, bool allowReorder, Func<T, int, string> getTextAction, Func<T> newAction, Action<T, bool> onChangeAction = null ) : base( id, items, allowReorder ) {
            GetTextAction = getTextAction;
            NewAction = newAction;
            OnChangeAction = onChangeAction;
        }

        protected virtual void OnNew() => CommandManager.Add( new ListAddCommand<T>( Items, NewAction.Invoke(), OnChangeAction ) );

        protected virtual void OnDelete( T item ) => CommandManager.Add( new ListRemoveCommand<T>( Items, item, OnChangeAction ) );

        protected override void DrawControls() {
            if( NewAction == null ) return;
            DrawNewDeleteControls( OnNew, OnDelete );
        }

        protected override bool RecordReorder() => true;

        protected override string GetText( T item, int idx ) => GetTextAction == null ? base.GetText( item, idx ) : GetTextAction.Invoke( item, idx );

        public void SetSelected( T item ) => Selected = item;
    }
}
