using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
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

        public LiteralInt ItemCount = new LiteralInt("itemCount", "ItCn");
        public LiteralInt TriggerCount = new LiteralInt("triggerCount", "TrCn");

        public List<AVFXScheduleSubItem> Items = new List<AVFXScheduleSubItem>();
        public List<AVFXScheduleSubItem> Triggers = new List<AVFXScheduleSubItem>();

        List<Base> Attributes;

        public AVFXSchedule() : base("schedules", NAME)
        {
            Attributes = new List<Base>(new Base[] {
                ItemCount,
                TriggerCount
            });
        }

        public override void read(AVFXNode node)
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
                        lastItem.read(item);
                        break;
                    // TRIGGERS =================
                    case AVFXScheduleTrigger.NAME:
                        lastTrigger = new AVFXScheduleTrigger();
                        lastTrigger.read(item);
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

        public override void toDefault()
        {
            Assigned = true;
            ItemCount.GiveValue(0);
            TriggerCount.GiveValue(0);
            Items = new List<AVFXScheduleSubItem>();
            Triggers = new List<AVFXScheduleSubItem>();

            addItem();
            for(int i = 0; i < 12; i++)
            {
                addTrigger();
            }
        }

        public void addItem()
        {
            AVFXScheduleSubItem Item = new AVFXScheduleSubItem();
            Item.toDefault();
            Items.Add(Item);
            ItemCount.GiveValue(Items.Count());
        }
        public void removeItem(int idx)
        {
            Items.RemoveAt(idx);
            ItemCount.GiveValue(Items.Count());
        }
        public void addTrigger()
        {
            AVFXScheduleSubItem Trigger = new AVFXScheduleSubItem();
            Trigger.toDefault();
            Triggers.Add(Trigger);
            TriggerCount.GiveValue(Triggers.Count());
        }
        public void removeTrigger(int idx)
        {
            Triggers.RemoveAt(idx);
            TriggerCount.GiveValue(Triggers.Count());
        }

        public override JToken toJSON()
        {
            JObject elem = new JObject();
            PutJSON(elem, Attributes);

            JArray itemArray = new JArray();
            foreach (AVFXScheduleSubItem item in Items)
            {
                itemArray.Add(item.toJSON());
            }
            elem["items"] = itemArray;

            JArray triggerArray = new JArray();
            foreach (AVFXScheduleSubItem trigger in Triggers)
            {
                triggerArray.Add(trigger.toJSON());
            }
            elem["triggers"] = triggerArray;

            return elem;
        }

        public override AVFXNode toAVFX()
        {
            AVFXNode schdAvfx = new AVFXNode("Schd");

            PutAVFX(schdAvfx, Attributes);

            // Items
            //=======================//
            for (int i = 0; i < Items.Count(); i++)
            {
                AVFXScheduleItem Item = new AVFXScheduleItem();
                Item.SubItems = Items.GetRange(0, i + 1);
                schdAvfx.Children.Add(Item.toAVFX());
            }

            // Triggers
            //=======================//
            for(int i = 0; i < Triggers.Count(); i++)
            {
                AVFXScheduleTrigger Trigger = new AVFXScheduleTrigger();
                Trigger.SubItems = new List<AVFXScheduleSubItem>();
                Trigger.SubItems.AddRange(Items);
                Trigger.SubItems.AddRange(Triggers.GetRange(0, i + 1));
                schdAvfx.Children.Add(Trigger.toAVFX());
            }

            return schdAvfx;
        }
    }
}
