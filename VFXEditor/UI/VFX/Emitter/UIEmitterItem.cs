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
    public class UIEmitterItem : UIBase
    {
        public AVFXEmitterIterationItem Iteration;
        public UIEmitter Emitter;
        public bool IsParticle;
        //=============================

        public UIEmitterItem(AVFXEmitterIterationItem iteration, bool isParticle, UIEmitter emitter)
        {
            Iteration = iteration;
            Emitter = emitter;
            IsParticle = isParticle;
            Init();
        }
        public override void Init()
        {
            base.Init();
            //=========================
            Attributes.Add(new UICheckbox("Enabled", Iteration.Enabled));
            Attributes.Add(new UIInt("Target Index", Iteration.TargetIdx));
            Attributes.Add(new UIInt("Local Direction", Iteration.LocalDirection));
            Attributes.Add(new UIInt("Create Time", Iteration.CreateTime));
            Attributes.Add(new UIInt("Create Count", Iteration.CreateCount));
            Attributes.Add(new UIInt("Create Probability", Iteration.CreateProbability));
            Attributes.Add(new UIInt("Parent Influence Coord", Iteration.ParentInfluenceCoord));
            Attributes.Add(new UIInt("Parent Influence Color", Iteration.ParentInfluenceColor));
            Attributes.Add(new UIInt("Influence Coord Scale", Iteration.InfluenceCoordScale));
            Attributes.Add(new UIInt("Influence Coord Rotation", Iteration.InfluenceCoordRot));
            Attributes.Add(new UIInt("Influence Coord Position", Iteration.InfluenceCoordPos));
            Attributes.Add(new UIInt("Influence Coord Binder Position", Iteration.InfluenceCoordBinderPosition));
            Attributes.Add(new UIInt("Influence Coord Unstickiness", Iteration.InfluenceCoordUnstickiness));
            Attributes.Add(new UIInt("Inherit Parent Velocity", Iteration.InheritParentVelocity));
            Attributes.Add(new UIInt("Inherit Parent Life", Iteration.InheritParentLife));
            Attributes.Add(new UICheckbox("Override Life", Iteration.OverrideLife));
            Attributes.Add(new UIInt("Override Life Value", Iteration.OverrideLifeValue));
            Attributes.Add(new UIInt("Override Life Random", Iteration.OverrideLifeRandom));
            Attributes.Add(new UIInt("Parameter Link", Iteration.ParameterLink));
            Attributes.Add(new UIInt("Start Frame", Iteration.StartFrame));
            Attributes.Add(new UICheckbox("Start Frame Null Update", Iteration.StartFrameNullUpdate));
            Attributes.Add(new UIFloat3("By Injection Angle", Iteration.ByInjectionAngleX, Iteration.ByInjectionAngleY, Iteration.ByInjectionAngleZ));
            Attributes.Add(new UIInt("Generate Delay", Iteration.GenerateDelay));
            Attributes.Add(new UICheckbox("Generate Delay By One", Iteration.GenerateDelayByOne));
        }
        
        // =========== DRAW ==============
        public override void Draw( string parentId )
        {
        }
        public override void DrawSelect( string parentId, ref UIBase selected )
        {
            string Type = IsParticle ? "Particle" : "Emitter";
            if( !Assigned )
            {
                return;
            }
            if( ImGui.Selectable( Type + " " + Idx + parentId, selected == this ) )
            {
                selected = this;
            }
        }
        public override void DrawBody( string parentId )
        {
            string Type = IsParticle ? "Particle" : "Emitter";
            string id = parentId + "/Item" + Type + Idx;
            if( UIUtils.RemoveButton( "Delete" + id ) )
            {
                if( IsParticle )
                {
                    Emitter.Emitter.removeParticle( Idx );
                }
                else
                {
                    Emitter.Emitter.removeEmitter( Idx );
                }
                Emitter.Init();
            }
            DrawAttrs( id );
        }
    }
}
