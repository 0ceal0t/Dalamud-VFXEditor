using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXTimelineSubItem : Base
    {
        public LiteralBool Enabled = new LiteralBool("enabled", "bEna");
        public LiteralInt StartTime = new LiteralInt("startTime", "StTm");
        public LiteralInt EndTime = new LiteralInt("endTime", "EdTm");
        public LiteralInt BinderIdx = new LiteralInt("binderIdx", "BdNo");
        public LiteralInt EffectorIdx = new LiteralInt("effectorIdx", "EfNo");
        public LiteralInt EmitterIdx = new LiteralInt("emitterIdx", "EmNo");
        public LiteralInt Platform = new LiteralInt("platform", "Plfm");
        public LiteralInt ClipNumber = new LiteralInt("clipIdx", "ClNo");

        List<Base> Attributes;

        public AVFXTimelineSubItem() : base("subItem", "SubItem")
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

        public override void toDefault()
        {
            SetDefault(Attributes);
            EndTime.GiveValue(1);
            BinderIdx.GiveValue(-1);
            EffectorIdx.GiveValue(-1);
            EmitterIdx.GiveValue(-1);
        }

        public override void read(AVFXNode node)
        {
            Assigned = true;
            ReadAVFX(Attributes, node);
        }

        public override JToken toJSON()
        {
            JObject elem = new JObject();
            PutJSON(elem, Attributes);
            return elem;
        }

        public override AVFXNode toAVFX()
        {
            AVFXNode dataAvfx = new AVFXNode("SubItem");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
