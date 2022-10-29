using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Emitter;

namespace VfxEditor.AVFX.VFX {
    public class UIEmitterItem : UIWorkspaceItem {
        public AVFXEmitterItem Iteration;
        public UIEmitter Emitter;
        public bool IsParticle;

        public UINodeSelect<UIParticle> ParticleSelect;
        public UINodeSelect<UIEmitter> EmitterSelect;

        private readonly List<IUIBase> Parameters;
        private readonly UICombo<ParentInfluenceCoordOptions> ParentInfluenceCoord;
        private readonly List<IUIBase> CoordOptions;
        private readonly List<IUIBase> Parameters2;

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

            Parameters = new List<IUIBase> {
                new UICheckbox( "Enabled", Iteration.Enabled ),
                new UIInt( "Local Direction", Iteration.LocalDirection ),
                new UIInt( "Create Time", Iteration.CreateTime ),
                new UIInt( "Create Count", Iteration.CreateCount ),
                new UIInt( "Create Probability", Iteration.CreateProbability ),
                new UICombo<ParentInfluenceColorOptions>( "Influence on Child Color", Iteration.ParentInfluenceColor )
            };

            ParentInfluenceCoord = new UICombo<ParentInfluenceCoordOptions>( "Influence on Child", Iteration.ParentInfluenceCoord );

            CoordOptions = new List<IUIBase> {
                new UICheckbox( "Influence on Scale", Iteration.InfluenceCoordScale ),
                new UICheckbox( "Influence on Rotation", Iteration.InfluenceCoordRot ),
                new UICheckbox( "Influence on Position", Iteration.InfluenceCoordPos )
            };

            Parameters2 = new List<IUIBase> {
                new UICheckbox( "Influence on Binder Position", Iteration.InfluenceCoordBinderPosition ),
                new UIInt( "Influence Coordinate Unstickiness", Iteration.InfluenceCoordUnstickiness ),
                new UICheckbox( "Inherit Parent Velocity", Iteration.InheritParentVelocity ),
                new UICheckbox( "Inherit Parent Life", Iteration.InheritParentLife ),
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

        public override void DrawInline( string parentId ) {
            var id = parentId + "/Item";
            DrawRename( id );

            if( IsParticle ) ParticleSelect.DrawInline( id );
            else EmitterSelect.DrawInline( id );

            IUIBase.DrawList( Parameters, id );

            ParentInfluenceCoord.DrawInline( id );
            var influenceType = ParentInfluenceCoord.Literal.GetValue();
            var allowOptions = influenceType == ParentInfluenceCoordOptions.InitialPosition_WithOptions || influenceType == ParentInfluenceCoordOptions.WithOptions_NoPosition;
            if( !allowOptions ) ImGui.PushStyleVar( ImGuiStyleVar.Alpha, 0.5f );
            IUIBase.DrawList( CoordOptions, id );
            if( !allowOptions ) ImGui.PopStyleVar();

            IUIBase.DrawList( Parameters2, id );
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
