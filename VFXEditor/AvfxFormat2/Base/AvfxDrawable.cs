namespace VfxEditor.AvfxFormat2 {
    public abstract class AvfxDrawable : AvfxBase, IUiBase {
        public AvfxDrawable( string avfxName ) : base( avfxName ) { }

        public abstract void Draw( string parentId );
    }
}
