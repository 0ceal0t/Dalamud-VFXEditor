using System;
using System.Collections.Generic;
using VfxEditor.Data.Command.ListCommands;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Ui.Components {
    public class CommandDropdown<T> : UiDropdown<T> where T : class, IUiItem {
        protected readonly Func<T, int, string> GetTextAction;
        protected readonly Func<T> NewAction;
        protected readonly Action<T, bool> OnChangeAction;

        public CommandDropdown( string id, List<T> items, Func<T, int, string> getTextAction, Func<T> newAction, Action<T, bool> onChangeAction = null ) : base( id, items ) {
            GetTextAction = getTextAction;
            NewAction = newAction;
            OnChangeAction = onChangeAction;
        }

        protected override void DrawSelected() => Selected.Draw();

        public override string GetText( T item, int idx ) => GetTextAction == null ? $"{Id} {idx}" : GetTextAction.Invoke( item, idx );

        protected override void DrawControls() {
            if( NewAction == null ) return;
            DrawNewDeleteControls( OnNew, OnDelete );
        }

        protected virtual void OnNew() => CommandManager.Add( new ListAddCommand<T>( Items, NewAction.Invoke(), OnChangeAction ) );

        protected virtual void OnDelete( T item ) => CommandManager.Add( new ListRemoveCommand<T>( Items, item, OnChangeAction ) );
    }
}
