using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Tracker {
    public unsafe class ActionTracker : Tracker {
        private class ActionData {
            public int ActorId;
            public string Path;
            public DateTime StartTime;

            public TrackerItem ToItem() => new() {
                Path = Path,
                Removed = false
            };
        }

        private readonly ConcurrentDictionary<IntPtr, ActionData> Actions = new();

        public ActionTracker() { }

        public void AddAction( IntPtr action, int actorId, string path ) {
            if( !Enabled ) return;
            Actions.TryAdd( action, new ActionData() {
                ActorId = actorId,
                Path = path,
                StartTime = DateTime.Now
            } );
        }

        public override void AddAll( HashSet<TrackerItem> displayItems ) {
            foreach( var item in Actions ) displayItems.Add( item.Value.ToItem() );
        }

        public override void Add( List<TrackerItemWithPosition> floatingItems, Dictionary<int, HashSet<TrackerItem>> actorToItems ) {
            foreach( var item in Actions ) {
                if( item.Key == IntPtr.Zero ) continue;

                var actorId = item.Value.ActorId;
                if( actorId <= 0 ) continue;

                if( !actorToItems.ContainsKey( actorId ) ) actorToItems[actorId] = new HashSet<TrackerItem>();
                actorToItems[actorId].Add( item.Value.ToItem() );
            }
        }

        public override void RemoveStale( DateTime _ ) {
            // Since you can't really "remove" an action, just wait 5 seconds for now
            var removeTime = DateTime.Now.Subtract( TimeSpan.FromSeconds( 5 ) );
            foreach( var item in Actions.Where( x => x.Value.StartTime < removeTime ).ToList() ) Actions.Remove( item.Key, out var _ );
        }

        public override void Reset() {
            Actions.Clear();
        }
    }
}
