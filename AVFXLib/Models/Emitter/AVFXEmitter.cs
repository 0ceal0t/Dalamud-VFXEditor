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

        public LiteralString Sound = new LiteralString("sound", "SdNm");
        public LiteralInt SoundNumber = new LiteralInt("soundNumber", "SdNo");
        public LiteralInt LoopStart = new LiteralInt("loopStart", "LpSt");
        public LiteralInt LoopEnd = new LiteralInt("loopEnd", "LpEd");
        public LiteralInt ChildLimit = new LiteralInt("childLimit", "ClCn");
        public LiteralInt EffectorIdx = new LiteralInt("effectorIdx", "EfNo");
        public LiteralBool AnyDirection = new LiteralBool("anyDirection", "bAD", size:1);
        public LiteralEnum<EmitterType> EmitterVariety = new LiteralEnum<EmitterType>("emitterType", "EVT");
        public LiteralEnum<RotationDirectionBase> RotationDirectionBaseType = new LiteralEnum<RotationDirectionBase>("rotationDirectionBase", "RBDT");
        public LiteralEnum<CoordComputeOrder> CoordComputeOrderType = new LiteralEnum<CoordComputeOrder>("coordComputeOrder", "CCOT");
        public LiteralEnum<RotationOrder> RotationOrderType = new LiteralEnum<RotationOrder>("rotationOrder", "ROT");
        public LiteralInt ParticleCount = new LiteralInt("particleCount", "PrCn");
        public LiteralInt EmitterCount = new LiteralInt("emitterCount", "EmCn");
        public AVFXLife Life = new AVFXLife("life");

        public AVFXCurve CreateCount = new AVFXCurve("createCount", "CrC");
        public AVFXCurve CreateCountRandom = new AVFXCurve("createCountRandom", "CrCR");
        public AVFXCurve CreateInterval = new AVFXCurve("createInterval", "CrI");
        public AVFXCurve CreateIntervalRandom = new AVFXCurve("createIntervalRandom", "CrIR");
        public AVFXCurve Gravity = new AVFXCurve("gravity", "Gra");
        public AVFXCurve GravityRandom = new AVFXCurve("gravityRandom", "GraR");
        public AVFXCurve AirResistance = new AVFXCurve("airResistance", "ARs");
        public AVFXCurve AirResistanceRandom = new AVFXCurve("airResistanceRandom", "ARsR");
        public AVFXCurveColor Color = new AVFXCurveColor("color");
        public AVFXCurve3Axis Position = new AVFXCurve3Axis("position", "Pos");
        public AVFXCurve3Axis Rotation = new AVFXCurve3Axis("rotation", "Rot");
        public AVFXCurve3Axis Scale = new AVFXCurve3Axis("scale", "Scl");

        public List<AVFXEmitterIterationItem> Particles = new List<AVFXEmitterIterationItem>();
        public List<AVFXEmitterIterationItem> Emitters = new List<AVFXEmitterIterationItem>();

        // Data
        //========================//
        public EmitterType Type;
        public AVFXEmitterData Data;

        List<Base> Attributes;

        public AVFXEmitter() : base("emitter", NAME)
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
                    // ItEm =================
                    case AVFXEmitterCreateEmitter.NAME:
                        lastEmitter = new AVFXEmitterCreateEmitter();
                        lastEmitter.read(item);
                        break;

                    // ITPR ==================
                    case AVFXEmitterCreateParticle.NAME:
                        lastParticle = new AVFXEmitterCreateParticle();
                        lastParticle.read(item);
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
                Emitters.AddRange(lastEmitter.Items);
            }
        }

        public override void toDefault()
        {
            Assigned = true;
            SetUnAssigned(Attributes);
            SoundNumber.GiveValue(-1);
            LoopStart.GiveValue(0);
            LoopEnd.GiveValue(0);
            ChildLimit.GiveValue(30);
            EffectorIdx.GiveValue(-1);
            AnyDirection.GiveValue(false);
            EmitterVariety.GiveValue(EmitterType.Point);
            RotationDirectionBaseType.GiveValue(RotationDirectionBase.Z);
            CoordComputeOrderType.GiveValue(CoordComputeOrder.Scale_Rot_Translate);
            RotationOrderType.GiveValue(RotationOrder.ZXY);
            ParticleCount.GiveValue(0);
            EmitterCount.GiveValue(0);
            Particles = new List<AVFXEmitterIterationItem>();
            Emitters = new List<AVFXEmitterIterationItem>();
            SetDefault(Life);

            SetVariety(EmitterVariety.Value);
        }

        public void addParticle()
        {
            AVFXEmitterIterationItem ItPr = new AVFXEmitterIterationItem();
            ItPr.toDefault();
            Particles.Add(ItPr);
            ParticleCount.GiveValue(Particles.Count());
        }
        public void removeParticle(int idx)
        {
            Particles.RemoveAt(idx);
            ParticleCount.GiveValue(Particles.Count());
        }
        public void addEmitter()
        {
            AVFXEmitterIterationItem ItEm = new AVFXEmitterIterationItem();
            ItEm.toDefault();
            Emitters.Add(ItEm);
            EmitterCount.GiveValue(Emitters.Count());
        }
        public void removeEmitter(int idx)
        {
            Emitters.RemoveAt(idx);
            EmitterCount.GiveValue(Emitters.Count());
        }

        public override JToken toJSON()
        {
            JObject elem = new JObject();
            PutJSON(elem, Attributes);

            // ItEm ========
            JArray itemArray = new JArray();
            foreach(var e in Emitters)
            {
                itemArray.Add(e.toJSON());
            }
            elem["emitters"] = itemArray;

            // ItPr ========
            JArray itPrArray = new JArray();
            foreach (var p in Particles)
            {
                itPrArray.Add(p.toJSON());
            }
            elem["particles"] = itPrArray;

            PutJSON(elem, Data);
            return elem;
        }

        public override AVFXNode toAVFX()
        {
            AVFXNode emitAvfx = new AVFXNode("Emit");

            PutAVFX(emitAvfx, Attributes);

            // ITEM
            //=======================//
            for (int i = 0; i < Emitters.Count; i++)
            {
                AVFXEmitterCreateEmitter ItEM = new AVFXEmitterCreateEmitter();
                ItEM.Items = Emitters.GetRange(0, i + 1); // get 1, then 2, etc.
                emitAvfx.Children.Add(ItEM.toAVFX());
            }

            // ITPR
            //=======================//
            for (int i = 0; i < Particles.Count; i++)
            {
                AVFXEmitterCreateParticle ItPr = new AVFXEmitterCreateParticle();
                ItPr.Items = Particles.GetRange(0, i + 1);
                emitAvfx.Children.Add(ItPr.toAVFX());
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
                    throw new System.InvalidOperationException("Cone Emitter!");
                case EmitterType.ConeModel:
                    throw new System.InvalidOperationException("Cone Model Emitter!");
                case EmitterType.SphereModel:
                    Data = new AVFXEmitterDataSphereModel("data");
                    break;
                case EmitterType.CylinderModel:
                    Data = new AVFXEmitterDataCylinderModel("data");
                    break;
                case EmitterType.Model:
                    Data = new AVFXEmitterDataModel("data");
                    break;
                default:
                    Data = null;
                    break;
            }
        }
    }
}
