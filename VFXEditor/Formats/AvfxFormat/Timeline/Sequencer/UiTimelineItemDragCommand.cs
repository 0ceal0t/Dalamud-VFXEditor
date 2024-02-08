namespace VfxEditor.AvfxFormat {
    public class UiTimelineItemDragCommand : ICommand {
        private readonly AvfxTimelineItem Item;
        private readonly (int, int) State;
        private readonly (int, int) PrevState;

        public UiTimelineItemDragCommand( AvfxTimelineItem item, int prevStart, int start, int prevEnd, int end ) {
            Item = item;
            State = (start, end);
            PrevState = (prevStart, prevEnd);
            // Already dragged, don't need to update here
        }

        public void Redo() {
            Item.StartTime.Value = State.Item1;
            Item.EndTime.Value = State.Item2;
        }

        public void Undo() {
            Item.StartTime.Value = PrevState.Item1;
            Item.EndTime.Value = PrevState.Item2;
        }
    }
}
