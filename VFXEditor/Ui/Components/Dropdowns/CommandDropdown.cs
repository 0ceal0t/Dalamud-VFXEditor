using System;
using System.Collections.Generic;
using VfxEditor.Data.Command.ListCommands;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Ui.Components {
    public class CommandDropdown<T> : Dropdown<T> where T : class, IUiItem {
        private readonly Func<T, int, string> GetTextAction;
        private readonly Func<T> NewAction;
        private readonly Action<T, bool> OnChangeAction;

        public CommandDropdown( string id, List<T> items, Func<T, int, string> getTextAction, Func<T> newAction, Action<T, bool> onChangeAction = null ) :
            base( id, items, true, true ) {

            GetTextAction = getTextAction;
            NewAction = newAction;
            OnChangeAction = onChangeAction;
        }

        protected override void DrawSelected() => Selected.Draw();

        protected override string GetText( T item, int idx ) => GetTextAction == null ? $"{Id} {idx}" : GetTextAction.Invoke( item, idx );

        protected override void OnNew() => CommandManager.Add( new ListAddCommand<T>( Items, NewAction.Invoke(), OnChangeAction ) );

        protected override void OnDelete( T item ) => CommandManager.Add( new ListRemoveCommand<T>( Items, item, OnChangeAction ) );
    }
}
