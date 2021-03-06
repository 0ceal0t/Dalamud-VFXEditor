using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIModelEmitterVertex : UIItem
    {
        public UIModel Model;
        public VNum VertNumber;
        public EmitVertex Vertex;

        public int Order;
        public Vector3 Position;
        public Vector3 Normal;
        public Vector4 Color;
        public UIModelEmitterVertex( VNum vnum, EmitVertex emitVertex, UIModel model )
        {
            Model = model;
            VertNumber = vnum;
            Vertex = emitVertex;
            //=======================
            Order = VertNumber.Num;
            Position = new Vector3( Vertex.Position[0], Vertex.Position[1], Vertex.Position[2] );
            Normal = new Vector3( Vertex.Normal[0], Vertex.Normal[1], Vertex.Normal[2] );
            Color = UIUtils.IntToColor( Vertex.C ) / 255;
        }

        public override void DrawBody( string parentId )
        {
            string id = parentId + "/VNum";
            if(ImGui.InputInt("Order" + id, ref Order ) )
            {
                VertNumber.Num = Order;
            }
            if(ImGui.InputFloat3("Position" + id, ref Position ) )
            {
                Vertex.Position = new float[] { Position.X, Position.Y, Position.Z };
            }
            if( ImGui.InputFloat3( "Normal" + id, ref Position ) )
            {
                Vertex.Normal = new float[] { Normal.X, Normal.Y, Normal.Z };
            }
            if(ImGui.ColorEdit4("Color" + id, ref Color ) )
            {
                Vertex.C = UIUtils.ColorToInt( Color * 255 );
            }
        }

        public override string GetText() {
            return "" + Idx;
        }
    }
}
