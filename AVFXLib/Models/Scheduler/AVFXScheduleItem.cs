using AVFXLib.AVFX;
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

        public override AVFXNode ToAVFX()
        {
            AVFXNode res = base.ToAVFX();
            res.Name = "Item";
            return res;
        }
    }
}
