using System.IO;
using System.Linq;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Timeline;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiTimelineView : UiNodeDropdownView<UiTimeline> {
        public UiTimelineView( AvfxFile vfxFile, AVFXMain avfx, UiNodeGroup<UiTimeline> group ) : base( vfxFile, avfx, group, "Timeline", true, true, "default_timeline.vfxedit" ) { }

        public override void RemoveFromAvfx( UiTimeline item ) => Avfx.Timelines.Remove( item.Timeline );

        public override void AddToAvfx( UiTimeline item, int idx ) => Avfx.Timelines.Insert( idx, item.Timeline );

        public override void OnExport( BinaryWriter writer, UiTimeline item ) => item.Write( writer );

        public override UiTimeline AddToAvfx( BinaryReader reader, int size, bool hasDepdencies ) {
            var item = new AVFXTimeline();
            item.Read( reader, size );
            Avfx.Timelines.Add( item );
            return new UiTimeline( item, VfxFile.NodeGroupSet, hasDepdencies );
        }

        public override void OnSelect( UiTimeline item ) { }
    }
}
