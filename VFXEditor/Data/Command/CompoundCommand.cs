using System;
using System.Collections.Generic;
using System.Linq;

namespace VfxEditor {
    public class CompoundCommand : ICommand {
        private readonly Action OnChangeAction;
        private readonly List<ICommand> Commands = [];

        public CompoundCommand( IEnumerable<ICommand> commands, Action onChangeAction = null ) {
            Commands.AddRange( commands );
            OnChangeAction = onChangeAction;

            OnChangeAction?.Invoke();
        }

        public virtual void Redo() {
            Commands.ForEach( x => x.Redo() );
            OnChangeAction?.Invoke();
        }

        public virtual void Undo() {
            Commands.AsEnumerable().Reverse().ToList().ForEach( x => x.Undo() );
            OnChangeAction?.Invoke();
        }
    }
}