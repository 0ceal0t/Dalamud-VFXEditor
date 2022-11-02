using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Emitter;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiEmitterItem : UiWorkspaceItem {
        public AVFXEmitterItem Iteration;
        public UiEmitter Emitter;
        public bool IsParticle;

        public UiNodeSelect<UiParticle> ParticleSelect;
        public UiNodeSelect<UiEmitter> EmitterSelect;

        private readonly List<IUiBase> Parameters;
        private readonly UiCombo<ParentInfluenceCoordOptions> ParentInfluenceCoord;
        private readonly List<IUiBase> CoordOptions;
        private readonly List<IUiBase> Parameters2;

        public UiEmitterItem( AVFXEmitterItem iteration, bool isParticle, UiEmitter emitter ) {
            Iteration = iteration;
            Emitter = emitter;
            IsParticle = isParticle;

            if( IsParticle ) {
                ParticleSelect = new UiNodeSelect<UiParticle>( emitter, "Target Particle", Emitter.NodeGroups.Particles, Iteration.TargetIdx );
            }
            else {
                EmitterSelect = new UiNodeSelect<UiEmitter>( emitter, "Target Emitter", Emitter.NodeGroups.Emitters, Iteration.TargetIdx );
            }

            Parameters = new List<IUiBase> {
                new UiCheckbox( "Enabled", Iteration.Enabled ),
                new UiInt( "Local Direction", Iteration.LocalDirection ),
                new UiInt( "Create Time", Iteration.CreateTime ),
                new UiInt( "Create Count", Iteration.CreateCount ),
                new UiInt( "Create Probability", Iteration.CreateProbability ),
                new UiCombo<ParentInfluenceColorOptions>( "Influence on Child Color", Iteration.ParentInfluenceColor )
            };

            ParentInfluenceCoord = new UiCombo<ParentInfluenceCoordOptions>( "Influence on Child", Iteration.ParentInfluenceCoord );

            CoordOptions = new List<IUiBase> {
                new UiCheckbox( "Influence on Scale", Iteration.InfluenceCoordScale ),
                new UiCheckbox( "Influence on Rotation", Iteration.InfluenceCoordRot ),
                new UiCheckbox( "Influence on Position", Iteration.InfluenceCoordPos )
            };

            Parameters2 = new List<IUiBase> {
                new UiCheckbox( "Influence on Binder Position", Iteration.InfluenceCoordBinderPosition ),
                new UiInt( "Influence Coordinate Unstickiness", Iteration.InfluenceCoordUnstickiness ),
                new UiCheckbox( "Inherit Parent Velocity", Iteration.InheritParentVelocity ),
                new UiCheckbox( "Inherit Parent Life", Iteration.InheritParentLife ),
                new UiCheckbox( "Override Life", Iteration.OverrideLife ),
                new UiInt( "Override Life Value", Iteration.OverrideLifeValue ),
                new UiInt( "Override Life Random", Iteration.OverrideLifeRandom ),
                new UiInt( "Parameter Link", Iteration.ParameterLink ),
                new UiInt( "Start Frame", Iteration.StartFrame ),
                new UiCheckbox( "Start Frame Null Update", Iteration.StartFrameNullUpdate ),
                new UiFloat3( "By Injection Angle", Iteration.ByInjectionAngleX, Iteration.ByInjectionAngleY, Iteration.ByInjectionAngleZ ),
                new UiInt( "Generate Delay", Iteration.GenerateDelay ),
                new UiCheckbox( "Generate Delay By One", Iteration.GenerateDelayByOne )
            };
        }

        public override void DrawInline( string parentId ) {
            var id = parentId + "/Item";
            DrawRename( id );

            if( IsParticle ) ParticleSelect.DrawInline( id );
            else EmitterSelect.DrawInline( id );

            IUiBase.DrawList( Parameters, id );

            ParentInfluenceCoord.DrawInline( id );
            var influenceType = ParentInfluenceCoord.Literal.GetValue();
            var allowOptions = influenceType == ParentInfluenceCoordOptions.InitialPosition_WithOptions || influenceType == ParentInfluenceCoordOptions.WithOptions_NoPosition;
            if( !allowOptions ) ImGui.PushStyleVar( ImGuiStyleVar.Alpha, 0.5f );
            IUiBase.DrawList( CoordOptions, id );
            if( !allowOptions ) ImGui.PopStyleVar();

            IUiBase.DrawList( Parameters2, id );
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
