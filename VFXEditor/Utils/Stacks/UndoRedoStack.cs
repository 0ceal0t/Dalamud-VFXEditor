namespace VfxEditor.Utils.Stacks {
    public class UndoRedoStack<T> {
        private readonly FixedSizeStack<T> UndoStack;
        private readonly FixedSizeStack<T> RedoStack;

        public UndoRedoStack( int capacity ) {
            UndoStack = new FixedSizeStack<T>( capacity );
            RedoStack = new FixedSizeStack<T>( capacity );
        }

        public bool CanUndo => UndoStack.Count > 0;

        public bool CanRedo => RedoStack.Count > 0;

        public void Add( T item ) {
            UndoStack.Push( item );
            RedoStack.Clear();
        }

        public bool Undo( out T item ) {
            item = default;
            if( !CanUndo ) return false;

            item = UndoStack.Pop();
            RedoStack.Push( item );
            return true;
        }

        public bool Redo( out T item ) {
            item = default;
            if( !CanRedo ) return false;

            item = RedoStack.Pop();
            UndoStack.Push( item );
            return true;
        }

        public void Clear() {
            UndoStack.Clear();
            RedoStack.Clear();
        }
    }
}
