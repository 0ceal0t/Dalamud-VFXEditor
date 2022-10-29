using System.Collections.Generic;
using System.IO;

namespace VfxEditor.AVFXLib.Binder {
    public class AVFXBinder : AVFXBase {
        public const string NAME = "Bind";

        public readonly AVFXBool StartToGlobalDirection = new( "bStG" );
        public readonly AVFXBool VfxScaleEnabled = new( "bVSc" );
        public readonly AVFXFloat VfxScaleBias = new( "bVSb" );
        public readonly AVFXBool VfxScaleDepthOffset = new( "bVSd" );
        public readonly AVFXBool VfxScaleInterpolation = new( "bVSi" );
        public readonly AVFXBool TransformScale = new( "bTSc" );
        public readonly AVFXBool TransformScaleDepthOffset = new( "bTSd" );
        public readonly AVFXBool TransformScaleInterpolation = new( "bTSi" );
        public readonly AVFXBool FollowingTargetOrientation = new( "bFTO" );
        public readonly AVFXBool DocumentScaleEnabled = new( "bDSE" );
        public readonly AVFXBool AdjustToScreenEnabled = new( "bATS" );
        public readonly AVFXBool BET_Unknown = new( "bBET" );
        public readonly AVFXInt Life = new( "Life" );
        public readonly AVFXEnum<BinderRotation> BinderRotationType = new( "RoTp" );
        public readonly AVFXEnum<BinderType> BinderVariety = new( "BnVr" );
        public readonly AVFXBinderProperty PropStart = new( "PrpS" );
        public readonly AVFXBinderProperty Prop1 = new( "Prp1" );
        public readonly AVFXBinderProperty Prop2 = new( "Prp2" );
        public readonly AVFXBinderProperty PropGoal = new( "PrpG" );

        public BinderType Type;
        public AVFXBase Data;

        private readonly List<AVFXBase> Children;

        public AVFXBinder() : base( NAME ) {
            Children = new List<AVFXBase> {
                StartToGlobalDirection,
                VfxScaleEnabled,
                VfxScaleBias,
                VfxScaleDepthOffset,
                VfxScaleInterpolation,
                TransformScale,
                TransformScaleDepthOffset,
                TransformScaleInterpolation,
                FollowingTargetOrientation,
                DocumentScaleEnabled,
                AdjustToScreenEnabled,
                BET_Unknown,
                Life,
                BinderRotationType,
                BinderVariety,
                PropStart,
                Prop1,
                Prop2,
                PropGoal
            };
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            Peek( reader, Children, size );
            Type = BinderVariety.GetValue();

            ReadNested( reader, ( BinaryReader _reader, string _name, int _size ) => {
                if( _name == "Data" ) {
                    SetData( Type );
                    Data?.Read( _reader, _size );
                }
            }, size );
        }

        protected override void RecurseChildrenAssigned( bool assigned ) {
            RecurseAssigned( Children, assigned );
            RecurseAssigned( Data, assigned );
        }

        protected override void WriteContents( BinaryWriter writer ) {
            WriteNested( writer, Children );
            Data?.Write( writer );
        }

        private void SetData( BinderType type ) {
            Data = type switch {
                BinderType.Point => new AVFXBinderDataPoint(),
                BinderType.Linear => new AVFXBinderDataLinear(),
                BinderType.Spline => new AVFXBinderDataSpline(),
                BinderType.Camera => new AVFXBinderDataCamera(),
                _ => null,
            };
        }

        public void SetType( BinderType type ) {
            BinderVariety.SetValue( type );
            Type = type;
            SetData( type );
            Data?.SetAssigned( true );
        }
    }
}
