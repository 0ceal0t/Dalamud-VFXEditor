using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXTimelineSubItem : Base
    {
        public LiteralBool Enabled = new LiteralBool("bEna");
        public LiteralInt StartTime = new LiteralInt("StTm");
        public LiteralInt EndTime = new LiteralInt("EdTm");
        public LiteralInt BinderIdx = new LiteralInt("BdNo");
        public LiteralInt EffectorIdx = new LiteralInt("EfNo");
        public LiteralInt EmitterIdx = new LiteralInt("EmNo");
        public LiteralInt Platform = new LiteralInt("Plfm");
        public LiteralInt ClipNumber = new LiteralInt("ClNo");

        List<Base> Attributes;

        public AVFXTimelineSubItem() : base("SubItem")
        {
            Attributes = new List<Base>(new Base[]{
                Enabled,
                StartTime,
                EndTime,
                BinderIdx,
                EffectorIdx,
                EmitterIdx,
                Platform,
                ClipNumber
            });
        }

        public override void ToDefault()
        {
            SetDefault(Attributes);
            EndTime.GiveValue(1);
            BinderIdx.GiveValue(-1);
            EffectorIdx.GiveValue(-1);
            EmitterIdx.GiveValue(-1);
        }

        public override void Read(AVFXNode node)
        {
            Assigned = true;
            ReadAVFX(Attributes, node);
        }

        public override AVFXNode ToAVFX()
        {
            AVFXNode dataAvfx = new AVFXNode("SubItem");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
