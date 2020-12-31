using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIEmitter : UIBase
    {
        public AVFXEmitter Emitter;
        public UIEmitterView View;
        public int Idx;
        //========================
        // TODO: sound
        //=======================
        public UICombo<EmitterType> Type;
        List<UIBase> Animation;
        //========================
        public List<UIEmitterItem> Particles;
        public List<UIEmitterItem> Emitters;
        //========================
        public UIBase Data;

        public UIEmitter(AVFXEmitter emitter, UIEmitterView view)
        {
            Emitter = emitter;
            View = view;
            Init();
        }
        public override void Init()
        {
            base.Init();
            // =====================
            Animation = new List<UIBase>();
            Particles = new List<UIEmitterItem>();
            Emitters = new List<UIEmitterItem>();
            //======================
            Type = new UICombo<EmitterType>("Type", Emitter.EmitterVariety, changeFunction: ChangeType);
            Attributes.Add(new UIInt("Sound Index", Emitter.SoundNumber));
            Attributes.Add(new UIInt("Loop Start", Emitter.LoopStart));
            Attributes.Add(new UIInt("Loop End", Emitter.LoopEnd));
            Attributes.Add(new UIInt("Child Limit", Emitter.ChildLimit));
            Attributes.Add(new UIInt("Effector Index", Emitter.EffectorIdx));
            Attributes.Add(new UICheckbox("Any Direction", Emitter.AnyDirection));
            Attributes.Add(new UICombo<RotationDirectionBase>("Rotation Direction Base", Emitter.RotationDirectionBaseType));
            Attributes.Add(new UICombo<CoordComputeOrder>("Coordinate Compute Order", Emitter.CoordComputeOrderType));
            Attributes.Add(new UICombo<RotationOrder>("Rotation Order", Emitter.RotationOrderType));
            // ==========================
            Animation.Add(new UILife(Emitter.Life));
            Animation.Add(new UICurve(Emitter.CreateCount, "Create Count"));
            Animation.Add(new UICurve(Emitter.CreateInterval, "Create Interval"));
            Animation.Add(new UICurve(Emitter.CreateIntervalRandom, "Create Interval Random"));
            Animation.Add(new UICurve(Emitter.Gravity, "Gravity"));
            Animation.Add(new UICurve(Emitter.GravityRandom, "Gravity Random"));
            Animation.Add(new UICurve(Emitter.AirResistance, "Air Resistance"));
            Animation.Add(new UICurve(Emitter.AirResistanceRandom, "Air Resistance Random"));
            Animation.Add(new UICurveColor(Emitter.Color, "Color"));
            Animation.Add(new UICurve3Axis(Emitter.Position, "Position"));
            Animation.Add(new UICurve3Axis(Emitter.Rotation, "Rotation"));
            Animation.Add(new UICurve3Axis(Emitter.Scale, "Scale"));
            //========================
            foreach(var particle in Emitter.Particles)
            {
                Particles.Add(new UIEmitterItem(particle, true, this));
            }
            //============================
            foreach (var emitter in Emitter.Emitters)
            {
                Emitters.Add(new UIEmitterItem(emitter, false, this));
            }
            //=======================
            switch (Emitter.EmitterVariety.Value)
            {
                case EmitterType.Point:
                    Data = null;
                    break;
                case EmitterType.SphereModel:
                    Data = new UIEmitterDataSphereModel((AVFXEmitterDataSphereModel)Emitter.Data);
                    break;
                case EmitterType.CylinderModel:
                    Data = new UIEmitterDataCylinderModel((AVFXEmitterDataCylinderModel)Emitter.Data);
                    break;
                case EmitterType.Model:
                    Data = new UIEmitterDataModel((AVFXEmitterDataModel)Emitter.Data);
                    break;
                default:
                    Data = null;
                    break;
            }
        }
        public void ChangeType(LiteralEnum<EmitterType> literal)
        {
            Emitter.SetVariety(literal.Value);
            Init();
        }

        public override void Draw(string parentId)
        {
            string id = parentId + "/Emitter" + Idx;
            if (ImGui.CollapsingHeader("Emitter " + Idx + "(" + Emitter.EmitterVariety.stringValue() + ")" + id))
            {
                if (UIUtils.RemoveButton("Delete" + id))
                {
                    View.AVFX.removeEmitter(Idx);
                    View.Init();
                }
                Type.Draw(id);
                //==========================
                if (ImGui.TreeNode("Parameters" + id))
                {
                    DrawAttrs(id);
                    ImGui.TreePop();
                }
                //======================
                if (ImGui.TreeNode("Animation" + id))
                {
                    DrawList(Animation, id);
                    ImGui.TreePop();
                }
                //=======================
                if (ImGui.TreeNode("Particles (" + Particles.Count + ")" + id))
                {
                    int pIdx = 0;
                    foreach (var particle in Particles)
                    {
                        particle.Idx = pIdx;
                        particle.Draw(id);
                        pIdx++;
                    }
                    if (ImGui.Button("+ Particle" + id))
                    {
                        Emitter.addParticle();
                        Init();
                    }
                    ImGui.TreePop();
                }
                //=======================
                if (ImGui.TreeNode("Emitters (" + Emitters.Count + ")" + id))
                {
                    int eIdx = 0;
                    foreach (var emitter in Emitters)
                    {
                        emitter.Idx = eIdx;
                        emitter.Draw(id);
                        eIdx++;
                    }
                    if (ImGui.Button("+ Emitter" + id))
                    {
                        Emitter.addEmitter();
                        Init();
                    }
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
