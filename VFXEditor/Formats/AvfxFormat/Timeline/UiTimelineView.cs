using System.IO;

namespace VfxEditor.AvfxFormat {
    public class UiTimelineView : AvfxNodeDropdownView<AvfxTimeline> {
        public UiTimelineView( AvfxFile file, NodeGroup<AvfxTimeline> group ) : base( file, group, "Timeline", true, true, "default_timeline.vfxedit" ) { }

        public override void OnSelect( AvfxTimeline item ) { }

        public override AvfxTimeline Read( BinaryReader reader, int size ) {
            var item = new AvfxTimeline( File.NodeGroupSet );
            item.Read( reader, size );
            return item;
        }
    }
}