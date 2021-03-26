using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXBinder : Base
    {
        public const string NAME = "Bind";

        public LiteralBool StartToGlobalDirection = new LiteralBool("bStG");
        public LiteralBool VfxScaleEnabled = new LiteralBool("bVSc");
        public LiteralFloat VfxScaleBias = new LiteralFloat("bVSb");
        public LiteralBool VfxScaleDepthOffset = new LiteralBool("bVSd");
        public LiteralBool VfxScaleInterpolation = new LiteralBool("bVSi");
        public LiteralBool TransformScale = new LiteralBool("bTSc");
        public LiteralBool TransformScaleDepthOffset = new LiteralBool("bTSd");
        public LiteralBool TransformScaleInterpolation = new LiteralBool("bTSi");
        public LiteralBool FollowingTargetOrientation = new LiteralBool("bFTO");
        public LiteralBool DocumentScaleEnabled = new LiteralBool("bDSE");
        public LiteralBool AdjustToScreenEnabled = new LiteralBool("bATS");
        public LiteralInt Life = new LiteralInt("Life");
        public LiteralEnum<BinderRotation> BinderRotationType = new LiteralEnum<BinderRotation>("RoTp");
        public LiteralEnum<BinderType> BinderVariety = new LiteralEnum<BinderType>("BnVr");
        public AVFXBinderProperty PropStart = new AVFXBinderProperty("PrpS");
        public AVFXBinderProperty Prop1 = new AVFXBinderProperty( "Prp1" );
        public AVFXBinderProperty Prop2 = new AVFXBinderProperty( "Prp2" );
        public AVFXBinderProperty PropGoal = new AVFXBinderProperty("PrpG");

        public BinderType Type;
        public AVFXBinderData Data;

        List<Base> Attributes;

        public AVFXBinder() : base(NAME)
        {
            Attributes = new List<Base>(new Base[]{
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
                Life,
                BinderRotationType,
                BinderVariety,
                PropStart,
                Prop1,
                Prop2,
                PropGoal
            });
        }

        public override void read(AVFXNode node)
        {
            Assigned = true;
            ReadAVFX(Attributes, node);
            Type = BinderVariety.Value;

            foreach (AVFXNode item in node.Children)
            {
                switch (item.Name)
                {
                    // DATA ====================
                    case AVFXParticleData.NAME:
                        SetType(Type);
                        ReadAVFX(Data, node);
                        break;
                }
            }
        }

        public override AVFXNode toAVFX()
        {
            AVFXNode bindAvfx = new AVFXNode("Bind");
            PutAVFX(bindAvfx, Attributes);
            PutAVFX(bindAvfx, Data);

            return bindAvfx;
        }

        public void SetVariety(BinderType type)
        {
            BinderVariety.GiveValue(type);
            Type = type;
            SetType(type);
            SetDefault(Data);
        }

        public void SetType(BinderType type)
        {
            switch (type)
            {
                case BinderType.Point:
                    Data = new AVFXBinderDataPoint();
                    break;
                case BinderType.Linear:
                    Data = new AVFXBinderDataLinear();
                    break;
                case BinderType.Spline:
                    Data = new AVFXBinderDataSpline();
                    break;
                case BinderType.Camera:
                    Data = new AVFXBinderDataCamera();
                    break;
                default:
                    Data = null;
                    break;
            }
        }
    }
}
