namespace VfxEditor.AvfxFormat2 {
    public class UiTimelineItemAddCommand : ICommand {
        private readonly UiTimelineItemSequencer View;
        private readonly int Idx;
        private AvfxTimelineSubItem Item;

        public UiTimelineItemAddCommand( UiTimelineItemSequencer view ) {
            View = view;
            Idx = View.Items.Count;
        }

        public void Execute() {
            Item = new AvfxTimelineSubItem( View.Timeline, true );
            Item.BinderSelect.Select( null );
            Item.EffectorSelect.Select( null );
            Item.EmitterSelect.Select( null );
            Item.EndTime.SetValue( 1 );
            Item.StartTime.SetValue( 0 );

            Add();
            View.Selected = Item;
        }

        public void Redo() {
            Add();
            Item.BinderSelect.Enable();
            Item.EmitterSelect.Enable();
            Item.EffectorSelect.Enable();
        }

        public void Undo() {
            View.Items.Remove( Item );
            View.UpdateIdx();
            View.ClearSelected();

            Item.BinderSelect.Disable();
            Item.EmitterSelect.Disable();
            Item.EffectorSelect.Disable();
        }

        private void Add() {
            View.Items.Insert( Idx, Item );
            View.UpdateIdx();
        }
    }
}
