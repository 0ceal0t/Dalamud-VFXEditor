namespace VfxEditor.AvfxFormat {
    public class AvfxIntListCommand : ICommand {
        private readonly AvfxIntList Item;
        private readonly int State;
        private readonly int PrevState;

        public AvfxIntListCommand( AvfxIntList item, int state ) {
            Item = item;
            State = state;
            PrevState = Item.GetItems()[0];

            Item.GetItems()[0] = State;
        }
        public void Redo() {
            Item.GetItems()[0] = State;
        }

        public void Undo() {
            Item.GetItems()[0] = PrevState;
        }
    }
}
