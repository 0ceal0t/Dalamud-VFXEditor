namespace VfxEditor.AvfxFormat {
    internal class AvfxAssignCommandMulti : ICommand {
        private readonly AvfxBase Item;
        private readonly bool Assigned;

        public AvfxAssignCommandMulti( AvfxBase item, bool assigned ) {
            Item = item;
            Assigned = assigned;
        }

        public void Execute() => SetState( Assigned );

        public void Redo() => SetState( Assigned );

        public void Undo() => SetState( !Assigned );

        private void SetState( bool state ) {
            if( state ) AvfxBase.RecurseAssigned( Item, true );
            else Item.SetAssigned( false );
        }
    }
}
