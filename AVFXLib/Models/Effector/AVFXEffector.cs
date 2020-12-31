using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXEffector : Base
    {
        public const string NAME = "Efct";

        public LiteralEnum<EffectorType> EffectorVariety = new LiteralEnum<EffectorType>("effectorType", "EfVT");
        public LiteralEnum<RotationOrder> RotationOrder = new LiteralEnum<RotationOrder>("rotationOrder", "RoOT");
        public LiteralEnum<CoordComputeOrder> CoordComputeOrder = new LiteralEnum<CoordComputeOrder>("coordComputeOrder", "CCOT");
        public LiteralBool AffectOtherVfx = new LiteralBool("affectOtherVfx", "bAOV");
        public LiteralBool AffectGame = new LiteralBool("affectGame", "bAGm");
        public LiteralInt LoopPointStart = new LiteralInt("loopPointStart", "LpSt");
        public LiteralInt LoopPointEnd = new LiteralInt("loopPointEnd", "LpEd");

        public EffectorType Type;
        public AVFXEffectorData Data;

        List<Base> Attributes;

        public AVFXEffector() : base("effectors", NAME)
        {
            Attributes = new List<Base>(new Base[]{
                EffectorVariety,
                RotationOrder,
                CoordComputeOrder,
                AffectOtherVfx,
                AffectGame,
                LoopPointStart,
                LoopPointEnd
            });
        }

        public override void read(AVFXNode node)
        {
            Assigned = true;
            ReadAVFX(Attributes, node);
            Type = EffectorVariety.Value;

            foreach (AVFXNode item in node.Children){
                switch (item.Name){
                    // DATA ======================
                    case AVFXEffectorData.NAME:
                        SetType(Type);
                        ReadAVFX(Data, node);
                        break;
                }
            }
        }

        public override void toDefault()
        {
            Assigned = true;
            SetDefault(Attributes);
            SetVariety(EffectorVariety.Value);
        }

        public override JToken toJSON()
        {
            JObject elem = new JObject();
            PutJSON(elem, Attributes);
            PutJSON(elem, Data);
            return elem;
        }

        public override AVFXNode toAVFX()
        {
            AVFXNode effectorAvfx = new AVFXNode("Efct");
            PutAVFX(effectorAvfx, Attributes);

            PutAVFX(effectorAvfx, Data);
            return effectorAvfx;
        }

        public void SetVariety(EffectorType type)
        {
            EffectorVariety.GiveValue(type);
            Type = type;
            SetType(type);
            SetDefault(Data);
        }

        public void SetType(EffectorType type)
        {
            switch (type)
            {
                case EffectorType.PointLight:
                    Data = new AVFXEffectorDataPointLight("data");
                    break;
                case EffectorType.DirectionalLight:
                    throw new System.InvalidOperationException("Directional Light Effector!");
                case EffectorType.RadialBlur:
                    Data = new AVFXEffectorDataRadialBlur("data");
                    break;
                case EffectorType.BlackHole:
                    throw new System.InvalidOperationException("Black Hole Effector!");
                case EffectorType.CameraQuake:
                    Data = new AVFXEffectorDataCameraQuake("data");
                    break;
                default:
                    Data = null;
                    break;
            }
        }
    }
}
