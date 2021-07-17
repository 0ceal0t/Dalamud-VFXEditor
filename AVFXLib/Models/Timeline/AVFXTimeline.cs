using AVFXLib.AVFX;
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

        public LiteralInt LoopStart = new LiteralInt("LpSt");
        public LiteralInt LoopEnd = new LiteralInt("LpEd");
        public LiteralInt BinderIdx = new LiteralInt("BnNo");
        public LiteralInt TimelineCount = new LiteralInt("TICn");
        public LiteralInt ClipCount = new LiteralInt("CpCn");

        List<Base> Attributes;

        public List<AVFXTimelineSubItem> Items = new List<AVFXTimelineSubItem>();
        public List<AVFXTimelineClip> Clips = new List<AVFXTimelineClip>();

        public AVFXTimeline() : base(NAME)
        {
            Attributes = new List<Base>(new Base[] {
                LoopStart,
                LoopEnd,
                BinderIdx,
                TimelineCount,
                ClipCount
            });
        }

        public override void Read(AVFXNode node)
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
                        lastItem.Read(item);
                        break;
                    // CLIPS ====================
                    case AVFXTimelineClip.NAME:
                        AVFXTimelineClip Clip = new AVFXTimelineClip();
                        Clip.Read(item);
                        Clips.Add(Clip);
                        break;
                }
            }

            if(lastItem != null)
            {
                Items.AddRange(lastItem.SubItems);
            }
        }

        public AVFXTimelineSubItem AddItem()
        {
            AVFXTimelineSubItem Item = new AVFXTimelineSubItem();
            Item.ToDefault();
            Items.Add(Item);
            TimelineCount.GiveValue(Items.Count());
            return Item;
        }
        public void AddItem(AVFXTimelineSubItem item ) {
            Items.Add( item );
            TimelineCount.GiveValue( Items.Count() );
        }
        public void RemoveItem(int idx)
        {
            Items.RemoveAt(idx);
            TimelineCount.GiveValue(Items.Count());
        }
        public void RemoveItem(AVFXTimelineSubItem item ) {
            Items.Remove( item );
            TimelineCount.GiveValue( Items.Count() );
        }
        public AVFXTimelineClip AddClip()
        {
            AVFXTimelineClip Clip = new AVFXTimelineClip();
            Clip.ToDefault();
            Clips.Add(Clip);
            ClipCount.GiveValue(Clips.Count());
            return Clip;
        }
        public void AddClip(AVFXTimelineClip item ) {
            Clips.Add( item );
            ClipCount.GiveValue( Clips.Count() );
        }
        public void RemoveClip(int idx)
        {
            Clips.RemoveAt(idx);
            ClipCount.GiveValue(Clips.Count());
        }
        public void RemoveClip(AVFXTimelineClip item ) {
            Clips.Remove( item );
            ClipCount.GiveValue( Clips.Count() );
        }

        public override AVFXNode ToAVFX()
        {
            AVFXNode tmlnAvfx = new AVFXNode("TmLn");

            PutAVFX(tmlnAvfx, Attributes);

            // Items
            //=======================//
            for (int i = 0; i < Items.Count(); i++)
            {
                AVFXTimelineItem Item = new AVFXTimelineItem();
                Item.SubItems = Items.GetRange(0, i + 1);
                tmlnAvfx.Children.Add(Item.ToAVFX());
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
