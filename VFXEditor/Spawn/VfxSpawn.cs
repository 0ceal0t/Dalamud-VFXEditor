using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using System;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Data;
using VfxEditor.Interop.Structs.Vfx;
using VfxEditor.Utils;

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

        // Also active while a looped VFX is waiting to respawn, so that it can still be de-spawned
        public static bool IsActive => Vfxs.Count > 0 || ToLoop.Count > 0;

        public static void DrawPopup( string path, bool loop ) {
            using var popup = ImRaii.Popup( "SpawnPopup" );
            if( !popup ) return;

            using( ImRaii.Disabled( !IsActive ) ) {
                if( ImGui.Selectable( "De-Spawn" ) ) Clear();
            }
            using( ImRaii.Disabled( IsActive ) ) {
                if( ImGui.Selectable( "Self" ) ) OnSelf( path, loop );
                if( ImGui.Selectable( "Ground" ) ) OnGround( path, loop );
                if( ImGui.Selectable( "Target" ) ) OnTarget( path, loop );
            }

            ImGui.Separator();

            if( ImGui.Checkbox( "Loop", ref Plugin.Configuration.VfxSpawnLoop ) ) Plugin.Configuration.Save();
            ImGui.SetNextItemWidth( 150 );
            if( ImGui.InputFloat( "Delay", ref Plugin.Configuration.VfxSpawnDelay ) ) Plugin.Configuration.Save();
        }

        // Spawning a VFX whose file the game cannot find crashes the game, so check first
        public static bool CanSpawn( string path, out string error ) {
            if( !GamePathUtils.IsWellFormed( path, "avfx", out error ) ) return false;
            if( GamePathUtils.CanLoad( path ) ) return true;

            error = Plugin.PenumbraIpc?.PenumbraEnabled != true ? "Penumbra is not available, and the path does not exist in the game files" :
                Plugin.PenumbraIpc.ModsEnabled != true ? "Penumbra mods are disabled, and the path does not exist in the game files" :
                "The path does not exist in the game files, and is not being replaced";
            return false;
        }

        private static bool CheckSpawn( string path ) {
            if( CanSpawn( path, out var error ) ) return true;
            Dalamud.Error( $"Could not spawn {path}: {error}" );
            Dalamud.ErrorNotification( $"Could not spawn {path}: {error}" );
            return false;
        }

        public static void OnGround( string path, bool canLoop ) {
            if( !CheckSpawn( path ) ) return;
            var playerObject = Plugin.PlayerObject;
            if( playerObject == null ) return;

            Add( new StaticVfx( path, playerObject.Position, playerObject.Rotation ), new( path, SpawnType.Ground, canLoop ) );
        }

        public static void OnSelf( string path, bool canLoop ) {
            if( !CheckSpawn( path ) ) return;
            var playerObject = Plugin.PlayerObject;
            if( playerObject == null ) return;

            Add( new ActorVfx( playerObject, playerObject, path ), new( path, SpawnType.Self, canLoop ) );
        }

        public static void OnTarget( string path, bool canLoop ) {
            if( !CheckSpawn( path ) ) return;
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
            var toRemove = Vfxs.Values.Select( x => x.Vfx ).ToList();
            Vfxs.Clear();
            ToLoop.Clear();

            foreach( var vfx in toRemove ) {
                try {
                    vfx.Remove();
                }
                catch( Exception e ) {
                    Dalamud.Error( e, $"Error removing {vfx.Path}" );
                }
            }
        }

        public static void InteropRemoved( nint data ) {
            if( data == IntPtr.Zero || !Vfxs.Remove( data, out var entry ) ) return;
            if( Plugin.Configuration.VfxSpawnLoop && entry.Item.CanLoop ) ToLoop.Add( new( entry.Item, DateTime.Now ) );
        }

        public static void Dispose() => Clear();
    }
}