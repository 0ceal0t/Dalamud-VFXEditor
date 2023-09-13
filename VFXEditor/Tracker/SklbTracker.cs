using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using VfxEditor.Ui.Tools;

namespace VfxEditor.Tracker {
    public unsafe class SklbTracker : Tracker {
        public SklbTracker() { }

        public override void PopulateAll( HashSet<TrackerItem> displayItems ) {
            foreach( var item in Plugin.Objects ) {
                PopulateSklbs( ( GameObject* )item.Address, displayItems );
            }
        }

        public override void Populate( List<TrackerItemWithPosition> floatingItems, Dictionary<int, HashSet<TrackerItem>> actorToItems, Dictionary<IntPtr, HashSet<TrackerItem>> addressToItems ) {
            foreach( var item in Plugin.Objects ) {
                var paths = new HashSet<TrackerItem>();
                PopulateSklbs( ( GameObject* )item.Address, paths );

                if( paths.Count == 0 ) continue;

                var id = new IntPtr( item.Address );
                if( !addressToItems.ContainsKey( id ) ) addressToItems[id] = new HashSet<TrackerItem>();

                foreach( var path in paths ) {
                    addressToItems[id].Add( path );
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
            if( skeleton->PartialSkeletonCount == 0 || Marshal.ReadIntPtr( new IntPtr( skeleton ) + 112 ) == IntPtr.Zero ) return; // idk what 112 even is

            var sklbTable = new IntPtr( skeleton->SkeletonResourceHandles );
            var resources = LoadedTab.GetResourcesFromTable( sklbTable, IntPtr.Zero, true );
            foreach( var resource in resources ) paths.Add( new() {
                Path = resource,
                Removed = false
            } );
        }
    }
}
