using System.Collections.Generic;
using System.Linq;

namespace VfxEditor {
    public class CompoundCommand : ICommand {
        private readonly List<ICommand> Commands = new();
        private readonly bool ReverseRedo;
        private readonly bool ReverseUndo;

        public CompoundCommand( bool reverseRedo, bool reverseUndo ) {
            ReverseRedo = reverseRedo;
            ReverseUndo = reverseUndo;
        }

        public void Add( ICommand command ) => Commands.Add( command );

        public void Clear() => Commands.Clear();

        public void Execute() {
            for( var i = 0; i < Commands.Count; i++ ) Commands[ReverseRedo ? Commands.Count - 1 - i : i].Execute();
        }

        public void Redo() {
            for( var i = 0; i < Commands.Count; i++ ) Commands[ReverseRedo ? Commands.Count - 1 - i : i].Redo();
        }

        public void Undo() {
            for( var i = 0; i < Commands.Count; i++ ) Commands[ReverseUndo ? Commands.Count - 1 - i : i].Undo();
        }
    }
}