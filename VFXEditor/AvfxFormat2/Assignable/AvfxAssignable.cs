namespace VfxEditor.AvfxFormat2 {
    public abstract class AvfxAssignable : AvfxItem {
        public AvfxAssignable( string avfxName ) : base( avfxName ) { }

        public abstract void DrawAssigned( string id );
        public abstract void DrawUnassigned( string id );

        public override void Draw( string id ) {
            if( IsAssigned() ) DrawAssigned( id );
            else DrawUnassigned( id );
        }
    }
}
