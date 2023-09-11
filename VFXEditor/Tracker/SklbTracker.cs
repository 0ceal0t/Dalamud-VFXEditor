using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using System;
using System.Collections.Generic;
using VfxEditor.Ui.Tools;

namespace VfxEditor.Tracker {
    public unsafe class SklbTracker : Tracker {
        public SklbTracker() { }

        public override void PopulateAll( HashSet<TrackerItem> displayItems ) {
            foreach( var item in Plugin.Objects ) {
                PopulateSklbs( ( GameObject* )item.Address, displayItems );
            }
        }

        public override void Populate( List<TrackerItemWithPosition> floatingItems, Dictionary<int, HashSet<TrackerItem>> actorToItems ) {
            foreach( var item in Plugin.Objects ) {
                var paths = new HashSet<TrackerItem>();
                PopulateSklbs( ( GameObject* )item.Address, paths );

                if( paths.Count == 0 ) continue;

                var id = ( int )item.ObjectId;
                if( !actorToItems.ContainsKey( id ) ) actorToItems[id] = new HashSet<TrackerItem>();

                foreach( var path in paths ) {
                    actorToItems[id].Add( path );
                }
            }
        }

        public override void RemoveStale( DateTime removeTime ) { }

        public override void Reset() { }

        private void PopulateSklbs( GameObject* gameObject, HashSet<TrackerItem> paths ) {
            if( gameObject == null ) return;
            if( gameObject->ObjectID == 0 ) return;

            var drawObject = gameObject->DrawObject;
            if( drawObject == null ) return;
            if( drawObject->Object.GetObjectType() != ObjectType.CharacterBase ) return;

            PopulateSklbs( ( CharacterBase* )drawObject, paths );

            var child = drawObject->Object.ChildObject;
            if( child != null && child->GetObjectType() == ObjectType.CharacterBase ) {
                PopulateSklbs( ( CharacterBase* )child, paths );
            }
        }

        private static void PopulateSklbs( CharacterBase* characterBase, HashSet<TrackerItem> paths ) {
            var skeleton = characterBase->Skeleton;
            if( skeleton == null ) return;

            var sklbTable = new IntPtr( skeleton->SkeletonResourceHandles );
            var resources = LoadedTab.GetResourcesFromTable( sklbTable, IntPtr.Zero, true );
            foreach( var resource in resources ) paths.Add( new() {
                Path = resource,
                Removed = false
            } );
        }
    }
}
