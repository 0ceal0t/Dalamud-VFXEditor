using System;
using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Ui.Interfaces;

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

        protected override void OnNew() {
            CommandAction.Invoke().Add( new GenericAddCommand<T>( Items, NewAction.Invoke(), OnChangeAction ) );
        }

        protected override void OnDelete( T item ) {
            CommandAction.Invoke().Add( new GenericRemoveCommand<T>( Items, item, OnChangeAction ) );
        }

        protected override string GetText( T item, int idx ) => GetTextAction == null ? base.GetText( item, idx ) : GetTextAction.Invoke( item, idx );
    }
}
