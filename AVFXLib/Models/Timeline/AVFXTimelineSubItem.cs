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
        public LiteralBool Enabled = new("bEna");
        public LiteralInt StartTime = new("StTm");
        public LiteralInt EndTime = new("EdTm");
        public LiteralInt BinderIdx = new("BdNo");
        public LiteralInt EffectorIdx = new("EfNo");
        public LiteralInt EmitterIdx = new("EmNo");
        public LiteralInt Platform = new("Plfm");
        public LiteralInt ClipNumber = new("ClNo");
        readonly List<Base> Attributes;

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
            var dataAvfx = new AVFXNode("SubItem");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
