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
        public int Idx;
        //=======================

        public UIModel(AVFXModel model)
        {
            Model = model;
            //=======================
        }

        public override void Draw(string parentId)
        {
            string id = parentId + "/Model" + Idx;
            if (ImGui.CollapsingHeader("Model " + Idx + id))
            {
                ImGui.Text("Vertices: " + Model.Vertices.Count);
                ImGui.Text("Indexes: " + Model.Indexes.Count);
                ImGui.Text("Emitter Vertices: " + Model.EmitVertices.Count);
                ImGui.Text("Emitter Vertex Order: " + Model.VNums.Count);
            }
        }
    }
}
