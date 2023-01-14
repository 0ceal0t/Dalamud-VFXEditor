using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using System;
using VfxEditor.Structs.Vfx;

namespace VfxEditor {
    public unsafe partial class Plugin {
        public static BaseVfx Spawn { get; private set; }
        public static bool InGpose => PluginInterface.UiBuilder.GposeActive;
        public static GameObject GposeTarget => Objects.CreateObjectReference( new IntPtr( TargetSystem.Instance()->GPoseTarget ) );
        public static GameObject PlayerObject => InGpose ? GposeTarget : ClientState?.LocalPlayer;
        public static GameObject TargetObject => InGpose ? GposeTarget : TargetManager?.Target;

        public static void RemoveSpawn() {
            if( Spawn == null ) return;
            Spawn?.Remove();
            Spawn = null;
        }

        public static void SpawnOnGround( string path ) {
            var playerObject = PlayerObject;
            if( playerObject == null ) return;
            Spawn = new StaticVfx( path, playerObject.Position, playerObject.Rotation );
        }

        public static void SpawnOnSelf( string path ) {
            var playerObject = PlayerObject;
            if( playerObject == null ) return;
            Spawn = new ActorVfx( playerObject, playerObject, path );
        }

        public static void SpawnOnTarget( string path ) {
            var targetObject = TargetObject;
            if( targetObject == null ) return;
            Spawn = new ActorVfx( targetObject, targetObject, path );
        }

        public static void ClearSpawn() {
            Spawn = null;
        }

        public static bool SpawnExists() => Spawn != null;
    }
}
