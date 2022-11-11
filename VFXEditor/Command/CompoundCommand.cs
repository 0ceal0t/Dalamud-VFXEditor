using System.Collections.Generic;

namespace VfxEditor {
    public class CompoundCommand : ICommand {
        private readonly List<ICommand> Commands = new();
        
        public CompoundCommand() {
        }

        public void Add( ICommand command ) => Commands.Add( command );

        public void Execute() => Commands.ForEach( x => x.Execute() );

        public void Redo() => Commands.ForEach( x => x.Redo() );

        public void Undo() => Commands.ForEach( x => x.Undo() );
    }
}