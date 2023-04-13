using System;
using VfxEditor.FileManager;
using VfxEditor.Select.Uld;
using VfxEditor.Utils;

namespace VfxEditor.UldFormat {
    public unsafe class UldManager : FileManagerWindow<UldDocument, UldFile, WorkspaceMetaRenamed> {
        public UldManager() : base( "Uld Editor", "Uld", "uld", "Uld", "Uld" ) {
            SourceSelect = new UldSelectDialog( "Uld Select [LOADED]", this, true );
            ReplaceSelect = new UldSelectDialog( "Uld Select [REPLACED]", this, false );
        }

        protected override UldDocument GetNewDocument() => new( this, LocalPath );

        protected override UldDocument GetWorkspaceDocument( WorkspaceMetaRenamed data, string localPath ) =>
            new( this, LocalPath, WorkspaceUtils.ResolveWorkspacePath( data.RelativeLocation, localPath ), data );
    }
}
