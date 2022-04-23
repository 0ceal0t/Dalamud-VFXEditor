using VFXEditor.AVFXLib;
using VFXEditor.AVFXLib.Particle;

namespace VFXEditor.AVFX.VFX {
    public class UIParticleDataPolyline : UIData {
        public UIParameters Parameters;

        public UIParticleDataPolyline( AVFXParticleDataPolyline data ) {
            Tabs.Add( Parameters = new UIParameters( "Parameters" ) );
            Parameters.Add( new UICombo<LineCreateType>( "Create Line Type", data.CreateLineType ) );
            Parameters.Add( new UICombo<NotBillboardBaseAxisType>( "Not Billboard Base Axis", data.NotBillBoardBaseAxisType ) );
            Parameters.Add( new UIInt( "Bind Weapon Type", data.BindWeaponType ) );
            Parameters.Add( new UIInt( "Point Count", data.PointCount ) );
            Parameters.Add( new UIInt( "Point Count Center", data.PointCountCenter ) );
            Parameters.Add( new UIInt( "Point Count End Distortion", data.PointCountEndDistortion ) );
            Parameters.Add( new UICheckbox( "Use Edge", data.UseEdge ) );
            Parameters.Add( new UICheckbox( "No Billboard", data.NotBillboard ) );
            Parameters.Add( new UICheckbox( "Bind Weapon", data.BindWeapon ) );
            Parameters.Add( new UICheckbox( "Connect Target", data.ConnectTarget ) );
            Parameters.Add( new UICheckbox( "Connect Target Reverse", data.ConnectTargetReverse ) );
            Parameters.Add( new UIInt( "Tag Number", data.TagNumber ) );
            Parameters.Add( new UICheckbox( "Is Spline", data.IsSpline ) );
            Parameters.Add( new UICheckbox( "Is Local", data.IsLocal ) );

            Tabs.Add( new UICurve( data.Width, "Width" ) );
            Tabs.Add( new UICurve( data.WidthBegin, "Width Begin" ) );
            Tabs.Add( new UICurve( data.WidthCenter, "Width Center" ) );
            Tabs.Add( new UICurve( data.WidthEnd, "Width End" ) );
            Tabs.Add( new UICurve( data.Length, "Length" ) );
            Tabs.Add( new UICurve( data.LengthRandom, "Length Random" ) );

            Tabs.Add( new UICurveColor( data.ColorBegin, "Color Begin" ) );
            Tabs.Add( new UICurveColor( data.ColorCenter, "Color Center" ) );
            Tabs.Add( new UICurveColor( data.ColorEnd, "Color End" ) );
            Tabs.Add( new UICurveColor( data.ColorEdgeBegin, "Color Edge Begin" ) );
            Tabs.Add( new UICurveColor( data.ColorEdgeCenter, "Color Edge Center" ) );
            Tabs.Add( new UICurveColor( data.ColorEdgeEnd, "Color Edge End" ) );

            Tabs.Add( new UICurve( data.CF, "CF (Unknown)" ) );
            Tabs.Add( new UICurve( data.Softness, "Softness" ) );
            Tabs.Add( new UICurve( data.SoftRandom, "Softness Random" ) );
            Tabs.Add( new UICurve( data.PnDs, "PnDs (Unknown)" ) );
        }
    }
}
