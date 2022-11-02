using System.IO;
using System.Linq;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Timeline;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiTimelineView : UiNodeDropdownView<UiTimeline> {
        public UiTimelineView( AvfxFile vfxFile, AVFXMain avfx, UiNodeGroup<UiTimeline> group ) : base( vfxFile, avfx, group, "Timeline", true, true, "default_timeline.vfxedit" ) { }

        public override void OnDelete( UiTimeline item ) => AVFX.RemoveTimeline( item.Timeline );

        public override void OnExport( BinaryWriter writer, UiTimeline item ) => item.Write( writer );

        public override UiTimeline OnImport( BinaryReader reader, int size, bool has_dependencies = false ) {
            var item = new AVFXTimeline();
            item.Read( reader, size );
            AVFX.AddTimeline( item );
            return new UiTimeline( item, VfxFile.NodeGroupSet, has_dependencies );
        }

        public override void OnSelect( UiTimeline item ) { }
    }
}
