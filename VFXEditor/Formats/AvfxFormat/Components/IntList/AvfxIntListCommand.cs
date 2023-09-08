namespace VfxEditor.AvfxFormat {
    public class AvfxIntListCommand : ICommand {
        private readonly AvfxIntList Item;
        private readonly int State;
        private int PrevState;

        public AvfxIntListCommand( AvfxIntList item, int state ) {
            Item = item;
            State = state;
        }

        public void Execute() {
            PrevState = Item.GetValue()[0];
            Item.GetValue()[0] = State;
        }

        public void Redo() {
            Item.GetValue()[0] = State;
        }

        public void Undo() {
            Item.GetValue()[0] = PrevState;
        }
    }
}
