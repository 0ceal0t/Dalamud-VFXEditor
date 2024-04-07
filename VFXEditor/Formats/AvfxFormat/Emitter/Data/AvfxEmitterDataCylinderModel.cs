using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxEmitterDataCylinderModel : AvfxDataWithParameters {
        public readonly AvfxEnum<RotationOrder> RotationOrderType = new( "Rotation Order", "ROT" );
        public readonly AvfxEnum<GenerateMethod> GenerateMethodType = new( "Generate Method", "GeMT" );
        public readonly AvfxInt DivideX = new( "Divide X", "DivX", value: 1 );
        public readonly AvfxInt DivideY = new( "Divide Y", "DivY", value: 1 );
        public readonly AvfxCurve Length = new( "Length", "Len" );
        public readonly AvfxCurve Radius = new( "Radius", "Rad" );
        public readonly AvfxCurve AX = new( "Angle X", "AnX", CurveType.Angle );
        public readonly AvfxCurve AY = new( "Angle Y", "AnY", CurveType.Angle );
        public readonly AvfxCurve InjectionSpeed = new( "Injection Speed", "IjS" );
        public readonly AvfxCurve InjectionSpeedRandom = new( "Injection Speed Random", "IjSR" );

        public AvfxEmitterDataCylinderModel() : base() {
            Parsed = [
                RotationOrderType,
                GenerateMethodType,
                DivideX,
                DivideY,
                Length,
                Radius,
                AX,
                AY,
                InjectionSpeed,
                InjectionSpeedRandom
            ];

            ParameterTab.Add( RotationOrderType );
            ParameterTab.Add( GenerateMethodType );
            ParameterTab.Add( DivideX );
            ParameterTab.Add( DivideY );

            DisplayTabs.Add( Radius );
            DisplayTabs.Add( Length );
            DisplayTabs.Add( AX );
            DisplayTabs.Add( AY );
            DisplayTabs.Add( InjectionSpeed );
            DisplayTabs.Add( InjectionSpeedRandom );
        }
    }
}
