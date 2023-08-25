using VfxEditor.FileManager;
using VfxEditor.Interop.Havok;
using VfxEditor.Select.Sklb;
using VfxEditor.Utils;

namespace VfxEditor.SklbFormat {
    public class SklbManager : FileManagerWindow<SklbDocument, SklbFile, WorkspaceMetaBasic> {
        public SklbManager() : base( "Sklb Editor", "Sklb" ) {
            SourceSelect = new SklbSelectDialog( "Sklb Select [LOADED]", this, true );
            ReplaceSelect = new SklbSelectDialog( "Sklb Select [REPLACED]", this, false );
        }

        protected override SklbDocument GetNewDocument() => new( this, NewWriteLocation );

        protected override SklbDocument GetWorkspaceDocument( WorkspaceMetaBasic data, string localPath ) =>
            new( this, NewWriteLocation, WorkspaceUtils.ResolveWorkspacePath( data.RelativeLocation, localPath ), data.Name, data.Source, data.Replace );

        public bool GetSimpleSklb( string path, out SimpleSklb skeleton, out bool replaced ) {
            replaced = false;
            skeleton = null;

            // Local
            if( System.IO.Path.IsPathRooted( path ) ) {
                if( System.IO.Path.Exists( path ) ) {
                    skeleton = SimpleSklb.LoadFromLocal( path );
                    return true;
                }
                return false;
            }

            // Game file
            foreach( var document in Documents ) {
                if( document.CurrentFile == null ) continue;
                if( document.ReplacePath.Equals( path ) ) {
                    replaced = true;
                    skeleton = SimpleSklb.LoadFromLocal( document.WritePath );
                    return true;
                }
            }

            if( Plugin.DataManager.FileExists( path ) ) {
                skeleton = Plugin.DataManager.GetFile<SimpleSklb>( path );
                return true;
            }

            return false;
        }
    }
}
