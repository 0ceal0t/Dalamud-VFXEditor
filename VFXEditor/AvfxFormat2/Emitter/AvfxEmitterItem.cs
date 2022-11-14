using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VfxEditor.AvfxFormat2.Enums;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxEmitterItem : GenericWorkspaceItem {
        public readonly bool IsParticle;
        public readonly AvfxEmitter Emitter;

        public readonly AvfxBool Enabled = new( "Enabled", "bEnb" );
        public readonly AvfxInt TargetIdx = new( "Target Index", "TgtB" );
        public readonly AvfxInt LocalDirection = new( "Local Direction", "LoDr" );
        public readonly AvfxInt CreateTime = new( "Create Time", "CrTm" );
        public readonly AvfxInt CreateCount = new( "Create Count", "CrCn" );
        public readonly AvfxInt CreateProbability = new( "Create Probability", "CrPr" );
        public readonly AvfxEnum<ParentInfluenceCoordOptions> ParentInfluenceCoord = new( "Influence on Child", "PICd" );
        public readonly AvfxEnum<ParentInfluenceColorOptions> ParentInfluenceColor = new( "Influence on Child Color", "PICo" );
        public readonly AvfxBool InfluenceCoordScale = new( "Influence on Scale", "ICbS" );
        public readonly AvfxBool InfluenceCoordRot = new( "Influence on Rotation", "ICbR" );
        public readonly AvfxBool InfluenceCoordPos = new( "Influence on Position", "ICbP" );
        public readonly AvfxBool InfluenceCoordBinderPosition = new( "Influence on Binder Position", "ICbB" );
        public readonly AvfxInt InfluenceCoordUnstickiness = new( "Influence Coordinate Unstickiness", "ICSK" );
        public readonly AvfxBool InheritParentVelocity = new( "Inherit Parent Velocity", "IPbV" );
        public readonly AvfxBool InheritParentLife = new( "Inherit Parent Life", "IPbL" );
        public readonly AvfxBool OverrideLife = new( "Override Life", "bOvr" );
        public readonly AvfxInt OverrideLifeValue = new( "Override Life Value", "OvrV" );
        public readonly AvfxInt OverrideLifeRandom = new( "Override Life Random", "OvrR" );
        public readonly AvfxInt ParameterLink = new( "Parameter Link","PrLk" );
        public readonly AvfxInt StartFrame = new( "Start Frame", "StFr" );
        public readonly AvfxBool StartFrameNullUpdate = new( "Start Frame Null Update", "bStN" );
        public readonly AvfxFloat ByInjectionAngleX = new( "By Injection Angle X", "BIAX" );
        public readonly AvfxFloat ByInjectionAngleY = new( "By Injection Angle Y", "BIAY" );
        public readonly AvfxFloat ByInjectionAngleZ = new( "By Injection Angle Z", "BIAZ" );
        public readonly AvfxInt GenerateDelay = new( "Generate Delay", "GenD" );
        public readonly AvfxBool GenerateDelayByOne = new( "Generate Delay By One", "bGD" );

        private readonly List<AvfxBase> Parsed;

        public UiNodeSelect<AvfxParticle> ParticleSelect;
        public UiNodeSelect<AvfxEmitter> EmitterSelect;

        private readonly List<IUiBase> Display;
        private readonly List<IUiBase> CoordOptionsDisplay;
        private readonly List<IUiBase> Display2;

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

            TargetIdx.SetValue( -1 );
            Enabled.SetValue( false );
            LocalDirection.SetValue( 0 );
            CreateTime.SetValue( 1 );
            CreateCount.SetValue( 1 );
            CreateProbability.SetValue( 100 );
            ParentInfluenceCoord.SetValue( ParentInfluenceCoordOptions.InitialPosition_WithOptions );
            ParentInfluenceColor.SetValue( ParentInfluenceColorOptions.Initial );
            InfluenceCoordScale.SetValue( false );
            InfluenceCoordRot.SetValue( false );
            InfluenceCoordPos.SetValue( true );
            InfluenceCoordBinderPosition.SetValue( false );
            InfluenceCoordUnstickiness.SetValue( 0 );
            InheritParentVelocity.SetValue( false );
            InheritParentLife.SetValue( false );
            OverrideLife.SetValue( false );
            OverrideLifeValue.SetValue( 60 );
            OverrideLifeRandom.SetValue( 0 );
            ParameterLink.SetValue( -1 );
            StartFrame.SetValue( 0 );
            StartFrameNullUpdate.SetValue( false );
            ByInjectionAngleX.SetValue( 0 );
            ByInjectionAngleY.SetValue( 0 );
            ByInjectionAngleZ.SetValue( 0 );
            GenerateDelay.SetValue( 0 );
            GenerateDelayByOne.SetValue( false );

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

            IUiBase.DrawList( Display, id );

            ParentInfluenceCoord.Draw( id );
            var influenceType = ParentInfluenceCoord.GetValue();
            var allowOptions = influenceType == ParentInfluenceCoordOptions.InitialPosition_WithOptions || influenceType == ParentInfluenceCoordOptions.WithOptions_NoPosition;
            if( !allowOptions ) ImGui.PushStyleVar( ImGuiStyleVar.Alpha, 0.5f );
            IUiBase.DrawList( CoordOptionsDisplay, id );
            if( !allowOptions ) ImGui.PopStyleVar();

            IUiBase.DrawList( Display2, id );
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
