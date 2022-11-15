using System;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxEmitterDataConeModel : AvfxData {
        public readonly AvfxEnum<RotationOrder> RotationOrderType = new( "Rotation Order", "ROT" );
        public readonly AvfxEnum<GenerateMethod> GenerateMethodType = new( "Generate Method", "GeMT" );
        public readonly AvfxInt DivideX = new( "Divide X", "DivX" );
        public readonly AvfxInt DivideY = new( "Divide Y", "DivY" );
        public readonly AvfxCurve AX = new( "Angle X", "AnX" );
        public readonly AvfxCurve AY = new( "Angle Y", "AnY" );
        public readonly AvfxCurve Radius = new( "Radius", "Rad" );
        public readonly AvfxCurve InjectionSpeed = new( "Injection Speed", "IjS" );
        public readonly AvfxCurve InjectionSpeedRandom = new( "Injection Speed Random", "IjSR" );
        public readonly AvfxCurve InjectionAngle = new( "Injection Angle", "IjA" );
        public readonly AvfxCurve InjectionAngleRandom = new( "Injection Angle Random", "IjAR" );

        public readonly UiDisplayList Display;

        public AvfxEmitterDataConeModel() : base() {
            Parsed = new() {
                RotationOrderType,
                GenerateMethodType,
                DivideX,
                DivideY,
                AX,
                AY,
                Radius,
                InjectionSpeed,
                InjectionSpeedRandom,
                InjectionAngle,
                InjectionAngleRandom
            };

            DisplayTabs.Add( Display = new UiDisplayList( "Parameters" ) );
            Display.Add( RotationOrderType );
            Display.Add( GenerateMethodType );
            Display.Add( DivideX );
            Display.Add( DivideY );
            DisplayTabs.Add( AX );
            DisplayTabs.Add( AY );
            DisplayTabs.Add( Radius );
            DisplayTabs.Add( InjectionSpeed );
            DisplayTabs.Add( InjectionSpeedRandom );
            DisplayTabs.Add( InjectionAngle );
            DisplayTabs.Add( InjectionAngleRandom );
        }
    }
}
