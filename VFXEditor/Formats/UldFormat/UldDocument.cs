using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.UldFormat {
    public class UldDocument : FileManagerDocument<UldFile, WorkspaceMetaRenamed> {
        public override string Id => "Uld";
        public override string Extension => "uld";

        public UldDocument( UldManager manager, string writeLocation ) : base( manager, writeLocation ) { }

        public UldDocument( UldManager manager, string writeLocation, string localPath, WorkspaceMetaRenamed data ) : this( manager, writeLocation ) {
            LoadWorkspace( localPath, data.RelativeLocation, data.Name, data.Source, data.Replace, data.Disabled );
            CurrentFile?.ReadRenamingMap( data.Renaming );
        }

        protected override UldFile FileFromReader( BinaryReader reader ) => new( reader, true );

        public override WorkspaceMetaRenamed GetWorkspaceMeta( string newPath ) => new() {
            Name = Name,
            RelativeLocation = newPath,
            Replace = Replace,
            Source = Source,
            Renaming = CurrentFile.GetRenamingMap(),
            Disabled = Disabled
        };
    }
}
