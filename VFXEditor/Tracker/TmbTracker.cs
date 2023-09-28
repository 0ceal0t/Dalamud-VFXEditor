using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using VfxEditor.Interop;
using VfxEditor.Structs;

namespace VfxEditor.Tracker {
    public unsafe class TmbTracker : Tracker {
        private class ActionData {
            public int ActorId;
            public IntPtr Address;
            public string Path;
            public DateTime StartTime;

            public TrackerItem ToItem() => new() {
                Path = Path,
                Removed = false
            };
        }

        // ===============

        private readonly ConcurrentDictionary<IntPtr, ActionData> Actions = new();

        public TmbTracker() { }

        public void AddAction( IntPtr timeline ) {
            if( !Enabled ) return;

            try {
                if( timeline == IntPtr.Zero ) return;

                var getGameObjectIdx = ( ( delegate* unmanaged< IntPtr, int >** )timeline )[0][Constants.GetGameObjectIdxVfunc];
                var idx = getGameObjectIdx( timeline );
                if( idx < 0 || idx >= Dalamud.Objects.Length ) return;

                var obj = ( GameObject* )Dalamud.Objects.GetObjectAddress( idx );
                if( obj == null ) return;

                var action = Marshal.ReadIntPtr( timeline + Constants.TimelineToActionOffset );
                if( action == IntPtr.Zero ) return;

                // Something like battle/idle
                // var actionString = Marshal.PtrToStringAnsi( action + 74 );

                var objectId = obj->ObjectID;

                var resource = ( ResourceHandle* )Marshal.ReadIntPtr( action + 24 );
                if( resource == null ) return;

                var tmbPath = resource->FileName().ToString();

                Actions.TryAdd( action, new ActionData() {
                    ActorId = ( int )objectId,
                    Address = new IntPtr( obj ),
                    Path = tmbPath,
                    StartTime = DateTime.Now
                } );

            }
            catch( Exception e ) {
                PluginLog.Error( e, "Error reading timeline" );
            }
        }

        public override void PopulateAll( HashSet<TrackerItem> displayItems ) {
            foreach( var item in Actions ) displayItems.Add( item.Value.ToItem() );
        }

        public override void Populate( List<TrackerItemWithPosition> floatingItems, Dictionary<int, HashSet<TrackerItem>> actorToItems, Dictionary<IntPtr, HashSet<TrackerItem>> addressToItems ) {
            foreach( var item in Actions ) {
                if( item.Key == IntPtr.Zero ) continue;

                var address = item.Value.Address;
                if( address <= 0 ) continue;

                if( !addressToItems.ContainsKey( address ) ) addressToItems[address] = new();
                addressToItems[address].Add( item.Value.ToItem() );
            }
        }

        public override void RemoveStale( DateTime _ ) {
            // Since you can't really "remove" an action, just wait 5 seconds for now
            var removeTime = DateTime.Now.Subtract( TimeSpan.FromSeconds( 5 ) );
            foreach( var item in Actions.Where( x => x.Value.StartTime < removeTime ).ToList() ) Actions.Remove( item.Key, out var _ );
        }

        public override void Reset() => Actions.Clear();
    }
}
