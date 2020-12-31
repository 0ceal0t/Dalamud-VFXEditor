using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXScheduleItem : AVFXScheduleTrigger
    {
        public new const string NAME = "Item";

        public AVFXScheduleItem() : base()
        {
        }

        public override AVFXNode toAVFX()
        {
            AVFXNode res = base.toAVFX();
            res.Name = "Item";
            return res;
        }
    }
}
