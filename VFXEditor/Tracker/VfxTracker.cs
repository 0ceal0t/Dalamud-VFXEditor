using SharpDX;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Structs.Vfx;

namespace VfxEditor.Tracker {
    public unsafe class VfxTracker : Tracker {
        private class VfxData {
            public bool IsStatic = false;
            public string Path;
            public VfxStruct* Vfx;
            public bool Removed = false;
            public DateTime RemovedTime;

            public int ActorId => IsStatic ? ChooseId( Vfx->StaticCaster, Vfx->StaticTarget ) :
                ChooseId( Vfx->ActorCaster, Vfx->ActorTarget );

            public TrackerItem ToItem() => new() {
                Path = Path,
                Removed = Removed
            };

            private static int ChooseId( int caster, int target ) => target > 0 ? target : ( caster > 0 ? caster : -1 );
        }

        private readonly ConcurrentDictionary<IntPtr, VfxData> ActorVfxs = new();
        private readonly ConcurrentDictionary<IntPtr, VfxData> StaticVfxs = new();

        public VfxTracker() { }

        public void AddActor( VfxStruct* vfx, string path ) {
            if( !Enabled ) return;
            ActorVfxs.TryAdd( new IntPtr( vfx ), new VfxData() {
                Path = path,
                Vfx = vfx
            } );
        }

        public void RemoveActor( VfxStruct* vfx ) {
            if( !Enabled ) return;
            if( ActorVfxs.TryGetValue( new IntPtr( vfx ), out var item ) ) {
                item.Removed = true;
                item.RemovedTime = DateTime.Now;
            }
        }

        public void AddStatic( VfxStruct* vfx, string path ) {
            if( !Enabled ) return;
            StaticVfxs.TryAdd( new IntPtr( vfx ), new VfxData() {
                IsStatic = true,
                Path = path,
                Vfx = vfx
            } );
        }

        public void RemoveStatic( VfxStruct* vfx ) {
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

        public override void Populate( List<TrackerItemWithPosition> floatingItems, Dictionary<int, HashSet<TrackerItem>> actorToItems, Dictionary<IntPtr, HashSet<TrackerItem>> addressToItems ) {
            // Static
            foreach( var item in StaticVfxs ) {
                if( item.Key == IntPtr.Zero ) continue;

                var actorId = item.Value.ActorId;
                if( actorId > 0 ) {
                    if( !actorToItems.ContainsKey( actorId ) ) actorToItems[actorId] = new();
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

                var actorId = item.Value.ActorId;
                if( actorId <= 0 ) continue;

                if( !actorToItems.ContainsKey( actorId ) ) actorToItems[actorId] = new();
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
