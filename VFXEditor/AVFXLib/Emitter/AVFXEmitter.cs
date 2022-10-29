using System.Collections.Generic;
using System.IO;
using VfxEditor.AVFXLib.Curve;

namespace VfxEditor.AVFXLib.Emitter {
    public class AVFXEmitter : AVFXBase {
        public const string NAME = "Emit";

        public readonly AVFXString Sound = new( "SdNm" );
        public readonly AVFXInt SoundNumber = new( "SdNo" );
        public readonly AVFXInt LoopStart = new( "LpSt" );
        public readonly AVFXInt LoopEnd = new( "LpEd" );
        public readonly AVFXInt ChildLimit = new( "ClCn" );
        public readonly AVFXInt EffectorIdx = new( "EfNo" );
        public readonly AVFXBool AnyDirection = new( "bAD", size: 1 );
        public readonly AVFXEnum<EmitterType> EmitterVariety = new( "EVT" );
        public readonly AVFXEnum<RotationDirectionBase> RotationDirectionBaseType = new( "RBDT" );
        public readonly AVFXEnum<CoordComputeOrder> CoordComputeOrderType = new( "CCOT" );
        public readonly AVFXEnum<RotationOrder> RotationOrderType = new( "ROT" );
        public readonly AVFXInt ParticleCount = new( "PrCn" );
        public readonly AVFXInt EmitterCount = new( "EmCn" );
        public readonly AVFXLife Life = new();

        public readonly AVFXCurve CreateCount = new( "CrC" );
        public readonly AVFXCurve CreateCountRandom = new( "CrCR" );
        public readonly AVFXCurve CreateInterval = new( "CrI" );
        public readonly AVFXCurve CreateIntervalRandom = new( "CrIR" );
        public readonly AVFXCurve Gravity = new( "Gra" );
        public readonly AVFXCurve GravityRandom = new( "GraR" );
        public readonly AVFXCurve AirResistance = new( "ARs" );
        public readonly AVFXCurve AirResistanceRandom = new( "ARsR" );
        public readonly AVFXCurveColor Color = new();
        public readonly AVFXCurve3Axis Position = new( "Pos" );
        public readonly AVFXCurve3Axis Rotation = new( "Rot" );
        public readonly AVFXCurve3Axis Scale = new( "Scl" );

        public readonly List<AVFXEmitterItem> Particles = new();
        public readonly List<AVFXEmitterItem> Emitters = new();

        public EmitterType Type;
        public AVFXBase Data;

        private readonly List<AVFXBase> Children;

        public AVFXEmitter() : base( NAME ) {
            Children = new List<AVFXBase> {
                Sound,
                SoundNumber,
                LoopStart,
                LoopEnd,
                ChildLimit,
                EffectorIdx,
                AnyDirection,
                EmitterVariety,
                RotationDirectionBaseType,
                CoordComputeOrderType,
                RotationOrderType,
                ParticleCount,
                EmitterCount,
                Life,
                CreateCount,
                CreateCountRandom,
                CreateInterval,
                CreateIntervalRandom,
                Gravity,
                GravityRandom,
                AirResistance,
                AirResistanceRandom,
                Color,
                Position,
                Rotation,
                Scale
            };
            Sound.SetAssigned( false );
        }

        /*
         * for 2 particles and 3 emitters:
         * 
         * P: p1 p2
         * E: e1 e2 e3
         * 
         * will be logged:
         * 
         * ItPr: p1
         * ItPr: p1 p2
         * ItEm: p1 p2 e1
         * ItEm: p1 p2 e1 e2
         * ItEm: p1 p2 e1 e2 e3
         */

