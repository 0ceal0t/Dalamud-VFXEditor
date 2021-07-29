using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXEmitterCreateParticle : Base
    {
        public const string NAME = "ItPr";

        public List<AVFXEmitterIterationItem> Items;

        public AVFXEmitterCreateParticle() : base(NAME)
        {
            Items = new List<AVFXEmitterIterationItem>();
        }

        public override void Read(AVFXNode node)
        {
            Assigned = true;
            // split every 26 leafs, make dummy elements and insert
            var numItems = (int)Math.Floor((double)node.Children.Count / 26);
            for (var idx = 0; idx < numItems; idx++)
            {
                var subItem = node.Children.GetRange(idx * 26, 26);
                var dummyNode = new AVFXNode( "ItPr_Item" ) {
                    Children = subItem
                };

                var Item = new AVFXEmitterIterationItem();
                Item.Read(dummyNode);
                Items.Add(Item);
            }
        }

        public override AVFXNode ToAVFX()
        {
            // make ItPr by concatting elements of dummy elements
            var ItPr = new AVFXNode("ItPr");
            foreach (var Item in Items)
            {
                ItPr.Children.AddRange(Item.ToAVFX().Children); // flatten
            }
            return ItPr;
        }
    }
}
