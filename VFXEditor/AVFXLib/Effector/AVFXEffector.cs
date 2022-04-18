using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.AVFXLib.Effector {
    public class AVFXEffector : AVFXBase {
        public const string NAME = "Efct";

        public readonly AVFXEnum<EffectorType> EffectorVariety = new( "EfVT" );
        public readonly AVFXEnum<RotationOrder> RotationOrder = new( "RoOT" );
        public readonly AVFXEnum<CoordComputeOrder> CoordComputeOrder = new( "CCOT" );
        public readonly AVFXBool AffectOtherVfx = new( "bAOV" );
        public readonly AVFXBool AffectGame = new( "bAGm" );
        public readonly AVFXInt LoopPointStart = new( "LpSt" );
        public readonly AVFXInt LoopPointEnd = new( "LpEd" );

        public EffectorType Type;
        public AVFXBase Data;

        private readonly List<AVFXBase> Children;

        public AVFXEffector() : base( NAME ) {
            Children = new List<AVFXBase> {
                EffectorVariety,
                RotationOrder,
                CoordComputeOrder,
                AffectOtherVfx,
                AffectGame,
                LoopPointStart,
                LoopPointEnd
            };
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            Peek( reader, Children, size );
            Type = EffectorVariety.GetValue();

            ReadNested( reader, ( BinaryReader _reader, string _name, int _size ) => {
                if (_name == "Data") {
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

        private void SetData( EffectorType type ) {
            Data = type switch {
                EffectorType.PointLight => new AVFXEffectorDataPointLight(),
                EffectorType.DirectionalLight => new AVFXEffectorDataDirectionalLight(),
                EffectorType.RadialBlur => new AVFXEffectorDataRadialBlur(),
                EffectorType.BlackHole => null,
                EffectorType.CameraQuake2_Unknown or EffectorType.CameraQuake => new AVFXEffectorDataCameraQuake(),
                _ => null
            };
        }

        public void SetType( EffectorType type ) {
            EffectorVariety.SetValue( type );
            Type = type;
            SetData( type );
            Data?.SetAssigned( true );
        }
    }
}
