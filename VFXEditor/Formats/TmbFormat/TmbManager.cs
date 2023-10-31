using VfxEditor.FileManager;
using VfxEditor.Select;
using VfxEditor.Select.Tmb;
using VfxEditor.Spawn;
using VfxEditor.Utils;

namespace VfxEditor.TmbFormat {
    public partial class TmbManager : FileManager<TmbDocument, TmbFile, WorkspaceMetaBasic> {
        public TmbManager() : base( "Tmb Editor", "Tmb" ) {
            SourceSelect = new TmbSelectDialog( "Tmb Select [LOADED]", this, true );
            ReplaceSelect = new TmbSelectDialog( "Tmb Select [REPLACED]", this, false );
        }

        public override void SetReplace( SelectResult result ) {
            base.SetReplace( result );
            if( Document != null ) Document.AnimationId = TmbSpawn.GetIdFromTmbPath( result.Path );
        }

        protected override TmbDocument GetNewDocument() => new( this, NewWriteLocation );

        protected override TmbDocument GetWorkspaceDocument( WorkspaceMetaBasic data, string localPath ) => new( this, NewWriteLocation, localPath, data );
    }
}
