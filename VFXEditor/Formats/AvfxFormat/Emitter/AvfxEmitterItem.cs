using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Ui.Interfaces;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxEmitterItem : GenericWorkspaceItem {
        public readonly bool IsParticle;
        public readonly AvfxEmitter Emitter;

        public readonly AvfxBool Enabled = new( "Enabled", "bEnb", value: false );
        public readonly AvfxInt TargetIdx = new( "Target Index", "TgtB", value: -1 );
        public readonly AvfxInt LocalDirection = new( "Local Direction", "LoDr", value: 0 );
        public readonly AvfxInt CreateTime = new( "Create Time", "CrTm", value: 1 );
        public readonly AvfxInt CreateCount = new( "Create Count", "CrCn", value: 1 );
        public readonly AvfxInt CreateProbability = new( "Create Probability", "CrPr", value: 100 );
        public readonly AvfxEnum<ParentInfluenceCoordOptions> ParentInfluenceCoord = new( "Influence on Child", "PICd", value: ParentInfluenceCoordOptions.InitialPosition_WithOptions );
        public readonly AvfxEnum<ParentInfluenceColorOptions> ParentInfluenceColor = new( "Influence on Child Color", "PICo", value: ParentInfluenceColorOptions.Initial );
        public readonly AvfxBool InfluenceCoordScale = new( "Influence on Scale", "ICbS", value: false );
        public readonly AvfxBool InfluenceCoordRot = new( "Influence on Rotation", "ICbR", value: false );
        public readonly AvfxBool InfluenceCoordPos = new( "Influence on Position", "ICbP", value: true );
        public readonly AvfxBool InfluenceCoordBinderPosition = new( "Influence on Binder Position", "ICbB", value: false );
        public readonly AvfxFloat InfluenceCoordUnstickiness = new( "Influence Coordinate Unstickiness", "ICSK", value: 0 );
        public readonly AvfxBool InheritParentVelocity = new( "Inherit Parent Velocity", "IPbV", value: false );
        public readonly AvfxBool InheritParentLife = new( "Inherit Parent Life", "IPbL", value: false );
        public readonly AvfxBool OverrideLife = new( "Override Life", "bOvr", value: false );
        public readonly AvfxInt OverrideLifeValue = new( "Override Life Value", "OvrV", value: 60 );
        public readonly AvfxInt OverrideLifeRandom = new( "Override Life Random", "OvrR", value: 0 );
        public readonly AvfxInt ParameterLink = new( "Parameter Link", "PrLk", value: -1 );
        public readonly AvfxInt StartFrame = new( "Start Frame", "StFr", value: 0 );
        public readonly AvfxBool StartFrameNullUpdate = new( "Start Frame Null Update", "bStN", value: false );
        public readonly AvfxRadians ByInjectionAngleX = new( "By Injection Angle X", "BIAX", value: 0 );
        public readonly AvfxRadians ByInjectionAngleY = new( "By Injection Angle Y", "BIAY", value: 0 );
        public readonly AvfxRadians ByInjectionAngleZ = new( "By Injection Angle Z", "BIAZ", value: 0 );
        public readonly AvfxInt GenerateDelay = new( "Generate Delay", "GenD", 0 );
        public readonly AvfxBool GenerateDelayByOne = new( "Generate Delay By One", "bGD", value: false );

        private readonly List<AvfxBase> Parsed;

        public AvfxNodeSelect<AvfxParticle> ParticleSelect;
        public AvfxNodeSelect<AvfxEmitter> EmitterSelect;

        private readonly List<IUiItem> Display;
        private readonly List<IUiItem> CoordOptionsDisplay;
        private readonly List<IUiItem> Display2;

        public AvfxEmitterItem( bool isParticle, AvfxEmitter emitter, bool initNodeSelects ) {
            IsParticle = isParticle;
            Emitter = emitter;

            Parsed = [
                Enabled,
                TargetIdx,
                LocalDirection,
                CreateTime,
                CreateCount,
                CreateProbability,
                ParentInfluenceCoord,
                ParentInfluenceColor,
                InfluenceCoordScale,
                InfluenceCoordRot,
                InfluenceCoordPos,
                InfluenceCoordBinderPosition,
                InfluenceCoordUnstickiness,
                InheritParentVelocity,
                InheritParentLife,
                OverrideLife,
                OverrideLifeValue,
                OverrideLifeRandom,
                ParameterLink,
                StartFrame,
                StartFrameNullUpdate,
                ByInjectionAngleX,
                ByInjectionAngleY,
                ByInjectionAngleZ,
                GenerateDelay,
                GenerateDelayByOne
            ];

            if( initNodeSelects ) InitializeNodeSelects();

            Display = [
                Enabled,
                LocalDirection,
                CreateTime,
                CreateCount,
                CreateProbability,
                ParentInfluenceColor
            ];

            CoordOptionsDisplay = [
                InfluenceCoordScale,
                InfluenceCoordRot,
                InfluenceCoordPos
            ];

            Display2 = [
                InfluenceCoordBinderPosition,
                InfluenceCoordUnstickiness,
                InheritParentVelocity,
                InheritParentLife,
                OverrideLife,
                OverrideLifeValue,
                OverrideLifeRandom,
                ParameterLink,
                StartFrame,
                StartFrameNullUpdate,
                ByInjectionAngleX,
                ByInjectionAngleY,
                ByInjectionAngleZ,
                GenerateDelay,
                GenerateDelayByOne
            ];
        }

        public AvfxEmitterItem( bool isParticle, AvfxEmitter emitter, bool initNodeSelects, int size, BinaryReader reader ) : this( isParticle, emitter, initNodeSelects ) => AvfxBase.ReadNested( reader, Parsed, size );

        public void InitializeNodeSelects() {
            if( IsParticle ) ParticleSelect = new AvfxNodeSelect<AvfxParticle>( Emitter, "Target Particle", Emitter.NodeGroups.Particles, TargetIdx );
            else EmitterSelect = new AvfxNodeSelect<AvfxEmitter>( Emitter, "Target Emitter", Emitter.NodeGroups.Emitters, TargetIdx );
        }

        public void Write( BinaryWriter writer ) => AvfxBase.WriteNested( writer, Parsed );

        public override void Draw() {
            using var _ = ImRaii.PushId( "Item" );
            DrawRename();

            if( IsParticle ) ParticleSelect.Draw();
            else EmitterSelect.Draw();

            AvfxBase.DrawItems( Display );

            ParentInfluenceCoord.Draw();
            var influenceType = ParentInfluenceCoord.Value;
            var allowOptions = influenceType == ParentInfluenceCoordOptions.InitialPosition_WithOptions || influenceType == ParentInfluenceCoordOptions.WithOptions_NoPosition;
            if( !allowOptions ) ImGui.PushStyleVar( ImGuiStyleVar.Alpha, 0.5f );
            AvfxBase.DrawItems( CoordOptionsDisplay );
            if( !allowOptions ) ImGui.PopStyleVar();

            AvfxBase.DrawItems( Display2 );
        }

        public override string GetDefaultText() => IsParticle ? ParticleSelect.GetText() : EmitterSelect.GetText();

        public override string GetWorkspaceId() {
            var type = IsParticle ? "Ptcl" : "Emit";
            return $"{Emitter.GetWorkspaceId()}/{type}{GetIdx()}";
        }
    }
}
