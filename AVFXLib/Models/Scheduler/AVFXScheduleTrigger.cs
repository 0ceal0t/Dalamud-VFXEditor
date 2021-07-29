using AVFXLib.AVFX;
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

        public List<AVFXScheduleSubItem> SubItems = new();

        public AVFXScheduleTrigger() : base(NAME)
        {
        }

        public override void Read(AVFXNode node)
        {
            Assigned = true;

            // split every 3 leafs, make dummy elements and insert
            var numItems = (int)Math.Floor((double)node.Children.Count / 3);
            for (var idx = 0; idx < numItems; idx++)
            {
                var subItem = node.Children.GetRange(idx * 3, 3);
                var dummyNode = new AVFXNode( "SubItem" ) {
                    Children = subItem
                };

                var Item = new AVFXScheduleSubItem();
                Item.Read(dummyNode);
                SubItems.Add(Item);
            }
        }

        public override AVFXNode ToAVFX()
        {
            // make ItPr by concatting elements of dummy elements
            var itemAvfx = new AVFXNode("Trgr");
            foreach (var Item in SubItems)
            {
                itemAvfx.Children.AddRange(Item.ToAVFX().Children); // flatten
            }

            return itemAvfx;
        }
    }
}
