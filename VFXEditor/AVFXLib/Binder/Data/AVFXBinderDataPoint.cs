using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.AVFXLib.Curve;

namespace VFXEditor.AVFXLib.Binder {
    public class AVFXBinderDataPoint : AVFXGenericData {
        public readonly AVFXCurve SpringStrength = new( "SpS" );

        public AVFXBinderDataPoint() : base() {
            Children = new List<AVFXBase> {
                SpringStrength
            };
        }
    }
}
