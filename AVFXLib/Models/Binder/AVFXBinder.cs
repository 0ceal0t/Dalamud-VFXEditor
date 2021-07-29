using AVFXLib.AVFX;
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

        public LiteralBool StartToGlobalDirection = new("bStG");
        public LiteralBool VfxScaleEnabled = new("bVSc");
        public LiteralFloat VfxScaleBias = new("bVSb");
        public LiteralBool VfxScaleDepthOffset = new("bVSd");
        public LiteralBool VfxScaleInterpolation = new("bVSi");
        public LiteralBool TransformScale = new("bTSc");
        public LiteralBool TransformScaleDepthOffset = new("bTSd");
        public LiteralBool TransformScaleInterpolation = new("bTSi");
        public LiteralBool FollowingTargetOrientation = new("bFTO");
        public LiteralBool DocumentScaleEnabled = new("bDSE");
        public LiteralBool AdjustToScreenEnabled = new("bATS");
        public LiteralInt Life = new("Life");
        public LiteralEnum<BinderRotation> BinderRotationType = new("RoTp");
        public LiteralEnum<BinderType> BinderVariety = new("BnVr");
        public AVFXBinderProperty PropStart = new("PrpS");
        public AVFXBinderProperty Prop1 = new( "Prp1" );
        public AVFXBinderProperty Prop2 = new( "Prp2" );
        public AVFXBinderProperty PropGoal = new("PrpG");

        public BinderType Type;
        public AVFXBinderData Data;
        readonly List<Base> Attributes;

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

        public override void Read(AVFXNode node)
        {
            Assigned = true;
            ReadAVFX(Attributes, node);
            Type = BinderVariety.Value;

            foreach (var item in node.Children)
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

        public override AVFXNode ToAVFX()
        {
            var bindAvfx = new AVFXNode("Bind");
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
