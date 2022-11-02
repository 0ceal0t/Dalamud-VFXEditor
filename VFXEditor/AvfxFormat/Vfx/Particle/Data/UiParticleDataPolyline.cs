using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Particle;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiParticleDataPolyline : UiData {
        public UiParameters Parameters;

        public UiParticleDataPolyline( AVFXParticleDataPolyline data ) {
            Tabs.Add( Parameters = new UiParameters( "Parameters" ) );
            Parameters.Add( new UiCombo<LineCreateType>( "Create Line Type", data.CreateLineType ) );
            Parameters.Add( new UiCombo<NotBillboardBaseAxisType>( "Not Billboard Base Axis", data.NotBillBoardBaseAxisType ) );
            Parameters.Add( new UiInt( "Bind Weapon Type", data.BindWeaponType ) );
            Parameters.Add( new UiInt( "Point Count", data.PointCount ) );
            Parameters.Add( new UiInt( "Point Count Center", data.PointCountCenter ) );
            Parameters.Add( new UiInt( "Point Count End Distortion", data.PointCountEndDistortion ) );
            Parameters.Add( new UiCheckbox( "Use Edge", data.UseEdge ) );
            Parameters.Add( new UiCheckbox( "No Billboard", data.NotBillboard ) );
            Parameters.Add( new UiCheckbox( "Bind Weapon", data.BindWeapon ) );
            Parameters.Add( new UiCheckbox( "Connect Target", data.ConnectTarget ) );
            Parameters.Add( new UiCheckbox( "Connect Target Reverse", data.ConnectTargetReverse ) );
            Parameters.Add( new UiInt( "Tag Number", data.TagNumber ) );
            Parameters.Add( new UiCheckbox( "Is Spline", data.IsSpline ) );
            Parameters.Add( new UiCheckbox( "Is Local", data.IsLocal ) );

            Tabs.Add( new UiCurve( data.Width, "Width" ) );
            Tabs.Add( new UiCurve( data.WidthBegin, "Width Begin" ) );
            Tabs.Add( new UiCurve( data.WidthCenter, "Width Center" ) );
            Tabs.Add( new UiCurve( data.WidthEnd, "Width End" ) );
            Tabs.Add( new UiCurve( data.Length, "Length" ) );
            Tabs.Add( new UiCurve( data.LengthRandom, "Length Random" ) );

            Tabs.Add( new UiCurveColor( data.ColorBegin, "Color Begin" ) );
            Tabs.Add( new UiCurveColor( data.ColorCenter, "Color Center" ) );
            Tabs.Add( new UiCurveColor( data.ColorEnd, "Color End" ) );
            Tabs.Add( new UiCurveColor( data.ColorEdgeBegin, "Color Edge Begin" ) );
            Tabs.Add( new UiCurveColor( data.ColorEdgeCenter, "Color Edge Center" ) );
            Tabs.Add( new UiCurveColor( data.ColorEdgeEnd, "Color Edge End" ) );

            Tabs.Add( new UiCurve( data.CF, "CF (Unknown)" ) );
            Tabs.Add( new UiCurve( data.Softness, "Softness" ) );
            Tabs.Add( new UiCurve( data.SoftRandom, "Softness Random" ) );
            Tabs.Add( new UiCurve( data.PnDs, "PnDs (Unknown)" ) );
        }
    }
}
