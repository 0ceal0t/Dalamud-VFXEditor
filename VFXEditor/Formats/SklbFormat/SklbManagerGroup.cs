using VfxEditor.FileManager;
using VfxEditor.Interop.Havok;
using VfxEditor.SklbFormat;
using VfxEditor.Utils;

namespace VfxEditor.Formats.SklbFormat {
    public class SklbManagerGroup : FileManagerGroup<SklbManager, SklbDocument, SklbFile, WorkspaceMetaBasic> {
        public SklbManagerGroup() : base( "Sklb Editor", "Sklb" ) { }

        protected override SklbManager GetNewManager() => new( this );

        public bool GetSimpleSklb( string path, out SimpleSklb skeleton, out SklbSource source ) {
            source = SklbSource.Game;
            skeleton = null;

            // Local file path (typed directly, or picked via the file browser)
            if( System.IO.Path.IsPathRooted( path ) ) {
                if( System.IO.Path.Exists( path ) ) {
                    skeleton = SimpleSklb.LoadFromLocal( path );
                    source = SklbSource.Local;
                    return true;
                }
                return false;
            }

            // Matches a skeleton currently being edited in this tool
            foreach( var document in Documents ) {
                if( document.File == null ) continue;
                if( document.ReplacePath.ToLower().Equals( path.ToLower() ) ) {
                    source = SklbSource.Document;
                    skeleton = SimpleSklb.LoadFromLocal( document.WriteLocation );
                    return true;
                }
            }

            // A Penumbra mod replaces this game path in the active collection
            if( Plugin.PenumbraIpc.PenumbraFileExists( path, out var penumbraPath ) ) {
                skeleton = SimpleSklb.LoadFromLocal( penumbraPath );
                source = SklbSource.Penumbra;
                return true;
            }

            // Plain game file
            if( Dalamud.DataManager.FileExists( path ) ) {
                skeleton = Dalamud.DataManager.GetFile<SimpleSklb>( path );
                return true;
            }

            return false;
        }
    }
}
