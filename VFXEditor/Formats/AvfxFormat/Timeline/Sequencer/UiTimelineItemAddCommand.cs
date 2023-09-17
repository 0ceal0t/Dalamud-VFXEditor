namespace VfxEditor.AvfxFormat {
    public class UiTimelineItemAddCommand : ICommand {
        private readonly UiTimelineItemSequencer View;
        private AvfxTimelineItem Item;
        private int Idx;

        public UiTimelineItemAddCommand( UiTimelineItemSequencer view ) {
            View = view;
        }

        public void Execute() {
            Idx = View.Items.Count;

            Item = new AvfxTimelineItem( View.Timeline, true );
            Item.BinderSelect.Select( null );
            Item.EffectorSelect.Select( null );
            Item.EmitterSelect.Select( null );

            Item.Enabled.Value = false;
            Item.EmitterIdx.Value = -1;
            Item.EffectorIdx.Value = -1;
            Item.BinderIdx.Value = -1;
            Item.Platform.Value = 0;
            Item.EndTime.Value = 1;
            Item.StartTime.Value = 0;

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
