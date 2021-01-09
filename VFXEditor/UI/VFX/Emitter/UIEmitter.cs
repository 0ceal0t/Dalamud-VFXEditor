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
        //=======================
        public UICombo<EmitterType> Type;
        List<UIBase> Animation;
        UISplitView AnimationSplit;
        //========================
        public List<UIBase> Particles;
        public List<UIBase> Emitters;
        //========================
        public UIBase Data;
        //========================
        public UIEmitterSplitView EmitterSplit;
        public UIEmitterSplitView ParticleSplit;

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
            Particles = new List<UIBase>();
            Emitters = new List<UIBase>();
            //======================
            Type = new UICombo<EmitterType>("Type", Emitter.EmitterVariety, changeFunction: ChangeType);
            Attributes.Add( new UIString( "Sound", Emitter.Sound ) );
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
                case EmitterType.Cone:
                    Data = new UIEmitterDataCone( ( AVFXEmitterDataCone )Emitter.Data );
                    break;
                case EmitterType.ConeModel:
                    Data = new UIEmitterDataConeModel( ( AVFXEmitterDataConeModel )Emitter.Data );
                    break;
                default:
                    Data = null;
                    break;
            }
            //=============================
            AnimationSplit = new UISplitView( Animation );
            EmitterSplit = new UIEmitterSplitView( Emitters, this, false );
            ParticleSplit = new UIEmitterSplitView( Particles, this, true );
        }
        public void ChangeType(LiteralEnum<EmitterType> literal)
        {
            Emitter.SetVariety(literal.Value);
            Init();
            View.RefreshDesc( Idx );
        }

        public string GetDescText()
        {
            return "Emitter " + Idx + "(" + Emitter.EmitterVariety.stringValue() + ")";
        }

        public override void Draw(string parentId)
        {
            string id = parentId + "/Emitter" + Idx;
            Type.Draw(id);
            //==========================

            if( ImGui.BeginTabBar( id + "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) )
            {
                if( ImGui.BeginTabItem( "Parameters" + id ) )
                {
                    DrawParameters( id + "/Param" );
                    ImGui.EndTabItem();
                }
                if( Data != null && ImGui.BeginTabItem( "Data" + id ) )
                {
                    DrawData( id + "/Data" );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Animation" + id ) )
                {
                    DrawAnimation( id + "/Anim" );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Particles" + id ) )
                {
                    DrawParticles( id + "/ItPr" );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Emitters" + id ) )
                {
                    DrawEmitters( id + "/ItEm" );
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
        }

        private void DrawParameters( string id )
        {
            ImGui.BeginChild( id );
            DrawAttrs( id );
            ImGui.EndChild();
        }
        private void DrawData( string id )
        {
            ImGui.BeginChild( id );
            Data.Draw( id );
            ImGui.EndChild();
        }
        private void DrawAnimation( string id )
        {
            AnimationSplit.Draw( id );
        }
        private void DrawParticles( string id )
        {
            ParticleSplit.Draw( id );
        }
        private void DrawEmitters( string id )
        {
            EmitterSplit.Draw( id );
        }
    }
}
