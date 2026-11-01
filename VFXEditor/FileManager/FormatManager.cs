namespace VfxEditor.FileManager {
    public class FormatManager<D, F, S> : FileManager<D, F, S> where D : FileManagerDocument<F, S> where F : FileManagerFile {
        public readonly FileFormat<D, F, S> Format;

        public FormatManager( FileManagerGroupBase group, FileFormat<D, F, S> format ) : base( group ) {
            Format = format;
            SourceSelect = format.CreateSelectDialog( this, true );
            ReplaceSelect = format.CreateSelectDialog( this, false );
        }

        protected override D GetNewDocument() => Format.NewDocument( this, NewWriteLocation );

        protected override D GetWorkspaceDocument( S data, string localPath ) => Format.WorkspaceDocument( this, NewWriteLocation, localPath, data );
    }
}