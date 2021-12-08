using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.Structs.Vfx;

namespace VFXEditor {
    public unsafe partial class Plugin {
        public static void RemoveSpawnVfx() {
            SpawnVFX?.Remove();
            SpawnVFX = null;
        }

        public static void SpawnOnGround( string path ) {
            SpawnVFX = new StaticVfx( path, ClientState.LocalPlayer.Position );
        }

        public static void SpawnOnSelf( string path ) {
            SpawnVFX = new ActorVfx( ClientState.LocalPlayer, ClientState.LocalPlayer, path );
        }

        public static void SpawnOnTarget( string path ) {
            var t = TargetManager.Target;
            if( t != null ) {
                SpawnVFX = new ActorVfx( t, t, path );
            }
        }

        public static void ClearSpawnVfx() {
            SpawnVFX = null;
        }

        public static bool SpawnExists() {
            return SpawnVFX != null;
        }
    }
}
