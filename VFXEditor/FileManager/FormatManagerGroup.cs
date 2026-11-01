namespace VfxEditor.FileManager {
    public class FormatManagerGroup<D, F, S>( FileFormat<D, F, S> format ) :
        FileManagerGroup<FormatManager<D, F, S>, D, F, S>( format.Title, format.Name ) where D : FileManagerDocument<F, S> where F : FileManagerFile {

        // Set by a field initializer so that it is available before the base constructor calls GetNewManager()
        public readonly FileFormat<D, F, S> Format = format;

        public override FormatCategory Category => Format.Category;

        protected override FormatManager<D, F, S> GetNewManager() => new( this, Format );
    }
}