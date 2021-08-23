using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models {
    public class AVFXEmitter : Base {
        public const string NAME = "Emit";

        public LiteralString Sound = new( "SdNm" );
        public LiteralInt SoundNumber = new( "SdNo" );
        public LiteralInt LoopStart = new( "LpSt" );
        public LiteralInt LoopEnd = new( "LpEd" );
        public LiteralInt ChildLimit = new( "ClCn" );
        public LiteralInt EffectorIdx = new( "EfNo" );
        public LiteralBool AnyDirection = new( "bAD", size: 1 );
        public LiteralEnum<EmitterType> EmitterVariety = new( "EVT" );
        public LiteralEnum<RotationDirectionBase> RotationDirectionBaseType = new( "RBDT" );
        public LiteralEnum<CoordComputeOrder> CoordComputeOrderType = new( "CCOT" );
        public LiteralEnum<RotationOrder> RotationOrderType = new( "ROT" );
        public LiteralInt ParticleCount = new( "PrCn" );
        public LiteralInt EmitterCount = new( "EmCn" );
        public AVFXLife Life = new();

        public AVFXCurve CreateCount = new( "CrC" );
        public AVFXCurve CreateCountRandom = new( "CrCR" );
        public AVFXCurve CreateInterval = new( "CrI" );
        public AVFXCurve CreateIntervalRandom = new( "CrIR" );
        public AVFXCurve Gravity = new( "Gra" );
        public AVFXCurve GravityRandom = new( "GraR" );
        public AVFXCurve AirResistance = new( "ARs" );
        public AVFXCurve AirResistanceRandom = new( "ARsR" );
        public AVFXCurveColor Color = new();
        public AVFXCurve3Axis Position = new( "Pos" );
        public AVFXCurve3Axis Rotation = new( "Rot" );
        public AVFXCurve3Axis Scale = new( "Scl" );

        public List<AVFXEmitterIterationItem> Particles = new();
        public List<AVFXEmitterIterationItem> Emitters = new();

        // Data
        //========================//
        public EmitterType Type;
        public AVFXEmitterData Data;
        private readonly List<Base> Attributes;

        public AVFXEmitter() : base( NAME ) {
            Assigned = true;
            Attributes = new List<Base>( new Base[]{
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
                Scale,
            } );
        }

        public override void Read( AVFXNode node ) {
            Assigned = true;
            ReadAVFX( Attributes, node );
            Type = EmitterVariety.Value;

            AVFXEmitterCreateParticle lastParticle = null;
            AVFXEmitterCreateEmitter lastEmitter = null;

            foreach( var item in node.Children ) {
                switch( item.Name ) {
                    // ITPR ==================
                    case AVFXEmitterCreateParticle.NAME:
                        lastParticle = new AVFXEmitterCreateParticle();
                        lastParticle.Read( item );
                        break;

                    // ItEm =================
                    case AVFXEmitterCreateEmitter.NAME:
                        lastEmitter = new AVFXEmitterCreateEmitter();
                        lastEmitter.Read( item );
                        break;

                    // DATA ================
                    case AVFXEmitterData.NAME:
                        SetType( Type );
                        ReadAVFX( Data, node );
                        break;
                }
            }

            if( lastParticle != null ) {
                Particles.AddRange( lastParticle.Items );
            }
            if( lastEmitter != null ) {
                var startIndex = Particles.Count;
                var emitterCount = lastEmitter.Items.Count - Particles.Count;
                Emitters.AddRange( lastEmitter.Items.GetRange( startIndex, emitterCount ) ); // remove particles
            }
        }

        public AVFXEmitterIterationItem AddParticle() {
            var ItPr = new AVFXEmitterIterationItem();
            ItPr.ToDefault();
            Particles.Add( ItPr );
            ParticleCount.GiveValue( Particles.Count );
            return ItPr;
        }
        public void AddParticle( AVFXEmitterIterationItem item ) {
            Particles.Add( item );
            ParticleCount.GiveValue( Particles.Count );
        }
        public void RemoveParticle( int idx ) {
            Particles.RemoveAt( idx );
            ParticleCount.GiveValue( Particles.Count );
        }
        public void RemoveParticle( AVFXEmitterIterationItem item ) {
            Particles.Remove( item );
            ParticleCount.GiveValue( Particles.Count );
        }
        //
        public AVFXEmitterIterationItem AddEmitter() {
            var ItEm = new AVFXEmitterIterationItem();
            ItEm.ToDefault();
            Emitters.Add( ItEm );
            EmitterCount.GiveValue( Emitters.Count );
            return ItEm;
        }
        public void AddEmitter( AVFXEmitterIterationItem item ) {
            Emitters.Add( item );
            EmitterCount.GiveValue( Emitters.Count );
        }
        public void RemoveEmitter( int idx ) {
            Emitters.RemoveAt( idx );
            EmitterCount.GiveValue( Emitters.Count );
        }
        public void RemoveEmitter( AVFXEmitterIterationItem item ) {
            Emitters.Remove( item );
            EmitterCount.GiveValue( Emitters.Count );
        }

        public override AVFXNode ToAVFX() {
            var emitAvfx = new AVFXNode( "Emit" );

            PutAVFX( emitAvfx, Attributes );

            // ITPR
            //=======================//
            for( var i = 0; i < Particles.Count; i++ ) {
                var ItPr = new AVFXEmitterCreateParticle {
                    Items = Particles.GetRange( 0, i + 1 )
                };
                emitAvfx.Children.Add( ItPr.ToAVFX() );
            }

            // ITEM
            //=======================//
            for( var i = 0; i < Emitters.Count; i++ ) {
                var ItEM = new AVFXEmitterCreateEmitter();
                ItEM.Items.AddRange( Particles );
                ItEM.Items.AddRange( Emitters.GetRange( 0, i + 1 ) ); // get 1, then 2, etc.
                emitAvfx.Children.Add( ItEM.ToAVFX() );
            }

            PutAVFX( emitAvfx, Data );

            return emitAvfx;
        }

        public void SetVariety( EmitterType type ) {
            EmitterVariety.GiveValue( type );
            Type = type;
            SetType( type );
            SetDefault( Data );
        }

        public void SetType( EmitterType type ) {
            Data = type switch {
                // no data here :)
                EmitterType.Point => null,
                EmitterType.Cone => new AVFXEmitterDataCone(),
                EmitterType.ConeModel => new AVFXEmitterDataConeModel(),
                EmitterType.SphereModel => new AVFXEmitterDataSphereModel(),
                EmitterType.CylinderModel => new AVFXEmitterDataCylinderModel(),
                EmitterType.Model => new AVFXEmitterDataModel(),
                _ => null,
            };
        }
    }
}
