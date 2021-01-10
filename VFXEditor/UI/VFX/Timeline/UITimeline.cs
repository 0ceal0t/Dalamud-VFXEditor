using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UITimeline : UIBase
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

        public string GetDescText()
        {
            return "Timeline " + Idx;
        }

        public override void Draw(string parentId)
        {
            string id = parentId + "/Timeline" + Idx;
            //=====================
            if( ImGui.BeginTabBar( id + "/Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) )
            {
                if( ImGui.BeginTabItem( "Parameters" + id ) )
                {
                    DrawParameters( id + "/Params" );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Items" + id ) )
                {
                    DrawItems( id + "/Items" );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Clips" + id ) )
                {
                    DrawClips( id + "/Clips" );
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
        }

        private void DrawParameters( string id )
        {
            ImGui.BeginChild( id );
            DrawAttrs( id );
            ImGui.EndChild();
        }
        private void DrawItems( string id )
        {
            ItemSplit.Draw( id );
        }
        private void DrawClips( string id )
        {
            ClipSplit.Draw( id );
        }
    }
}
