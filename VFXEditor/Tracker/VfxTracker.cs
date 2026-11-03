using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using FFXIVClientStructs.FFXIV.Client.Graphics.Vfx;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace VfxEditor.Tracker {
    public unsafe class VfxTracker : Tracker {
        private class VfxTrackedInstanceActor : VfxTrackedInstanceBase {
            public VfxData* Vfx;
            public ulong TargetId;
            public ulong CasterId;
            public override ulong GetActorId() => TargetId > 0 ? TargetId : CasterId;
        }

        private class VfxTrackedInstanceStatic : VfxTrackedInstanceBase {
            public VfxObject* Vfx;
            public override ulong GetActorId() => ( ulong )( Vfx->StaticTarget > 0 ? Vfx->StaticTarget : ( Vfx->StaticCaster > 0 ? Vfx->StaticCaster : 0 ) );
        }

        private abstract class VfxTrackedInstanceBase {
            public string Path;
            public bool Removed = false;
            public DateTime RemovedTime;

            public TrackerItem ToItem() => new() {
                Path = Path,
                Removed = Removed
            };

            public abstract ulong GetActorId();
        }

        private readonly ConcurrentDictionary<IntPtr, VfxTrackedInstanceActor> ActorVfxs = new();
        private readonly ConcurrentDictionary<IntPtr, VfxTrackedInstanceStatic> StaticVfxs = new();

        public VfxTracker() { }

        public void AddActor( VfxData* vfx, string path, ulong targetId, ulong casterId ) {
            if( !Enabled ) return;
            ActorVfxs.TryAdd( new IntPtr( vfx ), new VfxTrackedInstanceActor() {
                Path = path,
                Vfx = vfx,
                TargetId = targetId,
                CasterId = casterId
            } );
        }

        public void RemoveActor( VfxData* vfx ) {
            if( !Enabled ) return;
            if( ActorVfxs.TryGetValue( new IntPtr( vfx ), out var item ) ) {
                item.Removed = true;
                item.RemovedTime = DateTime.Now;
            }
        }

        public void AddStatic( VfxObject* vfx, string path ) {
            if( !Enabled ) return;
            StaticVfxs.TryAdd( new IntPtr( vfx ), new VfxTrackedInstanceStatic() {
                Path = path,
                Vfx = vfx
            } );
        }

        public void RemoveStatic( VfxObject* vfx ) {
            if( !Enabled ) return;
            if( StaticVfxs.TryGetValue( new IntPtr( vfx ), out var item ) ) {
                item.Removed = true;
                item.RemovedTime = DateTime.Now;
            }
        }

        public override void PopulateAll( HashSet<TrackerItem> displayItems ) {
            foreach( var item in StaticVfxs ) displayItems.Add( item.Value.ToItem() );
            foreach( var item in ActorVfxs ) displayItems.Add( item.Value.ToItem() );
        }

        public override void Populate( List<TrackerItemWithPosition> floatingItems, Dictionary<ulong, HashSet<TrackerItem>> actorToItems, Dictionary<IntPtr, HashSet<TrackerItem>> addressToItems ) {
            // Static
            foreach( var item in StaticVfxs ) {
                if( item.Key == IntPtr.Zero ) continue;

                var actorId = item.Value.GetActorId();
                if( actorId > 0 ) {
                    if( !actorToItems.ContainsKey( actorId ) ) actorToItems[actorId] = [];
                    actorToItems[actorId].Add( item.Value.ToItem() );
                }
                else {
                    var pos = item.Value.Vfx->Position;
                    floatingItems.Add( new TrackerItemWithPosition {
                        Item = item.Value.ToItem(),
                        Position = new Vector3( pos.X, pos.Y, pos.Z )
                    } );
                }
            }

            // Actors
            foreach( var item in ActorVfxs ) {
                if( item.Key == IntPtr.Zero ) continue;

                var actorId = item.Value.GetActorId();
                if( actorId <= 0 ) continue;

                if( !actorToItems.ContainsKey( actorId ) ) actorToItems[actorId] = [];
                actorToItems[actorId].Add( item.Value.ToItem() );
            }
        }

        public override void RemoveStale( DateTime removeTime ) {
            foreach( var item in StaticVfxs.Where( x => x.Value.Removed && x.Value.RemovedTime < removeTime ).ToList() ) StaticVfxs.Remove( item.Key, out var _ );
            foreach( var item in ActorVfxs.Where( x => x.Value.Removed && x.Value.RemovedTime < removeTime ).ToList() ) ActorVfxs.Remove( item.Key, out var _ );
        }

        public override void Reset() {
            ActorVfxs.Clear();
            StaticVfxs.Clear();
        }
    }
}