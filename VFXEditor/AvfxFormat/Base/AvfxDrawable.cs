namespace VfxEditor.AvfxFormat {
    public abstract class AvfxDrawable : AvfxBase, IAvfxUiBase {
        public AvfxDrawable( string avfxName ) : base( avfxName ) { }

        public abstract void Draw( string parentId );
    }
}
