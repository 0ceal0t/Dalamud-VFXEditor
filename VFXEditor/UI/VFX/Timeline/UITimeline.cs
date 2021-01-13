using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UITimeline : UIItem
    {
        public AVFXTimeline Timeline;
        public UITimelineView View;
        //=====================
        public List<UITimelineItem> Items;
        //=====================
        public List<UITimelineClip> Clips;
        //=====================
        public UITimelineClipSplitView ClipSplit;
        public UITimelineItemSplitView ItemSplit;

        public UITimeline(AVFXTimeline timeline, UITimelineView view)
        {
            Timeline = timeline;
            View = view;
            Init();
        }
        public override void Init()
        {
            base.Init();
            Items = new List<UITimelineItem>();
            Clips = new List<UITimelineClip>();
            //========================
            Attributes.Add(new UIInt("Loop Start", Timeline.LoopStart));
            Attributes.Add(new UIInt("Loop End", Timeline.LoopEnd));
            Attributes.Add(new UIInt("Binder Index", Timeline.BinderIdx));
            //========================
            foreach (var item in Timeline.Items)
            {
                Items.Add(new UITimelineItem(item, this));
            }
            //==========================
            foreach (var clip in Timeline.Clips)
            {
                Clips.Add(new UITimelineClip(clip, this));
            }
            //==========================
            ClipSplit = new UITimelineClipSplitView( Clips, this );
            ItemSplit = new UITimelineItemSplitView( Items, this );
        }

        public override void DrawBody( string parentId ) {
            string id = parentId + "/Timeline";
            //=====================
            if( ImGui.BeginTabBar( id + "/Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) ) {
                if( ImGui.BeginTabItem( "Parameters" + id ) ) {
                    DrawParameters( id + "/Params" );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Items" + id ) ) {
                    ItemSplit.Draw( id + "/Items" );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Clips" + id ) ) {
                    ClipSplit.Draw( id + "/Clips" );
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
        }

        private void DrawParameters( string id ) {
            ImGui.BeginChild( id );
            DrawAttrs( id );
            ImGui.EndChild();
        }

        public override void DrawSelect( int idx, string parentId, ref UIItem selected ) {
        }

        public override string GetText(int idx) {
            return "Timeline " + idx;
        }
    }
}
