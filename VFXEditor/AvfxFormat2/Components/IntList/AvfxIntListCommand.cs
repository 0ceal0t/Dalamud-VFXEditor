namespace VfxEditor.AvfxFormat2 {
    public class AvfxIntListCommand : ICommand {
        private readonly AvfxIntList Item;
        private readonly int State;
        private readonly int PrevState;

        public AvfxIntListCommand( AvfxIntList item, int state ) {
            Item = item;
            State = state;
            PrevState = item.GetValue()[0];
        }

        public void Execute() {
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
