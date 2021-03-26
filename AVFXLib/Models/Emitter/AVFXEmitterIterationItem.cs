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
        public LiteralBool Enabled = new LiteralBool("bEnb");
        public LiteralInt TargetIdx = new LiteralInt("TgtB");
        public LiteralInt LocalDirection = new LiteralInt("LoDr");
        public LiteralInt CreateTime = new LiteralInt("CrTm");
        public LiteralInt CreateCount = new LiteralInt("CrCn");
        public LiteralInt CreateProbability = new LiteralInt("CrPr");
        public LiteralInt ParentInfluenceCoord = new LiteralInt("PICd");
        public LiteralInt ParentInfluenceColor = new LiteralInt("PICo");
        public LiteralInt InfluenceCoordScale = new LiteralInt("ICbS");
        public LiteralInt InfluenceCoordRot = new LiteralInt("ICbR");
        public LiteralInt InfluenceCoordPos = new LiteralInt("ICbP");
        public LiteralInt InfluenceCoordBinderPosition = new LiteralInt("ICbB");
        public LiteralInt InfluenceCoordUnstickiness = new LiteralInt("ICSK");
        public LiteralInt InheritParentVelocity = new LiteralInt("IPbV");
        public LiteralInt InheritParentLife = new LiteralInt("IPbL");
        public LiteralBool OverrideLife = new LiteralBool("bOvr");
        public LiteralInt OverrideLifeValue = new LiteralInt("OvrV");
        public LiteralInt OverrideLifeRandom = new LiteralInt("OvrR");
        public LiteralInt ParameterLink = new LiteralInt("PrLk");
        public LiteralInt StartFrame = new LiteralInt("StFr");
        public LiteralBool StartFrameNullUpdate = new LiteralBool("bStN");
        public LiteralFloat ByInjectionAngleX = new LiteralFloat("BIAX");
        public LiteralFloat ByInjectionAngleY = new LiteralFloat("BIAY");
        public LiteralFloat ByInjectionAngleZ = new LiteralFloat("BIAZ");
        public LiteralInt GenerateDelay = new LiteralInt("GenD");
        public LiteralBool GenerateDelayByOne = new LiteralBool("bGD");

        List<Base> Attributes;


        public AVFXEmitterIterationItem() : base("ItPr_Item")
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
            TargetIdx.GiveValue( -1 );
            Enabled.GiveValue( false );
            LocalDirection.GiveValue( 0 );
            CreateTime.GiveValue( 1 );
            CreateCount.GiveValue( 1 );
            CreateProbability.GiveValue( 100 );
            ParentInfluenceCoord.GiveValue( 1 );
            ParentInfluenceColor.GiveValue( 0 );
            InfluenceCoordScale.GiveValue( 0 );
            InfluenceCoordRot.GiveValue( 0 );
            InfluenceCoordPos.GiveValue( 1 );
            InfluenceCoordBinderPosition.GiveValue( 0 );
            InfluenceCoordUnstickiness.GiveValue( 0 );
            InheritParentVelocity.GiveValue( 0 );
            InheritParentLife.GiveValue( 0 );
            OverrideLife.GiveValue( false );
            OverrideLifeValue.GiveValue( 60 );
            OverrideLifeRandom.GiveValue( 0 );
            ParameterLink.GiveValue( -1 );
            StartFrame.GiveValue( 0 );
            StartFrameNullUpdate.GiveValue( false );
            ByInjectionAngleX.GiveValue( 0 );
            ByInjectionAngleY.GiveValue( 0 );
            ByInjectionAngleZ.GiveValue( 0 );
            GenerateDelay.GiveValue( 0 );
            GenerateDelayByOne.GiveValue( false );
        }

        public override AVFXNode toAVFX()
        {
            AVFXNode dataAvfx = new AVFXNode("ItPr_Item");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
