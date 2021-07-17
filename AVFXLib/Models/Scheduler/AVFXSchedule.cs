using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXSchedule : Base
    {
        public const string NAME = "Schd";

        public LiteralInt ItemCount = new LiteralInt("ItCn");
        public LiteralInt TriggerCount = new LiteralInt("TrCn");

        public List<AVFXScheduleSubItem> Items = new List<AVFXScheduleSubItem>();
        public List<AVFXScheduleSubItem> Triggers = new List<AVFXScheduleSubItem>();

        List<Base> Attributes;

        public AVFXSchedule() : base(NAME)
        {
            Attributes = new List<Base>(new Base[] {
                ItemCount,
                TriggerCount
            });
        }

        public override void Read(AVFXNode node)
        {
            Assigned = true;
            ReadAVFX(Attributes, node);

            AVFXScheduleItem lastItem = null;
            AVFXScheduleTrigger lastTrigger = null;

            foreach (AVFXNode item in node.Children)
            {
                switch (item.Name)
                {
                    // ITEMS ===================
                    case AVFXScheduleItem.NAME:
                        lastItem = new AVFXScheduleItem();
                        lastItem.Read(item);
                        break;
                    // TRIGGERS =================
                    case AVFXScheduleTrigger.NAME:
                        lastTrigger = new AVFXScheduleTrigger();
                        lastTrigger.Read(item);
                        break;
                }
            }

            if(lastItem != null)
            {
                Items.AddRange(lastItem.SubItems);
            }
            if(lastTrigger != null)
            {
                Triggers.AddRange(lastTrigger.SubItems.GetRange(lastTrigger.SubItems.Count - 12, 12));
            }
        }

        public AVFXScheduleSubItem AddItem()
        {
            AVFXScheduleSubItem Item = new AVFXScheduleSubItem();
            Item.ToDefault();
            Items.Add(Item);
            ItemCount.GiveValue(Items.Count());
            return Item;
        }
        public void AddItem(AVFXScheduleSubItem item ) {
            Items.Add( item );
            ItemCount.GiveValue( Items.Count() );
        }
        public void RemoveItem(int idx)
        {
            Items.RemoveAt(idx);
            ItemCount.GiveValue(Items.Count());
        }
        public void RemoveItem(AVFXScheduleSubItem item ) {
            Items.Remove( item );
            ItemCount.GiveValue( Items.Count() );
        }

        public override AVFXNode ToAVFX()
        {
            AVFXNode schdAvfx = new AVFXNode("Schd");

            PutAVFX(schdAvfx, Attributes);

            // Items
            //=======================//
            for (int i = 0; i < Items.Count(); i++)
            {
                AVFXScheduleItem Item = new AVFXScheduleItem();
                Item.SubItems = Items.GetRange(0, i + 1);
                schdAvfx.Children.Add(Item.ToAVFX());
            }

            // Triggers
            //=======================//
            for(int i = 0; i < Triggers.Count(); i++)
            {
                AVFXScheduleTrigger Trigger = new AVFXScheduleTrigger();
                Trigger.SubItems = new List<AVFXScheduleSubItem>();
                Trigger.SubItems.AddRange(Items);
                Trigger.SubItems.AddRange(Triggers.GetRange(0, i + 1));
                schdAvfx.Children.Add(Trigger.ToAVFX());
            }

            return schdAvfx;
        }
    }
}