        public override void ReadContents( BinaryReader reader, int size ) {
            Peek( reader, Children, size );
            Type = EmitterVariety.GetValue();

            AVFXEmitterCreate lastParticle = null;
            AVFXEmitterCreate lastEmitter = null;

            ReadNested( reader, ( BinaryReader _reader, string _name, int _size ) => {
                if( _name == "Data" ) {
                    SetData( Type );
                    Data?.Read( _reader, _size );
                }
                else if( _name == "ItPr" ) {
                    lastParticle = new AVFXEmitterCreate( "ItPr" );
                    lastParticle.Read( _reader, _size );

                }
                else if( _name == "ItEm" ) {
                    lastEmitter = new AVFXEmitterCreate( "ItEm" );
                    lastEmitter.Read( _reader, _size );

                }
            }, size );

            if( lastParticle != null ) {
                Particles.AddRange( lastParticle.Items );
            }
            if( lastEmitter != null ) {
                var startIndex = Particles.Count;
                var emitterCount = lastEmitter.Items.Count - Particles.Count;
                Emitters.AddRange( lastEmitter.Items.GetRange( startIndex, emitterCount ) ); // remove particles
            }
        }

        protected override void RecurseChildrenAssigned( bool assigned ) {
            RecurseAssigned( Children, assigned );
            RecurseAssigned( Data, assigned );
        }

        protected override void WriteContents( BinaryWriter writer ) {
            WriteNested( writer, Children );

            // ItPr
            for( var i = 0; i < Particles.Count; i++ ) {
                var ItPr = new AVFXEmitterCreate( "ItPr" );
                ItPr.Items.AddRange( Particles.GetRange( 0, i + 1 ) );
                ItPr.Write( writer );
            }

            // ItEm
            for( var i = 0; i < Emitters.Count; i++ ) {
                var ItEm = new AVFXEmitterCreate( "ItEm" );
                ItEm.Items.AddRange( Particles );
                ItEm.Items.AddRange( Emitters.GetRange( 0, i + 1 ) ); // get 1, then 2, etc.
                ItEm.Write( writer );
            }

            Data?.Write( writer );
        }

        private void SetData( EmitterType type ) {
            Data = type switch {
                EmitterType.Point => null,
                EmitterType.Cone => new AVFXEmitterDataCone(),
                EmitterType.ConeModel => new AVFXEmitterDataConeModel(),
                EmitterType.SphereModel => new AVFXEmitterDataSphereModel(),
                EmitterType.CylinderModel => new AVFXEmitterDataCylinderModel(),
                EmitterType.Model => new AVFXEmitterDataModel(),
                _ => null,
            };
        }

        public void SetType( EmitterType type ) {
            EmitterVariety.SetValue( type );
            Type = type;
            SetData( type );
            Data?.SetAssigned( true );
        }

        public AVFXEmitterItem AddParticle() {
            var ItPr = new AVFXEmitterItem();
            Particles.Add( ItPr );
            ParticleCount.SetValue( Particles.Count );
            return ItPr;
        }

        public void AddParticle( AVFXEmitterItem item ) {
            Particles.Add( item );
            ParticleCount.SetValue( Particles.Count );
        }

        public void RemoveParticle( int idx ) {
            Particles.RemoveAt( idx );
            ParticleCount.SetValue( Particles.Count );
        }

        public void RemoveParticle( AVFXEmitterItem item ) {
            Particles.Remove( item );
            ParticleCount.SetValue( Particles.Count );
        }

        public AVFXEmitterItem AddEmitter() {
            var ItEm = new AVFXEmitterItem();
            Emitters.Add( ItEm );
            EmitterCount.SetValue( Emitters.Count );
            return ItEm;
        }

        public void AddEmitter( AVFXEmitterItem item ) {
            Emitters.Add( item );
            EmitterCount.SetValue( Emitters.Count );
        }

        public void RemoveEmitter( int idx ) {
            Emitters.RemoveAt( idx );
            EmitterCount.SetValue( Emitters.Count );
        }

        public void RemoveEmitter( AVFXEmitterItem item ) {
            Emitters.Remove( item );
            EmitterCount.SetValue( Emitters.Count );
        }
    }
}
