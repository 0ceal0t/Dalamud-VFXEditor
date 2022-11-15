namespace VfxEditor.AvfxFormat2 {
    public class UiTimelineItemDragCommand : ICommand {
        private readonly AvfxTimelineSubItem Item;
        private readonly int StartBegin;
        private readonly int StartFinish;
        private readonly int EndBegin;
        private readonly int EndFinish;

        public UiTimelineItemDragCommand( AvfxTimelineSubItem item, int startBegin, int startFinish, int endBegin, int endFinish ) {
            Item = item;
            StartBegin = startBegin;
            StartFinish = startFinish;
            EndBegin = endBegin;
            EndFinish = endFinish;
        }

        public void Execute() { }

        public void Redo() {
            Item.StartTime.SetValue( StartFinish );
            Item.EndTime.SetValue( EndFinish );
        }

        public void Undo() {
            Item.StartTime.SetValue( StartBegin );
            Item.EndTime.SetValue( EndBegin );
        }
    }
}
