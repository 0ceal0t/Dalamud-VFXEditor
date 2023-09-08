using System;

namespace VfxEditor.AvfxFormat {
    public class UiCurveEditorCommand : ICommand {
        private readonly UiCurveEditor Item;
        private readonly Action ChangeAction;
        private UiCurveEditorState State;
        private UiCurveEditorState PrevState;

        public UiCurveEditorCommand( UiCurveEditor item, Action changeAction ) {
            Item = item;
            ChangeAction = changeAction;
        }

        public void Execute() {
            PrevState = Item.GetState();
            Item.Curve.Keys.SetAssigned( true );
            ChangeAction.Invoke();
            State = Item.GetState();
        }

        public void Redo() => Item.SetState( State );

        public void Undo() => Item.SetState( PrevState );
    }
}
