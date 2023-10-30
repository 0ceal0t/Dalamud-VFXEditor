using System.Collections.Generic;
using System.Linq;

namespace VfxEditor {
    public class CompoundCommand : ICommand {
        private readonly List<ICommand> Commands = new();

        public CompoundCommand() { }

        public CompoundCommand( IEnumerable<ICommand> commands ) {
            Commands.AddRange( commands );
        }

        public void Add( ICommand command ) => Commands.Add( command );

        public void Clear() => Commands.Clear();

        public virtual void Execute() => Commands.ForEach( x => x.Execute() );

        public virtual void Redo() => Commands.ForEach( x => x.Redo() );

        public virtual void Undo() => Commands.AsEnumerable().Reverse().ToList().ForEach( x => x.Undo() );
    }
}