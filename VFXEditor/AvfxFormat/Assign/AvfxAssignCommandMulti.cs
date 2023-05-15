namespace VfxEditor.AvfxFormat {
    internal class AvfxAssignCommandMulti : ICommand {
        private readonly AvfxBase Item;
        private readonly bool State;

        public AvfxAssignCommandMulti( AvfxBase item, bool state ) {
            Item = item;
            State = state;
        }

        public void Execute() => SetState( State );

        public void Redo() => SetState( State );

        public void Undo() => SetState( !State );

        private void SetState( bool state ) {
            if( state ) AvfxBase.RecurseAssigned( Item, true );
            else Item.SetAssigned( false );
        }
    }
}
