using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{

    public class AVFXScheduleSubItem : Base
    {
        public LiteralBool Enabled = new LiteralBool("enabled", "bEna");
        public LiteralInt StartTime = new LiteralInt("startTime", "StTm");
        public LiteralInt TimelineIdx = new LiteralInt("timelineIdx", "TlNo");

        public List<Base> Attributes;

        public AVFXScheduleSubItem() : base("subItem", "SubItem")
        {
            Attributes = new List<Base>(new Base[]{
                Enabled,
                StartTime,
                TimelineIdx
            });
        }

        public override void toDefault()
        {
            Enabled.GiveValue(true);
            StartTime.GiveValue(0);
            TimelineIdx.GiveValue(-1);
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
