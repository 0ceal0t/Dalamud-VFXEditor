namespace VfxEditor.AvfxFormat {
    public class UiTimelineItemDragCommand : ICommand {
        private readonly AvfxTimelineItem Item;
        private readonly int StartBegin;
        private readonly int StartFinish;
        private readonly int EndBegin;
        private readonly int EndFinish;

        public UiTimelineItemDragCommand( AvfxTimelineItem item, int startBegin, int startFinish, int endBegin, int endFinish ) {
            Item = item;
            StartBegin = startBegin;
            StartFinish = startFinish;
            EndBegin = endBegin;
            EndFinish = endFinish;
        }

        public void Execute() { }

        public void Redo() {
            Item.StartTime.Value = StartFinish;
            Item.EndTime.Value = EndFinish;
        }

        public void Undo() {
            Item.StartTime.Value = StartBegin;
            Item.EndTime.Value = EndBegin;
        }
    }
}
