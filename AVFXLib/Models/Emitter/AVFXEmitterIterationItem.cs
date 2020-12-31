using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXEmitterIterationItem : Base
    {
        public LiteralBool Enabled = new LiteralBool("enabled", "bEnb");
        public LiteralInt TargetIdx = new LiteralInt("targetIdx", "TgtB");
        public LiteralInt LocalDirection = new LiteralInt("localDirection", "LoDr");
        public LiteralInt CreateTime = new LiteralInt("createTime", "CrTm");
        public LiteralInt CreateCount = new LiteralInt("createCount", "CrCn");
        public LiteralInt CreateProbability = new LiteralInt("createProbability", "CrPr");
        public LiteralInt ParentInfluenceCoord = new LiteralInt("parentInfluenceCoord", "PICd");
        public LiteralInt ParentInfluenceColor = new LiteralInt("parentInfluenceColor", "PICo");
        public LiteralInt InfluenceCoordScale = new LiteralInt("influenceCoordScale", "ICbS");
        public LiteralInt InfluenceCoordRot = new LiteralInt("influenceCoordRot", "ICbR");
        public LiteralInt InfluenceCoordPos = new LiteralInt("influenceCoordPos", "ICbP");
        public LiteralInt InfluenceCoordBinderPosition = new LiteralInt("influenceCoordBinderPosition", "ICbB");
        public LiteralInt InfluenceCoordUnstickiness = new LiteralInt("influenceCoordUnstickiness", "ICSK");
        public LiteralInt InheritParentVelocity = new LiteralInt("inheritParentVelocity", "IPbV");
        public LiteralInt InheritParentLife = new LiteralInt("inheritParentLife", "IPbL");
        public LiteralBool OverrideLife = new LiteralBool("overrideLife", "bOvr");
        public LiteralInt OverrideLifeValue = new LiteralInt("overrideLifeValue", "OvrV");
        public LiteralInt OverrideLifeRandom = new LiteralInt("overrideLifeRandom", "OvrR");
        public LiteralInt ParameterLink = new LiteralInt("parameterLink", "PrLk");
        public LiteralInt StartFrame = new LiteralInt("startFrame", "StFr");
        public LiteralBool StartFrameNullUpdate = new LiteralBool("startFrameNullUpdate", "bStN");
        public LiteralFloat ByInjectionAngleX = new LiteralFloat("byInjectionAngleX", "BIAX");
        public LiteralFloat ByInjectionAngleY = new LiteralFloat("byInjectionAngleY", "BIAY");
        public LiteralFloat ByInjectionAngleZ = new LiteralFloat("byInjectionAngleZ", "BIAZ");
        public LiteralInt GenerateDelay = new LiteralInt("generateDelay", "GenD");
        public LiteralBool GenerateDelayByOne = new LiteralBool("generateDelayByOne", "bGD");

        List<Base> Attributes;


        public AVFXEmitterIterationItem() : base("items", "ItPr_Item")
        {
            Attributes = new List<Base>(new Base[]{
                Enabled,
                TargetIdx,
                LocalDirection,
                CreateTime,
                CreateCount,
                CreateProbability,
                ParentInfluenceCoord,
                ParentInfluenceColor,
                InfluenceCoordScale,
                InfluenceCoordRot,
                InfluenceCoordPos,
                InfluenceCoordBinderPosition,
                InfluenceCoordUnstickiness,
                InheritParentVelocity,
                InheritParentLife,
                OverrideLife,
                OverrideLifeValue,
                OverrideLifeRandom,
                ParameterLink,
                StartFrame,
                StartFrameNullUpdate,
                ByInjectionAngleX,
                ByInjectionAngleY,
                ByInjectionAngleZ,
                GenerateDelay,
                GenerateDelayByOne
            });
        }

        public override void read(AVFXNode node)
        {
            Assigned = true;
            ReadAVFX(Attributes, node);
        }

        public override void toDefault()
        {
            Assigned = true;
            SetDefault(Attributes);
            Enabled.GiveValue(true);
            TargetIdx.GiveValue(-1);
        }

        public override JToken toJSON()
        {
            JObject elem = new JObject();
            PutJSON(elem, Attributes);
            return elem;
        }

        public override AVFXNode toAVFX()
        {
            AVFXNode dataAvfx = new AVFXNode("ItPr_Item");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
