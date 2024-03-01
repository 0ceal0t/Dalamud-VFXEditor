using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System;
using VfxEditor.Structs.Vfx;
using VfxEditor.Utils;

namespace VfxEditor.Spawn {
    public static unsafe class VfxSpawn {
        private enum SpawnType {
            None,
            Ground,
            Self,
            Target
        }

        public static BaseVfx Vfx { get; private set; }
        public static bool IsActive => Vfx != null;

        private static SpawnType LoopType = SpawnType.None;
        private static string LoopPath = "";
        private static bool LastSpawnCanLoop = false; // Don't want to loop from the selection dialog
        private static bool Removed = false;
        private static DateTime RemoveTime;

        public static void DrawButton( string path, bool loop ) {
            using var style = ImRaii.PushColor( ImGuiCol.Button, UiUtils.DARK_GRAY );

            if( IsActive ) {
                if( ImGui.Button( "Remove" ) ) Remove();
                return;
            }

            if( !Plugin.Configuration.VfxSpawnSplit ) {
                if( ImGui.Button( "Spawn" ) ) ImGui.OpenPopup( "SpawnPopup" );
                if( ImGui.IsItemClicked( ImGuiMouseButton.Right ) ) ToggleSplit();

                DrawPopup( path, false );
            }
            else {
                if( ImGui.Button( "Self" ) ) OnSelf( path, loop );
                if( ImGui.IsItemClicked( ImGuiMouseButton.Right ) ) ToggleSplit();

                ImGui.SameLine();
                if( ImGui.Button( "Ground" ) ) OnGround( path, loop );
                if( ImGui.IsItemClicked( ImGuiMouseButton.Right ) ) ToggleSplit();
            }
        }

        private static void ToggleSplit() {
            Plugin.Configuration.VfxSpawnSplit = !Plugin.Configuration.VfxSpawnSplit;
            Plugin.Configuration.Save();
        }

        public static void DrawPopup( string path, bool loop ) {
            using var popup = ImRaii.Popup( "SpawnPopup" );
            if( !popup ) return;

            if( ImGui.Selectable( "Self" ) ) OnSelf( path, loop );
            if( ImGui.Selectable( "Ground" ) ) OnGround( path, loop );
            if( ImGui.Selectable( "Target" ) ) OnTarget( path, loop );
        }

        public static void OnGround( string path, bool canLoop ) {
            var playerObject = Plugin.PlayerObject;
            if( playerObject == null ) return;

            LoopPath = path;
            LoopType = SpawnType.Ground;
            LastSpawnCanLoop = canLoop;
            Removed = false;

            Vfx = new StaticVfx( path, playerObject.Position, playerObject.Rotation );
        }

        public static void OnSelf( string path, bool canLoop ) {
            var playerObject = Plugin.PlayerObject;
            if( playerObject == null ) return;

            LoopPath = path;
            LoopType = SpawnType.Self;
            LastSpawnCanLoop = canLoop;
            Removed = false;

            Vfx = new ActorVfx( playerObject, playerObject, path );
        }

        public static void OnTarget( string path, bool canLoop ) {
            var targetObject = Plugin.TargetObject;
            if( targetObject == null ) return;

            LoopPath = path;
            LoopType = SpawnType.Target;
            LastSpawnCanLoop = canLoop;
            Removed = false;

            Vfx = new ActorVfx( targetObject, targetObject, path );
        }

        public static void Tick() {
            if( !Removed || !LastSpawnCanLoop || LoopType == SpawnType.None || string.IsNullOrEmpty( LoopPath ) ) return;
            if( ( DateTime.Now - RemoveTime ).TotalSeconds < Plugin.Configuration.VfxSpawnDelay ) return;

            if( LoopType == SpawnType.Self ) OnSelf( LoopPath, true );
            else if( LoopType == SpawnType.Ground ) OnGround( LoopPath, true );
            else if( LoopType == SpawnType.Target ) OnTarget( LoopPath, true );
        }

        public static void Remove() {
            if( Vfx != null ) {
                Vfx?.Remove(); // this also calls InteropRemoved()
                Vfx = null;
            }

            LoopType = SpawnType.None;
            LoopPath = "";
            Removed = false;
        }

        public static void InteropRemoved() {
            Vfx = null;

            if( Plugin.Configuration.VfxSpawnLoop && LastSpawnCanLoop ) {
                RemoveTime = DateTime.Now;
                Removed = true;
            }
            else {
                LoopType = SpawnType.None;
                LoopPath = "";
                Removed = false;
            }
        }

        public static void Dispose() => Remove();
    }
}
