using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Structs.Vfx;

namespace VfxEditor.Spawn {
    public enum SpawnType {
        None,
        Ground,
        Self,
        Target
    }

    public class VfxSpawnItem {
        public readonly string Path;
        public readonly SpawnType Type;
        public readonly bool CanLoop;

        public VfxSpawnItem( string path, SpawnType type, bool canLoop ) {
            Path = path;
            Type = type;
            CanLoop = canLoop;
        }
    }

    public class VfxLoopItem {
        public VfxSpawnItem Item;
        public DateTime RemovedTime;

        public VfxLoopItem( VfxSpawnItem item, DateTime removedTime ) {
            Item = item;
            RemovedTime = removedTime;
        }
    }

    public static class VfxSpawn {
        public static readonly Dictionary<nint, (BaseVfx Vfx, VfxSpawnItem Item)> Vfxs = [];
        public static readonly List<VfxLoopItem> ToLoop = [];

        public static bool IsActive => Vfxs.Count > 0;

        public static void DrawPopup( string path, bool loop ) {
            using var popup = ImRaii.Popup( "SpawnPopup" );
            if( !popup ) return;

            if( ImGui.Selectable( "Self" ) ) OnSelf( path, loop );
            if( ImGui.Selectable( "Ground" ) ) OnGround( path, loop );
            if( ImGui.Selectable( "Target" ) ) OnTarget( path, loop );

            ImGui.Separator();

            if( ImGui.Checkbox( "Loop", ref Plugin.Configuration.VfxSpawnLoop ) ) Plugin.Configuration.Save();
            ImGui.SetNextItemWidth( 150 );
            if( ImGui.InputFloat( "Delay", ref Plugin.Configuration.VfxSpawnDelay ) ) Plugin.Configuration.Save();
        }

        public static void OnGround( string path, bool canLoop ) {
            var playerObject = Plugin.PlayerObject;
            if( playerObject == null ) return;

            Add( new StaticVfx( path, playerObject.Position, playerObject.Rotation ), new( path, SpawnType.Ground, canLoop ) );
        }

        public static void OnSelf( string path, bool canLoop ) {
            var playerObject = Plugin.PlayerObject;
            if( playerObject == null ) return;

            Add( new ActorVfx( playerObject, playerObject, path ), new( path, SpawnType.Self, canLoop ) );
        }

        public static void OnTarget( string path, bool canLoop ) {
            var targetObject = Plugin.TargetObject;
            if( targetObject == null ) return;

            Add( new ActorVfx( targetObject, targetObject, path ), new( path, SpawnType.Target, canLoop ) );
        }

        private static void Add( BaseVfx vfx, VfxSpawnItem item ) {
            var address = vfx.GetAddress();
            if( address == IntPtr.Zero ) {
                Dalamud.Error( $"Could not spawn {item.Path}" );
                return;
            }
            Vfxs[address] = (vfx, item);
        }

        public static void Tick() {
            var justLooped = new List<VfxLoopItem>();
            foreach( var loop in ToLoop ) {
                if( ( DateTime.Now - loop.RemovedTime ).TotalSeconds < Plugin.Configuration.VfxSpawnDelay ) continue;

                justLooped.Add( loop );
                if( loop.Item.Type == SpawnType.Self ) OnSelf( loop.Item.Path, true );
                else if( loop.Item.Type == SpawnType.Ground ) OnGround( loop.Item.Path, true );
                else if( loop.Item.Type == SpawnType.Target ) OnTarget( loop.Item.Path, true );
            }

            ToLoop.RemoveAll( justLooped.Contains );
        }

        public static void Clear() {
            foreach( var (vfx, _) in Vfxs.Values.ToList() ) vfx.Remove(); // this also calls InteropRemoved()
            Vfxs.Clear();
            ToLoop.Clear();
        }

        public static void InteropRemoved( nint data ) {
            if( data == IntPtr.Zero || !Vfxs.Remove( data, out var entry ) ) return;
            if( Plugin.Configuration.VfxSpawnLoop && entry.Item.CanLoop ) ToLoop.Add( new( entry.Item, DateTime.Now ) );
        }

        public static void Dispose() => Clear();
    }
}