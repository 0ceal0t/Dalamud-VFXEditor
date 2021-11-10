using AVFXLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using AVFXLib.AVFX;

namespace VFXEditor.UI.Vfx {
    public class UITimelineView : UIDropdownView<UITimeline> {
        public UITimelineView(UIMain main, AVFXBase avfx) : base(main, avfx, "##TIME", "Select a Timeline", defaultPath: "timeline_default.vfxedit" ) {
            Group = main.Timelines;
            Group.Items = AVFX.Timelines.Select( item => new UITimeline( Main, item ) ).ToList();
        }

        public override void OnDelete( UITimeline item ) {
            AVFX.RemoveTimeline( item.Timeline );
        }

        public override byte[] OnExport( UITimeline item ) {
            return item.Timeline.ToAVFX().ToBytes();
        }

        public override UITimeline OnImport( AVFXNode node, bool has_dependencies = false ) {
            var item = new AVFXTimeline();
            item.Read( node );
            AVFX.AddTimeline( item );
            return new UITimeline( Main, item, has_dependencies );
        }
    }
}
