using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIEffector : UIBase
    {
        public AVFXEffector Effector;
        public UIEffectorView View;
        public int Idx;
        //========================
        public UICombo<EffectorType> Type;
        public UIBase Data;

        public UIEffector(AVFXEffector effector, UIEffectorView view)
        {
            Effector = effector;
            View = view;
            Init();
        }
        public override void Init()
        {
            base.Init();
            //======================
            Type = new UICombo<EffectorType>("Type", Effector.EffectorVariety, changeFunction: ChangeType);
            Attributes.Add(new UICombo<RotationOrder>("Rotation Order", Effector.RotationOrder));
            Attributes.Add(new UICombo<CoordComputeOrder>("Coordinate Compute Order", Effector.CoordComputeOrder));
            Attributes.Add(new UICheckbox("Affect Other VFX", Effector.AffectOtherVfx));
            Attributes.Add(new UICheckbox("Affect Game", Effector.AffectGame));
            Attributes.Add(new UIInt("Loop Start", Effector.LoopPointStart));
            Attributes.Add(new UIInt("Loop End", Effector.LoopPointEnd));
            //=======================
            switch (Effector.EffectorVariety.Value)
            {
                case EffectorType.PointLight:
                    Data = new UIEffectorDataPointLight((AVFXEffectorDataPointLight)Effector.Data);
                    break;
                case EffectorType.RadialBlur:
                    Data = new UIEffectorDataRadialBlur((AVFXEffectorDataRadialBlur)Effector.Data);
                    break;
                case EffectorType.CameraQuake:
                    Data = new UIEffectorDataCameraQuake((AVFXEffectorDataCameraQuake)Effector.Data);
                    break;
                case EffectorType.DirectionalLight:
                    Data = new UIEffectorDataDirectionalLight( ( AVFXEffectorDataDirectionalLight )Effector.Data );
                    break;
                default:
                    Data = null;
                    break;
            }
        }
        public void ChangeType(LiteralEnum<EffectorType> literal)
        {
            Effector.SetVariety(literal.Value);
            Init();
        }

        public override void Draw(string parentId)
        {
            string id = parentId + "/Effector" + Idx;
            if (ImGui.CollapsingHeader("Effector " + Idx + "(" + Effector.EffectorVariety.stringValue() + ")" + id))
            {
                if (UIUtils.RemoveButton("Delete" + id))
                {
                    View.AVFX.removeEffector(Idx);
                    View.Init();
                }
                Type.Draw(id);
                //==========================
                if (ImGui.TreeNode("Parameters" + id))
                {
                    DrawAttrs(id);
                    ImGui.TreePop();
                }
                //=============================
                if (Data != null)
                {
                    Data.Draw(id);
                }
            }
        }
    }
}
