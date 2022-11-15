using System;

namespace VfxEditor.AvfxFormat {
    public class UiCurveEditorCommand : ICommand {
        private readonly UiCurveEditor Item;
        private readonly UiCurveEditorState PrevState;
        private UiCurveEditorState State;
        private readonly Action ChangeAction;

        public UiCurveEditorCommand( UiCurveEditor item, Action changeAction ) {
            Item = item;
            ChangeAction = changeAction;
            PrevState = item.GetState();
        }

        public void Execute() {
            Item.Curve.Keys.SetAssigned( true );
            ChangeAction.Invoke();
            State = Item.GetState();
        }

        public void Redo() => Item.SetState( State );

        public void Undo() => Item.SetState( PrevState );
    }
}
