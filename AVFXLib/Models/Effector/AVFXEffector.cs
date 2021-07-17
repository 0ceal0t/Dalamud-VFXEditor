using AVFXLib.AVFX;
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

        public LiteralEnum<EffectorType> EffectorVariety = new LiteralEnum<EffectorType>("EfVT");
        public LiteralEnum<RotationOrder> RotationOrder = new LiteralEnum<RotationOrder>("RoOT");
        public LiteralEnum<CoordComputeOrder> CoordComputeOrder = new LiteralEnum<CoordComputeOrder>("CCOT");
        public LiteralBool AffectOtherVfx = new LiteralBool("bAOV");
        public LiteralBool AffectGame = new LiteralBool("bAGm");
        public LiteralInt LoopPointStart = new LiteralInt("LpSt");
        public LiteralInt LoopPointEnd = new LiteralInt("LpEd");

        public EffectorType Type;
        public AVFXEffectorData Data;

        List<Base> Attributes;

        public AVFXEffector() : base(NAME)
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

        public override void Read(AVFXNode node)
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

        public override AVFXNode ToAVFX()
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
                    Data = new AVFXEffectorDataPointLight();
                    break;
                case EffectorType.DirectionalLight:
                    Data = new AVFXEffectorDataDirectionalLight();
                    break;
                case EffectorType.RadialBlur:
                    Data = new AVFXEffectorDataRadialBlur();
                    break;
                case EffectorType.BlackHole:
                    Data = null;
                    break;
                case EffectorType.CameraQuake2_Unknown:
                case EffectorType.CameraQuake:
                    Data = new AVFXEffectorDataCameraQuake();
                    break;
                default:
                    Data = null;
                    break;
            }
        }
    }
}
