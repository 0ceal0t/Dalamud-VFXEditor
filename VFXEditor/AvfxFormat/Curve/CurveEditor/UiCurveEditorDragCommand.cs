namespace VfxEditor.AvfxFormat {
    public class UiCurveEditorDragCommand : ICommand {
        private readonly UiCurveEditor Item;
        private readonly UiCurveEditorState PrevState;
        private readonly UiCurveEditorState State;

        public UiCurveEditorDragCommand( UiCurveEditor item, UiCurveEditorState prevState, UiCurveEditorState state ) {
            Item = item;
            PrevState = prevState;
            State = state;
        }

        public void Execute() => Item.Curve.Keys.SetAssigned( true );

        public void Redo() => Item.SetState( State );

        public void Undo() => Item.SetState( PrevState );
    }
}
