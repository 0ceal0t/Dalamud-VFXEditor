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

        public LiteralInt LoopStart = new("LpSt");
        public LiteralInt LoopEnd = new("LpEd");
        public LiteralInt BinderIdx = new("BnNo");
        public LiteralInt TimelineCount = new("TICn");
        public LiteralInt ClipCount = new("CpCn");
        readonly List<Base> Attributes;

        public List<AVFXTimelineSubItem> Items = new();
        public List<AVFXTimelineClip> Clips = new();

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

            foreach (var item in node.Children)
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
                        var Clip = new AVFXTimelineClip();
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
            var Item = new AVFXTimelineSubItem();
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
            var Clip = new AVFXTimelineClip();
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
            var tmlnAvfx = new AVFXNode("TmLn");

            PutAVFX(tmlnAvfx, Attributes);

            // Items
            //=======================//
            for (var i = 0; i < Items.Count(); i++)
            {
                var Item = new AVFXTimelineItem {
                    SubItems = Items.GetRange( 0, i + 1 )
                };
                tmlnAvfx.Children.Add(Item.ToAVFX());
            }

            // Clips
            //=======================//
            foreach (var clipElem in Clips)
            {
                PutAVFX(tmlnAvfx, clipElem);
            }

            return tmlnAvfx;
        }
    }
}
