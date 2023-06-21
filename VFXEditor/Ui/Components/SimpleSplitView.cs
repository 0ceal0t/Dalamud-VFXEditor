using System;
using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Ui.Components {
    public class SimpleSplitview<T> : GenericSplitView<T> where T : class, IUiItem {
        private readonly Func<T, int, string> GetTextAction;
        private readonly Func<T> NewAction;
        private readonly Func<CommandManager> CommandAction;

        public SimpleSplitview( string id, List<T> items, bool allowReorder, Func<T, int, string> getTextAction, Func<T> newAction, Func<CommandManager> commandAction ) :
            base( id, items, true, allowReorder ) {

            GetTextAction = getTextAction;
            NewAction = newAction;
            CommandAction = commandAction;
        }

        protected override void OnNew() {
            CommandAction.Invoke().Add( new GenericAddCommand<T>( Items, NewAction.Invoke() ) );
        }

        protected override void OnDelete( T item ) {
            CommandAction.Invoke().Add( new GenericRemoveCommand<T>( Items, item ) );
        }

        protected override string GetText( T item, int idx ) => GetTextAction == null ? base.GetText( item, idx ) : GetTextAction.Invoke( item, idx );
    }
}
