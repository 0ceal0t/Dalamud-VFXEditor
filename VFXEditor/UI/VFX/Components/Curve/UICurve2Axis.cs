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
    public class UICurve2Axis : UIBase
    {
        public AVFXCurve2Axis Curve;
        public string Name;
        //=========================

        public UICurve2Axis(AVFXCurve2Axis curve, string name)
        {
            Curve = curve;
            Name = name;
            Init();
        }
        public override void Init()
        {
            base.Init();
            if (!Curve.Assigned) { Assigned = false; return; }
            // ======================
            Attributes.Add(new UICombo<AxisConnect>("Axis Connect", Curve.AxisConnectType));
            Attributes.Add(new UICombo<RandomType>("Axis Connect Random", Curve.AxisConnectRandomType));
            Attributes.Add(new UICurve(Curve.X, "X"));
            Attributes.Add(new UICurve(Curve.Y, "Y"));
            Attributes.Add(new UICurve(Curve.RX, "RX"));
            Attributes.Add(new UICurve(Curve.RY, "RY"));
        }

        public override void Draw(string parentId)
        {
            string id = parentId + "/" + Name;
            // ===== UNASSIGNED ====
            if (!Assigned)
            {
                if (ImGui.Button("+ " + Name + id))
                {
                    Curve.toDefault();
                    Init();
                }
                return;
            }
            // ===== ASSIGNED ===
            if (ImGui.TreeNode(Name + id))
            {
                if (UIUtils.RemoveButton("Delete" + id))
                {
                    Curve.Assigned = false;
                    Init();
                }
                DrawAttrs(id);
                ImGui.TreePop();
            }
        }
    }
}
