using System;
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
                if( System.IO.Path.Exists( path ) && TryLoadSklb( () => SimpleSklb.LoadFromLocal( path ), path, out skeleton ) ) {
                    source = SklbSource.Local;
                    return true;
                }
                return false;
            }

            // Matches a skeleton currently being edited in this tool
            foreach( var document in Documents ) {
                if( document.File == null ) continue;
                if( !document.ReplacePath.ToLower().Equals( path.ToLower() ) ) continue;
                if( TryLoadSklb( () => SimpleSklb.LoadFromLocal( document.WriteLocation ), document.WriteLocation, out skeleton ) ) {
                    source = SklbSource.Document;
                    return true;
                }
                break; // fall through to the Penumbra/game checks below
            }

            // A Penumbra mod replaces this game path in the active collection
            if( Plugin.PenumbraIpc.PenumbraFileExists( path, out var penumbraPath ) &&
                TryLoadSklb( () => SimpleSklb.LoadFromLocal( penumbraPath ), penumbraPath, out skeleton ) ) {
                source = SklbSource.Penumbra;
                return true;
            }

            // Plain game file
            if( Dalamud.DataManager.FileExists( path ) &&
                TryLoadSklb( () => Dalamud.DataManager.GetFile<SimpleSklb>( path ), path, out skeleton ) ) {
                source = SklbSource.Game;
                return true;
            }

            return false;
        }

        // Any of the above sources can point at a stale/corrupt/incompatible file; on failure
        // this logs and lets the caller fall through to the next source instead of crashing.
        private static bool TryLoadSklb( Func<SimpleSklb> loader, string path, out SimpleSklb skeleton ) {
            try {
                skeleton = loader();
                return true;
            }
            catch( Exception e ) {
                Dalamud.Error( e, $"Could not load sklb {path}" );
                skeleton = null;
                return false;
            }
        }
    }
}
