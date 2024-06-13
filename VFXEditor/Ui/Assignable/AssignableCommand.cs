namespace VfxEditor.Ui.Assignable {
    public class AssignableCommand<T> : ICommand {
        public readonly IAssignable Item;
        private readonly bool Assigned;

        public AssignableCommand( IAssignable item ) {
            Item = item;
            Assigned = Item.IsAssigned;
        }

        public void Redo() => Item.IsAssigned = Assigned;

        public void Undo() => Item.IsAssigned = !Assigned;
    }
}
