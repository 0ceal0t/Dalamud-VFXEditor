namespace VfxEditor.AvfxFormat {
    public class UiTimelineItemAddCommand : ICommand {
        private readonly UiTimelineItemSequencer View;
        private readonly AvfxTimelineItem Item;
        private readonly int Idx;

        public UiTimelineItemAddCommand( UiTimelineItemSequencer view ) {
            View = view;
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
            View.Items.Insert( Idx, Item );
            View.UpdateIdx();
            View.Selected = Item;
        }

        public void Redo() {
            View.Items.Insert( Idx, Item );
            View.UpdateIdx();
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
    }
}
