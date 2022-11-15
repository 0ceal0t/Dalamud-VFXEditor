using System;
using System.IO;

namespace VfxEditor.AvfxFormat2 {
    public class UiTimelineView : UiNodeDropdownView<AvfxTimeline> {
        public UiTimelineView( AvfxFile file, UiNodeGroup<AvfxTimeline> group ) : base( file, group, "Timeline", true, true, "default_timeline.vfxedit" ) { }

        public override void OnSelect( AvfxTimeline item ) { }

        public override AvfxTimeline Read( BinaryReader reader, int size, bool hasDependencies ) {
            var item = new AvfxTimeline( File.NodeGroupSet, hasDependencies );
            item.Read( reader, size );
            return item;
        }
    }
}