using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIModel : UIBase
    {
        public AVFXModel Model;
        //=======================

        public UIModel(AVFXModel model)
        {
            Model = model;
            //=======================
        }

        // ============== DRAW ===============
        public override void Draw( string parentId )
        {
        }
        public override void DrawSelect( string parentId, ref UIBase selected )
        {
            if( !Assigned )
            {
                return;
            }
            if( ImGui.Selectable( "Model " + Idx + parentId, selected == this ) )
            {
                selected = this;
            }
        }
        public override void DrawBody( string parentId )
        {
            ImGui.Text( "Vertices: " + Model.Vertices.Count );
            ImGui.Text( "Indexes: " + Model.Indexes.Count );
            ImGui.Text( "Emitter Vertices: " + Model.EmitVertices.Count );
            ImGui.Text( "Emitter Vertex Order: " + Model.VNums.Count );
        }
    }
}
