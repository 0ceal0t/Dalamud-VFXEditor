using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIParticle : UIBase
    {
        public AVFXParticle Particle;
        public UIParticleView View;
        public int Idx;
        // =======================
        List<UIBase> Animation;
        //==========================
        public UICombo<ParticleType> Type;
        public List<UIParticleUVSet> UVSets;
        //==========================
        public UIBase Data;
        //========================
        List<UIBase> Tex;

        public UIParticle(AVFXParticle particle, UIParticleView view)
        {
            Particle = particle;
            View = view;
            Init();
        }
        public override void Init()
        {
            base.Init();
            // =======================
            Animation = new List<UIBase>();
            Tex = new List<UIBase>();
            UVSets = new List<UIParticleUVSet>();
            //==========================
            Type = new UICombo<ParticleType>("Type", Particle.ParticleVariety, changeFunction: ChangeType);
            Attributes.Add(new UIInt("Loop Start", Particle.LoopStart));
            Attributes.Add(new UIInt("Loop End", Particle.LoopEnd));
            Attributes.Add(new UICheckbox("Use Simple Animation", Particle.SimpleAnimEnable));
            Attributes.Add(new UICombo<RotationDirectionBase>("Rotation Direction Base", Particle.RotationDirectionBase));
            Attributes.Add(new UICombo<RotationOrder>("Rotation Compute Order", Particle.RotationOrder));
            Attributes.Add(new UICombo<CoordComputeOrder>("Coord Compute Order", Particle.CoordComputeOrder));
            Attributes.Add(new UICombo<DrawMode>("Draw Mode", Particle.DrawMode));
            Attributes.Add(new UICombo<CullingType>("Culling Type", Particle.CullingType));
            Attributes.Add(new UICombo<EnvLight>("Enviornmental Light", Particle.EnvLight));
            Attributes.Add(new UICombo<DirLight>("Directional Light", Particle.DirLight));
            Attributes.Add(new UICombo<UVPrecision>("UV Precision", Particle.UvPrecision));
            Attributes.Add(new UIInt("Draw Priority", Particle.DrawPriority));
            Attributes.Add(new UICheckbox("Depth Test", Particle.IsDepthTest));
            Attributes.Add(new UICheckbox("Depth Write", Particle.IsDepthWrite));
            Attributes.Add(new UICheckbox("Soft Particle", Particle.IsSoftParticle));
            Attributes.Add(new UIInt("Collision Type", Particle.CollisionType));
            Attributes.Add(new UICheckbox("BS11", Particle.Bs11));
            Attributes.Add(new UICheckbox("Apply Tone Map", Particle.IsApplyToneMap));
            Attributes.Add(new UICheckbox("Apply Fog", Particle.IsApplyFog));
            Attributes.Add(new UICheckbox("Clip Near", Particle.ClipNearEnable));
            Attributes.Add(new UICheckbox("Clip Far", Particle.ClipFarEnable));
            Attributes.Add(new UIFloat2("Clip Near", Particle.ClipNearStart, Particle.ClipNearEnd));
            Attributes.Add(new UIFloat2("Clip Far", Particle.ClipFarStart, Particle.ClipFarEnd));
            Attributes.Add(new UICombo<ClipBasePoint>("Clip Base Point", Particle.ClipBasePoint));
            Attributes.Add(new UIInt("Apply Rate Env", Particle.ApplyRateEnvironment));
            Attributes.Add(new UIInt("Apply Rate Directional", Particle.ApplyRateDirectional));
            Attributes.Add(new UIInt("Apply Rate Light Buffer", Particle.ApplyRateLightBuffer));
            Attributes.Add(new UICheckbox("DOTy", Particle.DOTy));
            Attributes.Add(new UIFloat("Apply Rate Light Buffer", Particle.DepthOffset));
            //==============================
            Animation.Add(new UILife(Particle.Life));
            Animation.Add(new UIParticleSimple(Particle.Simple));
            Animation.Add(new UICurve(Particle.Gravity, "Gravity"));
            Animation.Add(new UICurve(Particle.GravityRandom, "Gravity Random"));
            Animation.Add(new UICurve(Particle.AirResistance, "Air Resistance"));
            Animation.Add(new UICurve(Particle.AirResistanceRandom, "Air Resistance Random"));
            Animation.Add(new UICurve3Axis(Particle.Scale, "Scale"));
            Animation.Add(new UICurve3Axis(Particle.Rotation, "Rotation"));
            Animation.Add(new UICurve3Axis(Particle.Position, "Position"));
            Animation.Add(new UICurveColor(Particle.Color, "Color"));
            //===============================
            foreach(var uvSet in Particle.UVSets)
            {
                UVSets.Add(new UIParticleUVSet(uvSet, this));
            }
            //===============================
            switch (Particle.ParticleVariety.Value)
            {
                case ParticleType.Model:
                    Data = new UIParticleDataModel((AVFXParticleDataModel)Particle.Data);
                    break;
                case ParticleType.LightModel:
                    Data = new UIParticleDataLightModel((AVFXParticleDataLightModel)Particle.Data);
                    break;
                case ParticleType.Powder:
                    Data = new UIParticleDataPowder((AVFXParticleDataPowder)Particle.Data);
                    break;
                case ParticleType.Decal:
                    Data = new UIParticleDataDecal((AVFXParticleDataDecal)Particle.Data);
                    break;
                case ParticleType.DecalRing:
                    Data = new UIParticleDataDecalRing((AVFXParticleDataDecalRing)Particle.Data);
                    break;
                case ParticleType.Disc:
                    Data = new UIParticleDataDisc((AVFXParticleDataDisc)Particle.Data);
                    break;
                case ParticleType.Laser:
                    Data = new UIParticleDataLaser((AVFXParticleDataLaser)Particle.Data);
                    break;
                case ParticleType.Polygon:
                    Data = new UIParticleDataPolygon((AVFXParticleDataPolygon)Particle.Data);
                    break;
                case ParticleType.Polyline:
                    Data = new UIParticleDataPolyline((AVFXParticleDataPolyline)Particle.Data);
                    break;
                case ParticleType.Windmill:
                    Data = new UIParticleDataWindmill((AVFXParticleDataWindmill)Particle.Data);
                    break;
                case ParticleType.Line:
                    Data = new UIParticleDataLine( ( AVFXParticleDataLine )Particle.Data );
                    break;
                default:
                    Data = null;
                    break;
            }
            //============================
            Tex.Add(new UITextureColor1(Particle.TC1));
            Tex.Add(new UITextureColor2(Particle.TC2, "Texture Color 2"));
            Tex.Add(new UITextureColor2(Particle.TC3, "Texture Color 3"));
            Tex.Add(new UITextureColor2(Particle.TC4, "Texture Color 4"));
            Tex.Add(new UITextureNormal(Particle.TN));
            Tex.Add(new UITextureReflection(Particle.TR));
            Tex.Add(new UITextureDistortion(Particle.TD));
            Tex.Add(new UITexturePalette(Particle.TP));
        }
        public void ChangeType(LiteralEnum<ParticleType> literal)
        {
            Particle.SetVariety(literal.Value);
            Init();
        }

        public override void Draw(string parentId)
        {
            string id = parentId + "/Particle" + Idx;
            if (ImGui.CollapsingHeader("Particle " + Idx + "(" + Particle.ParticleVariety.stringValue() + ")" + id))
            {
                if (UIUtils.RemoveButton("Delete" + id))
                {
                    View.AVFX.removeParticle(Idx);
                    View.Init();
                }
                Type.Draw(id);
                //=====================
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
                //=====================
                if (ImGui.TreeNode("UV Sets (" + UVSets.Count + ")" + id))
                {
                    int uvIdx = 0;
                    foreach (var uv in UVSets)
                    {
                        uv.Idx = uvIdx;
                        uv.Draw(id);
                        uvIdx++;
                    }

                    if (UVSets.Count < 4)
                    {
                        if (ImGui.Button("+ UVSet" + id))
                        {
                            Particle.addUvSet();
                            Init();
                        }
                    }
                    ImGui.TreePop();
                }
                //====================
                if(Data != null)
                {
                    Data.Draw(id);
                }
                //===================
                DrawList(Tex, id);
            }
        }
    }
}
