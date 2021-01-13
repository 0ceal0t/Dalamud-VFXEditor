using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIParticleDataLine : UIBase
    {
        public AVFXParticleDataLine Data;
        //==========================

        public UIParticleDataLine( AVFXParticleDataLine data )
        {
            Data = data;
            //=======================
            Attributes.Add( new UIInt( "Line Count", Data.LineCount ) );
            Attributes.Add( new UICurve( Data.Length, "Length" ) );
            Attributes.Add( new UICurveColor( Data.ColorBegin, "Color Begin" ) );
            Attributes.Add( new UICurveColor( Data.ColorEnd, "Color End" ) );
        }

        public override void Draw( string parentId )
        {
            string id = parentId + "/Data";
            DrawAttrs( id );
        }
    }
}
