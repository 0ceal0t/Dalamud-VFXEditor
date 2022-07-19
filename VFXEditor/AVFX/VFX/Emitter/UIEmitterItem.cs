using System.Collections.Generic;
using VFXEditor.AVFXLib.Emitter;

namespace VFXEditor.AVFX.VFX {
    public class UIEmitterItem : UIWorkspaceItem {
        public AVFXEmitterItem Iteration;
        public UIEmitter Emitter;
        public bool IsParticle;

        public UINodeSelect<UIParticle> ParticleSelect;
        public UINodeSelect<UIEmitter> EmitterSelect;
        private readonly List<UIBase> Parameters;

        public UIEmitterItem( AVFXEmitterItem iteration, bool isParticle, UIEmitter emitter ) {
            Iteration = iteration;
            Emitter = emitter;
            IsParticle = isParticle;

            if( IsParticle ) {
                ParticleSelect = new UINodeSelect<UIParticle>( emitter, "Target Particle", Emitter.NodeGroups.Particles, Iteration.TargetIdx );
            }
            else {
                EmitterSelect = new UINodeSelect<UIEmitter>( emitter, "Target Emitter", Emitter.NodeGroups.Emitters, Iteration.TargetIdx );
            }

            Parameters = new List<UIBase> {
                new UICheckbox( "Enabled", Iteration.Enabled ),
                new UIInt( "Local Direction", Iteration.LocalDirection ),
                new UIInt( "Create Time", Iteration.CreateTime ),
                new UIInt( "Create Count", Iteration.CreateCount ),
                new UIInt( "Create Probability", Iteration.CreateProbability ),
                new UIInt( "Parent Influence Coord", Iteration.ParentInfluenceCoord ),
                new UIInt( "Parent Influence Color", Iteration.ParentInfluenceColor ),
                new UIInt( "Influence Coord Scale", Iteration.InfluenceCoordScale ),
                new UIInt( "Influence Coord Rotation", Iteration.InfluenceCoordRot ),
                new UIInt( "Influence Coord Position", Iteration.InfluenceCoordPos ),
                new UIInt( "Influence Coord Binder Position", Iteration.InfluenceCoordBinderPosition ),
                new UIInt( "Influence Coord Unstickiness", Iteration.InfluenceCoordUnstickiness ),
                new UIInt( "Inherit Parent Velocity", Iteration.InheritParentVelocity ),
                new UIInt( "Inherit Parent Life", Iteration.InheritParentLife ),
                new UICheckbox( "Override Life", Iteration.OverrideLife ),
                new UIInt( "Override Life Value", Iteration.OverrideLifeValue ),
                new UIInt( "Override Life Random", Iteration.OverrideLifeRandom ),
                new UIInt( "Parameter Link", Iteration.ParameterLink ),
                new UIInt( "Start Frame", Iteration.StartFrame ),
                new UICheckbox( "Start Frame Null Update", Iteration.StartFrameNullUpdate ),
                new UIFloat3( "By Injection Angle", Iteration.ByInjectionAngleX, Iteration.ByInjectionAngleY, Iteration.ByInjectionAngleZ ),
                new UIInt( "Generate Delay", Iteration.GenerateDelay ),
                new UICheckbox( "Generate Delay By One", Iteration.GenerateDelayByOne )
            };
        }

        public override void DrawBody( string parentId ) {
            var id = parentId + "/Item";
            DrawRename( id );
            if( IsParticle ) {
                ParticleSelect.Draw( id );
            }
            else {
                EmitterSelect.Draw( id );
            }
            DrawList( Parameters, id );
        }

        public override string GetDefaultText() {
            var Type = IsParticle ? "Particle" : "Emitter";
            return Idx + ": " + Type + " " + Iteration.TargetIdx.GetValue();
        }

        public override string GetWorkspaceId() {
            var Type = IsParticle ? "Ptcl" : "Emit";
            return $"{Emitter.GetWorkspaceId()}/{Type}{Idx}";
        }
    }
}
