using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXTimeline : Base
    {
        public const string NAME = "TmLn";

        public LiteralInt LoopStart = new LiteralInt("loopStart", "LpSt");
        public LiteralInt LoopEnd = new LiteralInt("loopEnd", "LpEd");
        public LiteralInt BinderIdx = new LiteralInt("binderIdx", "BnNo");
        public LiteralInt TimelineCount = new LiteralInt("timelineCount", "TICn");
        public LiteralInt ClipCount = new LiteralInt("clipCount", "CpCn");

        List<Base> Attributes;

        public List<AVFXTimelineSubItem> Items = new List<AVFXTimelineSubItem>();
        public List<AVFXTimelineClip> Clips = new List<AVFXTimelineClip>();

        public AVFXTimeline() : base("timelines", NAME)
        {
            Attributes = new List<Base>(new Base[] {
                LoopStart,
                LoopEnd,
                BinderIdx,
                TimelineCount,
                ClipCount
            });
        }

        public override void read(AVFXNode node)
        {
            Assigned = true;
            ReadAVFX(Attributes, node);

            AVFXTimelineItem lastItem = null;

            foreach (AVFXNode item in node.Children)
            {
                switch (item.Name)
                {
                    // ITEMS ====================
                    case AVFXTimelineItem.NAME:
                        lastItem = new AVFXTimelineItem();
                        lastItem.read(item);
                        break;
                    // CLIPS ====================
                    case AVFXTimelineClip.NAME:
                        AVFXTimelineClip Clip = new AVFXTimelineClip();
                        Clip.read(item);
                        Clips.Add(Clip);
                        break;
                }
            }

            if(lastItem != null)
            {
                Items.AddRange(lastItem.SubItems);
            }
        }

        public override void toDefault()
        {
            Assigned = true;
            SetDefault(Attributes);
            BinderIdx.GiveValue(-1);
            TimelineCount.GiveValue(0);
            ClipCount.GiveValue(0);
            Items = new List<AVFXTimelineSubItem>();
            Clips = new List<AVFXTimelineClip>();
        }

        public void addItem()
        {
            AVFXTimelineSubItem Item = new AVFXTimelineSubItem();
            Item.toDefault();
            Items.Add(Item);
            TimelineCount.GiveValue(Items.Count());
        }
        public void removeItem(int idx)
        {
            Items.RemoveAt(idx);
            TimelineCount.GiveValue(Items.Count());
        }
        public void addClip()
        {
            AVFXTimelineClip Clip = new AVFXTimelineClip();
            Clip.toDefault();
            Clips.Add(Clip);
            ClipCount.GiveValue(Clips.Count());
        }
        public void removeClip(int idx)
        {
            Clips.RemoveAt(idx);
            ClipCount.GiveValue(Clips.Count());
        }

        public override JToken toJSON()
        {
            JObject elem = new JObject();
            PutJSON(elem, Attributes);

            JArray itemArray = new JArray();
            foreach(AVFXTimelineSubItem item in Items)
            {
                itemArray.Add(item.toJSON());
            }
            elem["items"] = itemArray;

            JArray clipArray = new JArray();
            foreach (AVFXTimelineClip clip in Clips)
            {
                clipArray.Add(clip.toJSON());
            }
            elem["clips"] = clipArray;

            return elem;
        }

        public override AVFXNode toAVFX()
        {
            AVFXNode tmlnAvfx = new AVFXNode("TmLn");

            PutAVFX(tmlnAvfx, Attributes);

            // Items
            //=======================//
            for (int i = 0; i < Items.Count(); i++)
            {
                AVFXTimelineItem Item = new AVFXTimelineItem();
                Item.SubItems = Items.GetRange(0, i + 1);
                tmlnAvfx.Children.Add(Item.toAVFX());
            }

            // Clips
            //=======================//
            foreach (AVFXTimelineClip clipElem in Clips)
            {
                PutAVFX(tmlnAvfx, clipElem);
            }

            return tmlnAvfx;
        }
    }
}
