using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXScheduleTrigger : Base
    {
        public const string NAME = "Trgr";

        public List<AVFXScheduleSubItem> SubItems = new List<AVFXScheduleSubItem>();

        public AVFXScheduleTrigger() : base("trigger", NAME)
        {
        }

        public override void read(AVFXNode node)
        {
            Assigned = true;

            // split every 3 leafs, make dummy elements and insert
            int numItems = (int)Math.Floor((double)node.Children.Count / 3);
            for (int idx = 0; idx < numItems; idx++)
            {
                List<AVFXNode> subItem = node.Children.GetRange(idx * 3, 3);
                AVFXNode dummyNode = new AVFXNode("SubItem");
                dummyNode.Children = subItem;

                AVFXScheduleSubItem Item = new AVFXScheduleSubItem();
                Item.read(dummyNode);
                SubItems.Add(Item);
            }
        }

        public override JToken toJSON()
        {
            JObject elem = new JObject();

            JArray partArray = new JArray();
            foreach (AVFXScheduleSubItem subItem in SubItems)
            {
                partArray.Add(subItem.toJSON());
            }
            elem["parts"] = partArray;

            return elem;
        }

        public override AVFXNode toAVFX()
        {
            // make ItPr by concatting elements of dummy elements
            AVFXNode itemAvfx = new AVFXNode("Trgr");
            foreach (AVFXScheduleSubItem Item in SubItems)
            {
                itemAvfx.Children.AddRange(Item.toAVFX().Children); // flatten
            }

            return itemAvfx;
        }
    }
}
