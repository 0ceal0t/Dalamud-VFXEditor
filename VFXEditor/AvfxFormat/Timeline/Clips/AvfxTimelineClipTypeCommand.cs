namespace VfxEditor.AvfxFormat {
    public class AvfxTimelineClipTypeCommand : ICommand {
        private readonly AvfxTimelineClipType Item;
        private readonly string State;
        private string PrevState;

        public AvfxTimelineClipTypeCommand( AvfxTimelineClipType item, string state ) {
            Item = item;
            State = state;
        }

        public void Execute() {
            PrevState = Item.Value;
            Item.Value = State;
        }

        public void Redo() {
            Item.Value = State;
        }

        public void Undo() {
            Item.Value = PrevState;
        }
    }
}
