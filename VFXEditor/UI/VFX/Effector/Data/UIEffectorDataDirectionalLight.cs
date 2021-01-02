using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIEffectorDataDirectionalLight : UIBase
    {
        public AVFXEffectorDataDirectionalLight Data;
        //==========================

        public UIEffectorDataDirectionalLight( AVFXEffectorDataDirectionalLight data )
        {
            Data = data;
            //=======================
            Attributes.Add( new UICurveColor( Data.Ambient, "Ambient" ) );
            Attributes.Add( new UICurveColor( Data.Color, "Color" ) );
            Attributes.Add( new UICurve( Data.Power, "Power" ) );
            Attributes.Add( new UICurve( Data.Rotation, "Rotation" ) );
        }

        public override void Draw( string parentId )
        {
            string id = parentId + "/Data";
            if( ImGui.TreeNode( "Data" + id ) )
            {
                DrawAttrs( id );
                ImGui.TreePop();
            }
        }
    }
}
