using AVFXLib.Models;
using System.Collections.Generic;

namespace VFXEditor.Avfx.Vfx
{
    public class UIParticleDataWindmill : UIData {
        public AVFXParticleDataWindmill Data;
        public UIParameters Parameters;

        public UIParticleDataWindmill(AVFXParticleDataWindmill data) {
            Data = data;
            //=======================
            Tabs.Add( Parameters = new UIParameters( "Parameters" ) );
            Parameters.Add( new UICombo<WindmillUVType>( "Windmill UV Type", Data.WindmillUVType ) );
        }
    }
}
