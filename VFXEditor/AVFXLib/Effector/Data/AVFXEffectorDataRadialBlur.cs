using System.Collections.Generic;
using VFXEditor.AVFXLib.Curve;

namespace VFXEditor.AVFXLib.Effector {
    public class AVFXEffectorDataRadialBlur : AVFXGenericData {
        public readonly AVFXCurve Length = new( "Len" );
        public readonly AVFXCurve Strength = new( "Str" );
        public readonly AVFXCurve Gradation = new( "Gra" );
        public readonly AVFXCurve InnerRadius = new( "IRad" );
        public readonly AVFXCurve OuterRadius = new( "ORad" );
        public readonly AVFXFloat FadeStartDistance = new( "FSDc" );
        public readonly AVFXFloat FadeEndDistance = new( "FEDc" );
        public readonly AVFXEnum<ClipBasePoint> FadeBasePointType = new( "FaBP" );

        public AVFXEffectorDataRadialBlur() : base() {
            Children = new List<AVFXBase> {
                Length,
                Strength,
                Gradation,
                InnerRadius,
                OuterRadius,
                FadeStartDistance,
                FadeEndDistance,
                FadeBasePointType
            };
        }
    }
}
