using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXTimelineItem : Base
    {
        public const string NAME = "Item";

        List<Base> Attributes;

        public List<AVFXTimelineSubItem> SubItems = new List<AVFXTimelineSubItem>();

        public AVFXTimelineItem() : base("item",NAME)
        {
            Attributes = new List<Base>(new Base[]{
            });
        }

        public override void read(AVFXNode node)
        {
            Assigned = true;
            ReadAVFX(Attributes, node);

            // split on every bEna
            List<AVFXNode> split = new List<AVFXNode>();
            int idx = 0;
            foreach (AVFXNode child in node.Children)
            {
                split.Add(child);

                if (idx == (node.Children.Count - 1) || node.Children[idx + 1].Name == "bEna") // split before bEna
                {
                    AVFXNode dummyNode = new AVFXNode("SubItem");
                    dummyNode.Children = split;
                    AVFXTimelineSubItem Item = new AVFXTimelineSubItem();
                    Item.read(dummyNode);
                    SubItems.Add(Item);
                    split = new List<AVFXNode>();
                }

                idx++;
            }
        }

        public override JToken toJSON()
        {
            JObject elem = new JObject();
            JArray itemArray = new JArray();
            foreach (AVFXTimelineSubItem item in SubItems)
            {
                itemArray.Add(item.toJSON());
            }
            elem["items"] = itemArray;
            return elem;
        }

        public override AVFXNode toAVFX()
        {
            // make ItPr by concatting elements of dummy elements
            AVFXNode itemAvfx = new AVFXNode("Item");
            foreach (AVFXTimelineSubItem Item in SubItems)
            {
                itemAvfx.Children.AddRange(Item.toAVFX().Children); // flatten
            }
            return itemAvfx;
        }
    }
}
