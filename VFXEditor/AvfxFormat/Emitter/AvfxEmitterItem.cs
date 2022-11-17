using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxEmitterItem : GenericWorkspaceItem {
        public readonly bool IsParticle;
        public readonly AvfxEmitter Emitter;

        public readonly AvfxBool Enabled = new( "Enabled", "bEnb", defaultValue: false );
        public readonly AvfxInt TargetIdx = new( "Target Index", "TgtB", defaultValue: -1 );
        public readonly AvfxInt LocalDirection = new( "Local Direction", "LoDr", defaultValue: 0 );
        public readonly AvfxInt CreateTime = new( "Create Time", "CrTm", defaultValue: 1 );
        public readonly AvfxInt CreateCount = new( "Create Count", "CrCn", defaultValue: 1 );
        public readonly AvfxInt CreateProbability = new( "Create Probability", "CrPr", defaultValue: 100 );
        public readonly AvfxEnum<ParentInfluenceCoordOptions> ParentInfluenceCoord = new( "Influence on Child", "PICd", defaultValue: ParentInfluenceCoordOptions.InitialPosition_WithOptions );
        public readonly AvfxEnum<ParentInfluenceColorOptions> ParentInfluenceColor = new( "Influence on Child Color", "PICo", defaultValue: ParentInfluenceColorOptions.Initial );
        public readonly AvfxBool InfluenceCoordScale = new( "Influence on Scale", "ICbS", defaultValue: false );
        public readonly AvfxBool InfluenceCoordRot = new( "Influence on Rotation", "ICbR", defaultValue: false );
        public readonly AvfxBool InfluenceCoordPos = new( "Influence on Position", "ICbP", defaultValue: true );
        public readonly AvfxBool InfluenceCoordBinderPosition = new( "Influence on Binder Position", "ICbB", defaultValue: false );
        public readonly AvfxInt InfluenceCoordUnstickiness = new( "Influence Coordinate Unstickiness", "ICSK", defaultValue: 0 );
        public readonly AvfxBool InheritParentVelocity = new( "Inherit Parent Velocity", "IPbV", defaultValue: false );
        public readonly AvfxBool InheritParentLife = new( "Inherit Parent Life", "IPbL", defaultValue: false );
        public readonly AvfxBool OverrideLife = new( "Override Life", "bOvr", defaultValue: false );
        public readonly AvfxInt OverrideLifeValue = new( "Override Life Value", "OvrV", defaultValue: 60 );
        public readonly AvfxInt OverrideLifeRandom = new( "Override Life Random", "OvrR", defaultValue: 0 );
        public readonly AvfxInt ParameterLink = new( "Parameter Link","PrLk", defaultValue: -1 );
        public readonly AvfxInt StartFrame = new( "Start Frame", "StFr", defaultValue: 0 );
        public readonly AvfxBool StartFrameNullUpdate = new( "Start Frame Null Update", "bStN", defaultValue: false );
        public readonly AvfxFloat ByInjectionAngleX = new( "By Injection Angle X", "BIAX", defaultValue: 0 );
        public readonly AvfxFloat ByInjectionAngleY = new( "By Injection Angle Y", "BIAY", defaultValue: 0 );
        public readonly AvfxFloat ByInjectionAngleZ = new( "By Injection Angle Z", "BIAZ", defaultValue: 0 );
        public readonly AvfxInt GenerateDelay = new( "Generate Delay", "GenD", 0 );
        public readonly AvfxBool GenerateDelayByOne = new( "Generate Delay By One", "bGD", false );

        private readonly List<AvfxBase> Parsed;

        public UiNodeSelect<AvfxParticle> ParticleSelect;
        public UiNodeSelect<AvfxEmitter> EmitterSelect;

        private readonly List<IAvfxUiBase> Display;
        private readonly List<IAvfxUiBase> CoordOptionsDisplay;
        private readonly List<IAvfxUiBase> Display2;

        public AvfxEmitterItem( bool isParticle, AvfxEmitter emitter, bool initNodeSelects ) {
            IsParticle = isParticle;
            Emitter = emitter;

            Parsed = new() {
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
            };

            if( initNodeSelects ) InitializeNodeSelects();

            Display = new() {
                Enabled,
                LocalDirection,
                CreateTime,
                CreateCount,
                CreateProbability,
                ParentInfluenceColor
            };

            CoordOptionsDisplay = new() {
                InfluenceCoordScale,
                InfluenceCoordRot,
                InfluenceCoordPos
            };

            Display2 = new() {
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
                new UiFloat3( "By Injection Angle", ByInjectionAngleX, ByInjectionAngleY, ByInjectionAngleZ ),
                GenerateDelay,
                GenerateDelayByOne
            };
        }

        public AvfxEmitterItem( bool isParticle, AvfxEmitter emitter, bool initNodeSelects, BinaryReader reader ) : this( isParticle, emitter, initNodeSelects ) => AvfxBase.ReadNested( reader, Parsed, 312 );

        public void InitializeNodeSelects() {
            if( IsParticle ) ParticleSelect = new UiNodeSelect<AvfxParticle>( Emitter, "Target Particle", Emitter.NodeGroups.Particles, TargetIdx );
            else EmitterSelect = new UiNodeSelect<AvfxEmitter>( Emitter, "Target Emitter", Emitter.NodeGroups.Emitters, TargetIdx );
        }

        public void Write( BinaryWriter writer ) => AvfxBase.WriteNested( writer, Parsed );

        public override void Draw( string parentId ) {
            var id = parentId + "/Item";
            DrawRename( id );

            if( IsParticle ) ParticleSelect.Draw( id );
            else EmitterSelect.Draw( id );

            IAvfxUiBase.DrawList( Display, id );

            ParentInfluenceCoord.Draw( id );
            var influenceType = ParentInfluenceCoord.GetValue();
            var allowOptions = influenceType == ParentInfluenceCoordOptions.InitialPosition_WithOptions || influenceType == ParentInfluenceCoordOptions.WithOptions_NoPosition;
            if( !allowOptions ) ImGui.PushStyleVar( ImGuiStyleVar.Alpha, 0.5f );
            IAvfxUiBase.DrawList( CoordOptionsDisplay, id );
            if( !allowOptions ) ImGui.PopStyleVar();

            IAvfxUiBase.DrawList( Display2, id );
        }

        public override string GetDefaultText() {
            var type = IsParticle ? "Particle" : "Emitter";
            return GetIdx() + ": " + type + " " + TargetIdx.GetValue();
        }

        public override string GetWorkspaceId() {
            var type = IsParticle ? "Ptcl" : "Emit";
            return $"{Emitter.GetWorkspaceId()}/{type}{GetIdx()}";
        }
    }
}
