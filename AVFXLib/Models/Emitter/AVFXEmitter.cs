using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXEmitter : Base
    {
        public const string NAME = "Emit";

        public LiteralString Sound = new LiteralString("SdNm");
        public LiteralInt SoundNumber = new LiteralInt( "SdNo");
        public LiteralInt LoopStart = new LiteralInt("LpSt");
        public LiteralInt LoopEnd = new LiteralInt("LpEd");
        public LiteralInt ChildLimit = new LiteralInt("ClCn");
        public LiteralInt EffectorIdx = new LiteralInt("EfNo");
        public LiteralBool AnyDirection = new LiteralBool("bAD", size:1);
        public LiteralEnum<EmitterType> EmitterVariety = new LiteralEnum<EmitterType>("EVT");
        public LiteralEnum<RotationDirectionBase> RotationDirectionBaseType = new LiteralEnum<RotationDirectionBase>("RBDT");
        public LiteralEnum<CoordComputeOrder> CoordComputeOrderType = new LiteralEnum<CoordComputeOrder>("CCOT");
        public LiteralEnum<RotationOrder> RotationOrderType = new LiteralEnum<RotationOrder>("ROT");
        public LiteralInt ParticleCount = new LiteralInt("PrCn");
        public LiteralInt EmitterCount = new LiteralInt("EmCn");
        public AVFXLife Life = new AVFXLife();

        public AVFXCurve CreateCount = new AVFXCurve("CrC");
        public AVFXCurve CreateCountRandom = new AVFXCurve("CrCR");
        public AVFXCurve CreateInterval = new AVFXCurve("CrI");
        public AVFXCurve CreateIntervalRandom = new AVFXCurve("CrIR");
        public AVFXCurve Gravity = new AVFXCurve("Gra");
        public AVFXCurve GravityRandom = new AVFXCurve("GraR");
        public AVFXCurve AirResistance = new AVFXCurve("ARs");
        public AVFXCurve AirResistanceRandom = new AVFXCurve("ARsR");
        public AVFXCurveColor Color = new AVFXCurveColor();
        public AVFXCurve3Axis Position = new AVFXCurve3Axis("Pos");
        public AVFXCurve3Axis Rotation = new AVFXCurve3Axis("Rot");
        public AVFXCurve3Axis Scale = new AVFXCurve3Axis("Scl");

        public List<AVFXEmitterIterationItem> Particles = new List<AVFXEmitterIterationItem>();
        public List<AVFXEmitterIterationItem> Emitters = new List<AVFXEmitterIterationItem>();

        // Data
        //========================//
        public EmitterType Type;
        public AVFXEmitterData Data;

        List<Base> Attributes;

        public AVFXEmitter() : base(NAME)
        {
            Assigned = true;
            Attributes = new List<Base>(new Base[]{
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
            });
        }

        public override void read(AVFXNode node)
        {
            Assigned = true;
            ReadAVFX(Attributes, node);
            Type = EmitterVariety.Value;

            AVFXEmitterCreateParticle lastParticle = null;
            AVFXEmitterCreateEmitter lastEmitter = null;

            foreach (AVFXNode item in node.Children)
            {
                switch (item.Name){
                    // ITPR ==================
                    case AVFXEmitterCreateParticle.NAME:
                        lastParticle = new AVFXEmitterCreateParticle();
                        lastParticle.read(item);
                        break;

                    // ItEm =================
                    case AVFXEmitterCreateEmitter.NAME:
                        lastEmitter = new AVFXEmitterCreateEmitter();
                        lastEmitter.read( item );
                        break;

                    // DATA ================
                    case AVFXEmitterData.NAME:
                        SetType(Type);
                        ReadAVFX(Data, node);
                        break;
                }
            }

            if(lastParticle != null)
            {
                Particles.AddRange(lastParticle.Items);
            }
            if(lastEmitter != null)
            {
                int startIndex = Particles.Count();
                int emitterCount = lastEmitter.Items.Count() - Particles.Count();
                Emitters.AddRange(lastEmitter.Items.GetRange(startIndex, emitterCount)); // remove particles
            }
        }

        public AVFXEmitterIterationItem addParticle()
        {
            AVFXEmitterIterationItem ItPr = new AVFXEmitterIterationItem();
            ItPr.toDefault();
            Particles.Add(ItPr);
            ParticleCount.GiveValue(Particles.Count());
            return ItPr;
        }
        public void addParticle(AVFXEmitterIterationItem item ) {
            Particles.Add( item );
            ParticleCount.GiveValue( Particles.Count() );
        }
        public void removeParticle(int idx)
        {
            Particles.RemoveAt(idx);
            ParticleCount.GiveValue(Particles.Count());
        }
        public void removeParticle(AVFXEmitterIterationItem item ) {
            Particles.Remove( item );
            ParticleCount.GiveValue( Particles.Count() );
        }
        //
        public AVFXEmitterIterationItem addEmitter()
        {
            AVFXEmitterIterationItem ItEm = new AVFXEmitterIterationItem();
            ItEm.toDefault();
            Emitters.Add(ItEm);
            EmitterCount.GiveValue(Emitters.Count());
            return ItEm;
        }
        public void addEmitter( AVFXEmitterIterationItem item ) {
            Emitters.Add( item );
            EmitterCount.GiveValue( Emitters.Count() );
        }
        public void removeEmitter(int idx)
        {
            Emitters.RemoveAt(idx);
            EmitterCount.GiveValue(Emitters.Count());
        }
        public void removeEmitter( AVFXEmitterIterationItem item ) {
            Emitters.Remove( item );
            EmitterCount.GiveValue( Emitters.Count() );
        }

        public override AVFXNode toAVFX()
        {
            AVFXNode emitAvfx = new AVFXNode("Emit");

            PutAVFX(emitAvfx, Attributes);

            // ITPR
            //=======================//
            for (int i = 0; i < Particles.Count; i++)
            {
                AVFXEmitterCreateParticle ItPr = new AVFXEmitterCreateParticle();
                ItPr.Items = Particles.GetRange(0, i + 1);
                emitAvfx.Children.Add(ItPr.toAVFX());
            }

            // ITEM
            //=======================//
            for( int i = 0; i < Emitters.Count; i++ )
            {
                AVFXEmitterCreateEmitter ItEM = new AVFXEmitterCreateEmitter();
                ItEM.Items.AddRange(Particles);
                ItEM.Items.AddRange(Emitters.GetRange( 0, i + 1 )); // get 1, then 2, etc.
                emitAvfx.Children.Add( ItEM.toAVFX() );
            }

            PutAVFX(emitAvfx, Data);

            return emitAvfx;
        }

        public void SetVariety(EmitterType type)
        {
            EmitterVariety.GiveValue(type);
            Type = type;
            SetType(type);
            SetDefault(Data);
        }

        public void SetType(EmitterType type)
        {
            switch (type)
            {
                case EmitterType.Point: // no data here :)
                    Data = null;
                    break;
                case EmitterType.Cone:
                    Data = new AVFXEmitterDataCone();
                    break;
                case EmitterType.ConeModel:
                    Data = new AVFXEmitterDataConeModel();
                    break;
                case EmitterType.SphereModel:
                    Data = new AVFXEmitterDataSphereModel();
                    break;
                case EmitterType.CylinderModel:
                    Data = new AVFXEmitterDataCylinderModel();
                    break;
                case EmitterType.Model:
                    Data = new AVFXEmitterDataModel();
                    break;
                default:
                    Data = null;
                    break;
            }
        }
    }
}
