using System.Collections.Generic;
using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Select;
using VfxEditor.Utils;

namespace VfxEditor.UldFormat {
    public class UldDocument : FileManagerDocument<UldFile, WorkspaceMetaRenamed> {
        public UldDocument( UldManager manager, string writeLocation ) : base( manager, writeLocation, "Uld", "uld" ) { }

        public UldDocument( UldManager manager, string writeLocation, string localPath, string name,
                SelectResult source, SelectResult replace, bool disabled, Dictionary<string, string> renamed ) :
                base( manager, writeLocation, localPath, name, source, replace, disabled, "Uld", "uld" ) {

            CurrentFile.ReadRenamingMap( renamed );
        }

        protected override UldFile FileFromReader( BinaryReader reader ) => new( reader );

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
