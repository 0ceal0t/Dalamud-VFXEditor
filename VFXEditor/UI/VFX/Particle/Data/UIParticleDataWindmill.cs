using AVFXLib.Models;
using System.Collections.Generic;

namespace VFXEditor.UI.VFX
{
    public class UIParticleDataWindmill : UIData {
        public AVFXParticleDataWindmill Data;
        public List<UIBase> Attributes = new List<UIBase>();
        //==========================

        public UIParticleDataWindmill(AVFXParticleDataWindmill data)
        {
            Data = data;
            //=======================
            Attributes.Add( new UICombo<WindmillUVType>( "Windmill UV Type", Data.WindmillUVType ) );
        }

        public override void Draw(string parentId)
        {
            string id = parentId + "/Data";
            DrawList( Attributes, id );
        }
    }
}
