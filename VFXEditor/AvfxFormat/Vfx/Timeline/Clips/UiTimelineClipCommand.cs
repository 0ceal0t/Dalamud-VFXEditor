using System;
using VfxEditor.AVFXLib;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiTimelineClipCommand : ICommand {
        private readonly UiTimelineClip Item;
        private readonly UiTimelineClipState PrevState;
        private UiTimelineClipState State;
        private readonly Action ChangeAction;

        public UiTimelineClipCommand( UiTimelineClip item, Action changeAction ) {
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
