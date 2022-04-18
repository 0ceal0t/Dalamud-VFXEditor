using System;
using System.IO;
using System.Linq;
using VFXEditor.AVFXLib;
using VFXEditor.AVFXLib.Timeline;

namespace VFXEditor.Avfx.Vfx {
    public class UITimelineView : UIDropdownView<UITimeline> {
        public UITimelineView( AvfxFile main, AVFXMain avfx ) : base( main, avfx, "##TIME", "Select a Timeline", defaultPath: "timeline_default.vfxedit" ) {
            Group = main.Timelines;
            Group.Items = AVFX.Timelines.Select( item => new UITimeline( Main, item ) ).ToList();
        }

        public override void OnDelete( UITimeline item ) {
            AVFX.RemoveTimeline( item.Timeline );
        }

        public override void OnExport( BinaryWriter writer, UITimeline item ) => item.Write( writer );

        public override UITimeline OnImport( BinaryReader reader, int size, bool has_dependencies = false ) {
            var item = new AVFXTimeline();
            item.Read( reader, size );
            AVFX.AddTimeline( item );
            return new UITimeline( Main, item, has_dependencies );
        }
    }
}
