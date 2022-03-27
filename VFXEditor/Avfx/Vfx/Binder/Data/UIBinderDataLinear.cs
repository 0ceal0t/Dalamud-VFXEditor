using AVFXLib.Models;
using System;

namespace VFXEditor.Avfx.Vfx {
    public class UIBinderDataLinear : UIData {
        public UIBinderDataLinear( AVFXBinderDataLinear data ) {
            Tabs.Add( new UICurve( data.CarryOverFactor, "Carry Over Factor" ) );
            Tabs.Add( new UICurve( data.CarryOverFactorRandom, "Carry Over Factor Random" ) );
        }
    }
}
