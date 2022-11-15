namespace VfxEditor.AvfxFormat {
    public abstract class AvfxOptional : AvfxItem {
        public AvfxOptional( string avfxName ) : base( avfxName ) { }

        public abstract void DrawAssigned( string id );
        public abstract void DrawUnassigned( string id );

        public override void Draw( string id ) {
            if( IsAssigned() ) DrawAssigned( id );
            else DrawUnassigned( id );
        }
    }
}
