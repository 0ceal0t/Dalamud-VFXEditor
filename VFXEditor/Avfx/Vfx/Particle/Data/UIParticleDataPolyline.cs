using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.Avfx.Vfx
{
    public class UIParticleDataPolyline : UIData {
        public AVFXParticleDataPolyline Data;
        public UIParameters Parameters;

        public UIParticleDataPolyline(AVFXParticleDataPolyline data)
        {
            Data = data;
            //=======================
            Tabs.Add( Parameters = new UIParameters( "Parameters" ) );
            Parameters.Add( new UICombo<LineCreateType>( "Create Line Type", Data.CreateLineType ) );
            Parameters.Add( new UICombo<NotBillboardBaseAxisType>( "Not Billboard Base Axis", Data.NotBillBoardBaseAxisType ) );
            Parameters.Add( new UIInt( "Bind Weapon Type", Data.BindWeaponType ) );
            Parameters.Add( new UIInt( "Point Count", Data.PointCount ) );
            Parameters.Add( new UIInt( "Point Count Center", Data.PointCountCenter ) );
            Parameters.Add( new UIInt( "Point Count End Distortion", Data.PointCountEndDistortion ) );
            Parameters.Add( new UICheckbox( "Use Edge", Data.UseEdge ) );
            Parameters.Add( new UICheckbox( "No Billboard", Data.NotBillboard ) );
            Parameters.Add( new UICheckbox( "Bind Weapon", Data.BindWeapon ) );
            Parameters.Add( new UICheckbox( "Connect Target", Data.ConnectTarget ) );
            Parameters.Add( new UICheckbox( "Connect Target Reverse", Data.ConnectTargetReverse ) );
            Parameters.Add( new UIInt( "Tag Number", Data.TagNumber ) );
            Parameters.Add( new UICheckbox( "Is Spline", Data.IsSpline ) );
            Parameters.Add( new UICheckbox( "Is Local", Data.IsLocal ) );

            Tabs.Add( new UICurve( Data.Width, "Width" ) );
            Tabs.Add( new UICurve( Data.WidthBegin, "Width Begin" ) );
            Tabs.Add( new UICurve( Data.WidthCenter, "Width Center" ) );
            Tabs.Add( new UICurve( Data.WidthEnd, "Width End" ) );
            Tabs.Add( new UICurve( Data.Length, "Length" ) );
            Tabs.Add( new UICurve( Data.LengthRandom, "Length Random" ) );

            Tabs.Add( new UICurveColor( Data.ColorBegin, "Color Begin" ) );
            Tabs.Add( new UICurveColor( Data.ColorCenter, "Color Center" ) );
            Tabs.Add( new UICurveColor( Data.ColorEnd, "Color End" ) );
            Tabs.Add( new UICurveColor( Data.ColorEdgeBegin, "Color Edge Begin" ) );
            Tabs.Add( new UICurveColor( Data.ColorEdgeCenter, "Color Edge Center" ) );
            Tabs.Add( new UICurveColor( Data.ColorEdgeEnd, "Color Edge End" ) );

            Tabs.Add( new UICurve( Data.CF, "CF (Unknown)" ) );
            Tabs.Add( new UICurve( Data.Softness, "Softness" ) );
            Tabs.Add( new UICurve( Data.SoftRandom, "Softness Random" ) );
            Tabs.Add( new UICurve( Data.PnDs, "PnDs (Unknown)" ) );
        }
    }
}
