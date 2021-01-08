using AVFXLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using AVFXLib.AVFX;

namespace VFXEditor.UI.VFX
{
    public class UITimelineView : UIDropdownView
    {
        public AVFXBase AVFX;
        List<UITimeline> Timelines;

        public UITimelineView(AVFXBase avfx) : base( "##TIME", "Select a Timeline" )
        {
            AVFX = avfx;
            Init();
        }
        public override void Init()
        {
            base.Init();
            Timelines = new List<UITimeline>();
            Options = new string[AVFX.Timelines.Count];
            int idx = 0;
            foreach( var timeline in AVFX.Timelines )
            {
                var item = new UITimeline( timeline, this );
                item.Idx = idx;
                Options[idx] = item.GetDescText();
                Timelines.Add( item );
                idx++;
            }
        }

        public override void OnNew()
        {
            AVFX.addTimeline();
        }
        public override void OnDelete( int idx )
        {
            AVFX.removeTimeline( idx );
        }
        public override void OnDraw( int idx )
        {
            Timelines[idx].Draw( id );
        }
        public override byte[] OnExport( int idx )
        {
            return Timelines[idx].Timeline.toAVFX().toBytes();
        }
        public override void RefreshDesc( int idx )
        {
            Options[idx] = Timelines[idx].GetDescText();
        }
        public override void OnImport( AVFXNode node ) {
            AVFXTimeline item = new AVFXTimeline();
            item.read( node );
            AVFX.addTimeline( item );
        }
    }
}
