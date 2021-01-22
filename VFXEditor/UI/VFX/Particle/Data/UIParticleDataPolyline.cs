using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIParticleDataPolyline : UIData {
        public AVFXParticleDataPolyline Data;
        public List<UIBase> Attributes = new List<UIBase>();
        //==========================

        public UIParticleDataPolyline(AVFXParticleDataPolyline data)
        {
            Data = data;
            //=======================
            Attributes.Add( new UICombo<LineCreateType>( "Create Line Type", Data.CreateLineType ) );
            Attributes.Add( new UICombo<NotBillboardBaseAxisType>( "Not Billboard Base Axis", Data.NotBillBoardBaseAxisType ) );
            Attributes.Add( new UIInt( "Bind Weapon Type", Data.BindWeaponType ) );
            Attributes.Add( new UIInt( "Point Count", Data.PointCount ) );
            Attributes.Add( new UIInt( "Point Count Center", Data.PointCountCenter ) );
            Attributes.Add( new UIInt( "Point Count End Distortion", Data.PointCountEndDistortion ) );
            Attributes.Add( new UICheckbox( "Use Edge", Data.UseEdge ) );
            Attributes.Add( new UICheckbox( "No Billboard", Data.NotBillboard ) );
            Attributes.Add( new UICheckbox( "Bind Weapon", Data.BindWeapon ) );
            Attributes.Add( new UICheckbox( "Connect Target", Data.ConnectTarget ) );
            Attributes.Add( new UICheckbox( "Connect Target Reverse", Data.ConnectTargetReverse ) );
            Attributes.Add( new UIInt( "Tag Number", Data.TagNumber ) );
            Attributes.Add( new UICheckbox( "Is Spline", Data.IsSpline ) );
            Attributes.Add( new UICheckbox( "Is Local", Data.IsLocal ) );
            Attributes.Add( new UICurve( Data.CF, "CF (Unknown)" ) );
            Attributes.Add( new UICurve( Data.Width, "Width" ) );
            Attributes.Add( new UICurve( Data.WidthBegin, "Width Begin" ) );
            Attributes.Add( new UICurve( Data.WidthCenter, "Width Center" ) );
            Attributes.Add( new UICurve( Data.WidthEnd, "Width End" ) );
            Attributes.Add( new UICurve( Data.Softness, "Softness" ) );
            Attributes.Add( new UICurve( Data.SoftRandom, "Softness Random" ) );
            Attributes.Add( new UICurve( Data.PnDs, "PnDs (Unknown)" ) );
            Attributes.Add( new UICurve( Data.Length, "Length" ) );
            Attributes.Add( new UICurve( Data.LengthRandom, "Length Random" ) );
            Attributes.Add( new UICurveColor( Data.ColorBegin, "Color Begin" ) );
            Attributes.Add( new UICurveColor( Data.ColorCenter, "Color Center" ) );
            Attributes.Add( new UICurveColor( Data.ColorEnd, "Color End" ) );
            Attributes.Add( new UICurveColor( Data.ColorEdgeBegin, "Color Edge Begin" ) );
            Attributes.Add( new UICurveColor( Data.ColorEdgeCenter, "Color Edge Center" ) );
            Attributes.Add( new UICurveColor( Data.ColorEdgeEnd, "Color Edge End" ) );
        }

        public override void Draw(string parentId)
        {
            string id = parentId + "/Data";
            DrawList( Attributes, id );
        }
    }
}
