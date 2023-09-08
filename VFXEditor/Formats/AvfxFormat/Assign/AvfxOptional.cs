namespace VfxEditor.AvfxFormat {
    public abstract class AvfxOptional : AvfxItem {
        public AvfxOptional( string avfxName ) : base( avfxName ) { }

        public abstract void DrawAssigned();

        public abstract void DrawUnassigned();

        public override void Draw() {
            if( IsAssigned() ) DrawAssigned();
            else DrawUnassigned();
        }
    }
}
