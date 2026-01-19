using System;
using System.Collections.Generic;

namespace VfxEditor.Data.Command.ListCommands {
    public class ListSetCommand<T> : ICommand {
        protected readonly Action OnChangeAction;
        protected readonly List<T> Items;
        protected readonly List<T> State;
        protected readonly List<T> PrevState;

        public ListSetCommand( List<T> items, IEnumerable<T> state, Action onChangeAction = null ) {
            OnChangeAction = onChangeAction;
            Items = items;
            State = [.. state];
            PrevState = [.. Items];

            Items.Clear();
            Items.AddRange( State );
            OnChangeAction?.Invoke();
        }

        public virtual void Redo() {
            Items.Clear();
            Items.AddRange( State );
            OnChangeAction?.Invoke();
        }

        public virtual void Undo() {
            Items.Clear();
            Items.AddRange( PrevState );
            OnChangeAction?.Invoke();
        }
    }
}
