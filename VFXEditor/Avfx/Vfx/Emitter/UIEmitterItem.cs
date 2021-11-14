using AVFXLib.Models;

namespace VFXEditor.Avfx.Vfx {
    public class UIEmitterItem : UIWorkspaceItem {
        public AVFXEmitterIterationItem Iteration;
        public UIEmitter Emitter;
        public bool IsParticle;

        public UINodeSelect<UIParticle> ParticleSelect;
        public UINodeSelect<UIEmitter> EmitterSelect;

        public UIEmitterItem( AVFXEmitterIterationItem iteration, bool isParticle, UIEmitter emitter ) {
            Iteration = iteration;
            Emitter = emitter;
            IsParticle = isParticle;

            if( IsParticle ) {
                ParticleSelect = new UINodeSelect<UIParticle>( emitter, "Target Particle", Emitter.Main.Particles, Iteration.TargetIdx );
            }
            else {
                EmitterSelect = new UINodeSelect<UIEmitter>( emitter, "Target Emitter", Emitter.Main.Emitters, Iteration.TargetIdx );
            }
            //=========================
            Attributes.Add( new UICheckbox( "Enabled", Iteration.Enabled ) );
            Attributes.Add( new UIInt( "Local Direction", Iteration.LocalDirection ) );
            Attributes.Add( new UIInt( "Create Time", Iteration.CreateTime ) );
            Attributes.Add( new UIInt( "Create Count", Iteration.CreateCount ) );
            Attributes.Add( new UIInt( "Create Probability", Iteration.CreateProbability ) );
            Attributes.Add( new UIInt( "Parent Influence Coord", Iteration.ParentInfluenceCoord ) );
            Attributes.Add( new UIInt( "Parent Influence Color", Iteration.ParentInfluenceColor ) );
            Attributes.Add( new UIInt( "Influence Coord Scale", Iteration.InfluenceCoordScale ) );
            Attributes.Add( new UIInt( "Influence Coord Rotation", Iteration.InfluenceCoordRot ) );
            Attributes.Add( new UIInt( "Influence Coord Position", Iteration.InfluenceCoordPos ) );
            Attributes.Add( new UIInt( "Influence Coord Binder Position", Iteration.InfluenceCoordBinderPosition ) );
            Attributes.Add( new UIInt( "Influence Coord Unstickiness", Iteration.InfluenceCoordUnstickiness ) );
            Attributes.Add( new UIInt( "Inherit Parent Velocity", Iteration.InheritParentVelocity ) );
            Attributes.Add( new UIInt( "Inherit Parent Life", Iteration.InheritParentLife ) );
            Attributes.Add( new UICheckbox( "Override Life", Iteration.OverrideLife ) );
            Attributes.Add( new UIInt( "Override Life Value", Iteration.OverrideLifeValue ) );
            Attributes.Add( new UIInt( "Override Life Random", Iteration.OverrideLifeRandom ) );
            Attributes.Add( new UIInt( "Parameter Link", Iteration.ParameterLink ) );
            Attributes.Add( new UIInt( "Start Frame", Iteration.StartFrame ) );
            Attributes.Add( new UICheckbox( "Start Frame Null Update", Iteration.StartFrameNullUpdate ) );
            Attributes.Add( new UIFloat3( "By Injection Angle", Iteration.ByInjectionAngleX, Iteration.ByInjectionAngleY, Iteration.ByInjectionAngleZ ) );
            Attributes.Add( new UIInt( "Generate Delay", Iteration.GenerateDelay ) );
            Attributes.Add( new UICheckbox( "Generate Delay By One", Iteration.GenerateDelayByOne ) );
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
            DrawAttrs( id );
        }

        public override string GetDefaultText() {
            var Type = IsParticle ? "Particle" : "Emitter";
            return Idx + ": " + Type + " " + Iteration.TargetIdx.Value;
        }

        public override string GetWorkspaceId() {
            var Type = IsParticle ? "Ptcl" : "Emit";
            return $"{Emitter.GetWorkspaceId()}/{Type}{Idx}";
        }
    }
}
