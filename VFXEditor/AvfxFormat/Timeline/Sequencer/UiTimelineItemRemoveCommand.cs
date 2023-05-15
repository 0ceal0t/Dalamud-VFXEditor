namespace VfxEditor.AvfxFormat {
    public class UiTimelineItemRemoveCommand : ICommand {
        private readonly UiTimelineItemSequencer View;
        private readonly AvfxTimelineItem Item;
        private int Idx;

        public UiTimelineItemRemoveCommand( UiTimelineItemSequencer view, AvfxTimelineItem item ) {
            View = view;
            Item = item;
        }

        public void Execute() {
            Idx = View.Items.IndexOf( Item );
            Redo();
        }

        public void Redo() {
            View.Items.Remove( Item );
            View.UpdateIdx();
            View.ClearSelected();

            Item.BinderSelect.Disable();
            Item.EmitterSelect.Disable();
            Item.EffectorSelect.Disable();
        }

        public void Undo() {
            View.Items.Insert( Idx, Item );
            View.UpdateIdx();

            Item.BinderSelect.Enable();
            Item.EmitterSelect.Enable();
            Item.EffectorSelect.Enable();
        }
    }
}
