using System;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiTimelineItemDragCommand : ICommand {
        private readonly UiTimelineItem Item;
        private readonly int StartBegin;
        private readonly int StartFinish;
        private readonly int EndBegin;
        private readonly int EndFinish;

        public UiTimelineItemDragCommand( UiTimelineItem item, int startBegin, int startFinish, int endBegin, int endFinish ) {
            Item = item;
            StartBegin = startBegin;
            StartFinish = startFinish;
            EndBegin = endBegin;
            EndFinish = endFinish;
        }

        public void Execute() {
        }

        public void Redo() {
            Item.StartTime.Literal.SetValue( StartFinish );
            Item.EndTime.Literal.SetValue( EndFinish );
        }

        public void Undo() {
            Item.StartTime.Literal.SetValue( StartBegin );
            Item.EndTime.Literal.SetValue( EndBegin );
        }
    }
}

