using System.Collections.Generic;
using System.IO;

namespace VfxEditor.AVFXLib.Emitter {
    public class AVFXEmitterCreate : AVFXBase {
        public readonly List<AVFXEmitterItem> Items = new();

        public AVFXEmitterCreate( string name ) : base( name ) {
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            for( var i = 0; i < size / 312; i++ ) {
                Items.Add( new AVFXEmitterItem( reader ) );
            }
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            foreach( var item in Items ) {
                item.Write( writer );
            }
        }
    }

    public class AVFXEmitterItem {
        public readonly AVFXBool Enabled = new( "bEnb" );
        public readonly AVFXInt TargetIdx = new( "TgtB" );
        public readonly AVFXInt LocalDirection = new( "LoDr" );
        public readonly AVFXInt CreateTime = new( "CrTm" );
        public readonly AVFXInt CreateCount = new( "CrCn" );
        public readonly AVFXInt CreateProbability = new( "CrPr" );
        public readonly AVFXEnum<ParentInfluenceCoordOptions> ParentInfluenceCoord = new( "PICd" );
        public readonly AVFXEnum<ParentInfluenceColorOptions> ParentInfluenceColor = new( "PICo" );
        public readonly AVFXBool InfluenceCoordScale = new( "ICbS" );
        public readonly AVFXBool InfluenceCoordRot = new( "ICbR" );
        public readonly AVFXBool InfluenceCoordPos = new( "ICbP" );
        public readonly AVFXBool InfluenceCoordBinderPosition = new( "ICbB" );
        public readonly AVFXInt InfluenceCoordUnstickiness = new( "ICSK" );
        public readonly AVFXBool InheritParentVelocity = new( "IPbV" );
        public readonly AVFXBool InheritParentLife = new( "IPbL" );
        public readonly AVFXBool OverrideLife = new( "bOvr" );
        public readonly AVFXInt OverrideLifeValue = new( "OvrV" );
        public readonly AVFXInt OverrideLifeRandom = new( "OvrR" );
        public readonly AVFXInt ParameterLink = new( "PrLk" );
        public readonly AVFXInt StartFrame = new( "StFr" );
        public readonly AVFXBool StartFrameNullUpdate = new( "bStN" );
        public readonly AVFXFloat ByInjectionAngleX = new( "BIAX" );
        public readonly AVFXFloat ByInjectionAngleY = new( "BIAY" );
        public readonly AVFXFloat ByInjectionAngleZ = new( "BIAZ" );
        public readonly AVFXInt GenerateDelay = new( "GenD" );
        public readonly AVFXBool GenerateDelayByOne = new( "bGD" );

        private readonly List<AVFXBase> Children;

        public AVFXEmitterItem() {
            Children = new List<AVFXBase> {
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
        }

        public AVFXEmitterItem( BinaryReader reader ) : this() {
            AVFXBase.ReadNested( reader, Children, 312 );
        }

        public void Write( BinaryWriter writer ) {
            AVFXBase.WriteNested( writer, Children );
        }
    }
}
