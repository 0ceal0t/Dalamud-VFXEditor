namespace VfxEditor.AvfxFormat {
    public abstract class GenericItem : IUiItem {
        public abstract string GetDefaultText();
        public virtual string GetText() => GetDefaultText();

        public abstract void Draw( string parentId );
    }
}
