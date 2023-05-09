using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Select;
using VfxEditor.Utils;

namespace VfxEditor.UldFormat {
    public class UldDocument : FileManagerDocument<UldFile, WorkspaceMetaRenamed> {
        public UldDocument( UldManager manager, string writeLocation ) : base( manager, writeLocation, "Uld", "uld" ) { }

        public UldDocument( UldManager manager, string writeLocation, string localPath, string name, SelectResult source, SelectResult replace ) : 
            base( manager, writeLocation, localPath, name, source, replace, "Uld", "uld" ) { }

        public UldDocument( UldManager manager, string writeLocation, string localPath, WorkspaceMetaRenamed data ) : 
            this( manager, writeLocation, localPath, data.Name, data.Source, data.Replace ) {
            CurrentFile.ReadRenamingMap( data.Renaming );
        }

        protected override UldFile FileFromReader( BinaryReader reader ) => new( reader );

        public override WorkspaceMetaRenamed GetWorkspaceMeta( string newPath ) => new() {
            Name = Name,
            RelativeLocation = newPath,
            Replace = Replace,
            Source = Source,
            Renaming = CurrentFile.GetRenamingMap()
        };
    }
}
