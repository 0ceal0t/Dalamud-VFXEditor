using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.Avfx.Vfx {
    public class UIBinderDataCamera : UIData {
        public AVFXBinderDataCamera Data;

        public UIBinderDataCamera( AVFXBinderDataCamera data ) {
            Data = data;
            //==================
            Tabs.Add( new UICurve( data.Distance, "Distance" ) );
            Tabs.Add( new UICurve( data.DistanceRandom, "Distance Random" ) );
        }
    }
}
