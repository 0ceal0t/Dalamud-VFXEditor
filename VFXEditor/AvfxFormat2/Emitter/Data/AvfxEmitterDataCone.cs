using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VfxEditor.AvfxFormat2.Enums;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxEmitterDataCone : AvfxData {
        public readonly AvfxEnum<RotationOrder> RotationOrderType = new( "Rotation Order", "ROT" );
        public readonly AvfxCurve AngleY = new( "Angle Y", "AnY" );
        public readonly AvfxCurve OuterSize = new( "Outer Size", "OuS" );
        public readonly AvfxCurve InjectionSpeed = new( "Injection Speed", "IjS" );
        public readonly AvfxCurve InjectionSpeedRandom = new( "Injection Speed Random", "IjSR" );
        public readonly AvfxCurve InjectionAngle = new( "Injection Angle", "IjA" );

        public readonly UiParameters Parameters;

        public AvfxEmitterDataCone() : base() {
            Children = new() {
                RotationOrderType,
                AngleY,
                OuterSize,
                InjectionSpeed,
                InjectionSpeedRandom,
                InjectionAngle
            };

            Tabs.Add( Parameters = new UiParameters( "Parameters" ) );
            Parameters.Add( RotationOrderType );
            Tabs.Add( AngleY );
            Tabs.Add( OuterSize );
            Tabs.Add( InjectionSpeed );
            Tabs.Add( InjectionSpeedRandom );
            Tabs.Add( InjectionAngle );
        }
    }
}
