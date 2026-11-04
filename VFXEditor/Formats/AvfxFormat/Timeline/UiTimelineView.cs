using System.IO;
using VfxEditor.Formats.AvfxFormat.Nodes;
using VfxEditor.Formats.AvfxFormat.Views.Nodes;

namespace VfxEditor.Formats.AvfxFormat.Timeline {
    public class UiTimelineView : AvfxNodeDropdownView<AvfxTimeline> {
        public UiTimelineView( AvfxFile file, NodeGroup<AvfxTimeline> group ) : base( file, group, "Timeline", true, true, "default_timeline.vfxedit" ) { }

        public override void OnSelect( AvfxTimeline item ) { }

        public override AvfxTimeline Read( BinaryReader reader, int size ) {
            var item = new AvfxTimeline( File, File.NodeGroupSet );
            item.Read( reader, size );
            return item;
        }
    }
}