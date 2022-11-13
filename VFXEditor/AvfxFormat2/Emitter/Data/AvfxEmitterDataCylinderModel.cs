using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VfxEditor.AvfxFormat2.Enums;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxEmitterDataCylinderModel : AvfxData {
        public readonly AvfxEnum<RotationOrder> RotationOrderType = new( "Rotation Order", "ROT" );
        public readonly AvfxEnum<GenerateMethod> GenerateMethodType = new( "Generate Method", "GeMT" );
        public readonly AvfxInt DivideX = new( "Divide X", "DivX" );
        public readonly AvfxInt DivideY = new( "Divide Y", "DivY" );
        public readonly AvfxCurve Length = new( "Length", "Len" );
        public readonly AvfxCurve Radius = new( "Radius", "Rad" );
        public readonly AvfxCurve InjectionSpeed = new( "Injection Speed", "IjS" );
        public readonly AvfxCurve InjectionSpeedRandom = new( "Injection Speed Random", "IjSR" );

        public readonly UiParameters Parameters;

        public AvfxEmitterDataCylinderModel() : base() {
            Children = new() {
                RotationOrderType,
                GenerateMethodType,
                DivideX,
                DivideY,
                Length,
                Radius,
                InjectionSpeed,
                InjectionSpeedRandom
            };
            DivideX.SetValue( 1 );
            DivideY.SetValue( 1 );

            Tabs.Add( Parameters = new UiParameters( "Parameters" ) );
            Parameters.Add( RotationOrderType );
            Parameters.Add( GenerateMethodType );
            Parameters.Add( DivideX );
            Parameters.Add( DivideY );
            Tabs.Add( Radius );
            Tabs.Add( Length );
            Tabs.Add( InjectionSpeed );
            Tabs.Add( InjectionSpeedRandom );
        }
    }
}
