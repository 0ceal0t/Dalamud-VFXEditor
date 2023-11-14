namespace VfxEditor.AvfxFormat {
    public abstract class AvfxOptional : AvfxItem {
        public AvfxOptional( string avfxName ) : base( avfxName ) { }

        public override void SetAssigned( bool assigned, bool recurse = false ) {
            base.SetAssigned( assigned, recurse );
            if( recurse && assigned ) { // never recursively unassign
                foreach( var child in GetChildren() ) child?.SetAssigned( assigned, recurse );
            }
        }

        public abstract void DrawAssigned();

        public abstract void DrawUnassigned();

        public override void Draw() {
            if( IsAssigned() ) DrawAssigned();
            else DrawUnassigned();
        }
    }
}
