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

        public LiteralEnum<EffectorType> EffectorVariety = new("EfVT");
        public LiteralEnum<RotationOrder> RotationOrder = new("RoOT");
        public LiteralEnum<CoordComputeOrder> CoordComputeOrder = new("CCOT");
        public LiteralBool AffectOtherVfx = new("bAOV");
        public LiteralBool AffectGame = new("bAGm");
        public LiteralInt LoopPointStart = new("LpSt");
        public LiteralInt LoopPointEnd = new("LpEd");

        public EffectorType Type;
        public AVFXEffectorData Data;
        readonly List<Base> Attributes;

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

            foreach (var item in node.Children){
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
            var effectorAvfx = new AVFXNode("Efct");
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
