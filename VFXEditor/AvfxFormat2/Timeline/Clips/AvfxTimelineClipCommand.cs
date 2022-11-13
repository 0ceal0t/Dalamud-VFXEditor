using System;
using VfxEditor;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxTimelineClipCommand : ICommand {
        private readonly AvfxTimelineClip Item;
        private readonly AvfxTimelineClipState PrevState;
        private AvfxTimelineClipState State;
        private readonly Action ChangeAction;

        public AvfxTimelineClipCommand( AvfxTimelineClip item, Action changeAction ) {
            Item = item;
            ChangeAction = changeAction;
            PrevState = item.GetState();
        }

        public void Execute() {
            ChangeAction.Invoke();
            State = Item.GetState();
        }

        public void Redo() {
            Item.SetState( State );
        }

        public void Undo() {
            Item.SetState( PrevState );
        }
    }
}
