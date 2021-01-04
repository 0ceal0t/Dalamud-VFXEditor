using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIParticleDataWindmill : UIBase
    {
        public AVFXParticleDataWindmill Data;
        //==========================

        public UIParticleDataWindmill(AVFXParticleDataWindmill data)
        {
            Data = data;
            Init();
        }
        public override void Init()
        {
            base.Init();
            //=======================
            Attributes.Add(new UICombo<WindmillUVType>("Windmill UV Type", Data.WindmillUVType));
        }

        public override void Draw(string parentId)
        {
            string id = parentId + "/Data";
            DrawAttrs( id );
        }
    }
}
