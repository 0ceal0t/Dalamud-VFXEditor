using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXParticle : Base
    {
        public const string NAME = "Ptcl";

        public LiteralInt LoopStart = new LiteralInt("LpSt");
        public LiteralInt LoopEnd = new LiteralInt("LpEd");
        public LiteralEnum<ParticleType> ParticleVariety = new LiteralEnum<ParticleType>("PrVT");
        public LiteralEnum<RotationDirectionBase> RotationDirectionBaseType = new LiteralEnum<RotationDirectionBase>("RBDT");
        public LiteralEnum<RotationOrder> RotationOrderType = new LiteralEnum<RotationOrder>("RoOT");
        public LiteralEnum<CoordComputeOrder> CoordComputeOrderType = new LiteralEnum<CoordComputeOrder>("CCOT");
        public LiteralEnum<DrawMode> DrawModeType = new LiteralEnum<DrawMode>("RMT");
        public LiteralEnum<CullingType> CullingTypeType = new LiteralEnum<CullingType>("CulT");
        public LiteralEnum<EnvLight> EnvLightType = new LiteralEnum<EnvLight>("EnvT");
        public LiteralEnum<DirLight> DirLightType = new LiteralEnum<DirLight>("DirT");
        public LiteralEnum<UVPrecision> UvPrecisionType = new LiteralEnum<UVPrecision>("UVPT");
        public LiteralInt DrawPriority = new LiteralInt("DwPr");
        public LiteralBool IsDepthTest = new LiteralBool("DsDt");
        public LiteralBool IsDepthWrite = new LiteralBool("DsDw");
        public LiteralBool IsSoftParticle = new LiteralBool("DsSp");
        public LiteralInt CollisionType = new LiteralInt("Coll");
        public LiteralBool Bs11 = new LiteralBool("bS11");
        public LiteralBool IsApplyToneMap = new LiteralBool("bATM");
        public LiteralBool IsApplyFog = new LiteralBool("bAFg");
        public LiteralBool ClipNearEnable = new LiteralBool("bNea");
        public LiteralBool ClipFarEnable = new LiteralBool("bFar");
        public LiteralFloat ClipNearStart = new LiteralFloat("NeSt");
        public LiteralFloat ClipNearEnd = new LiteralFloat("NeEd");
        public LiteralFloat ClipFarStart = new LiteralFloat("FaSt");
        public LiteralFloat ClipFarEnd = new LiteralFloat("FaEd");
        public LiteralEnum<ClipBasePoint> ClipBasePointType = new LiteralEnum<ClipBasePoint>("FaBP");
        public LiteralInt UvSetCount = new LiteralInt("UvSN");
        public LiteralInt ApplyRateEnvironment = new LiteralInt("EvAR");
        public LiteralInt ApplyRateDirectional = new LiteralInt("DlAR");
        public LiteralInt ApplyRateLightBuffer = new LiteralInt("LBAR");
        public LiteralBool DOTy = new LiteralBool("DOTy");
        public LiteralFloat DepthOffset = new LiteralFloat("DpOf");
        public LiteralBool SimpleAnimEnable = new LiteralBool("bSCt");
        public AVFXLife Life = new AVFXLife();
        public AVFXParticleSimple Simple = new AVFXParticleSimple();

        public AVFXCurve Gravity = new AVFXCurve("Gra");
        public AVFXCurve GravityRandom = new AVFXCurve("GraR");
        public AVFXCurve AirResistance = new AVFXCurve("ARs");
        public AVFXCurve AirResistanceRandom = new AVFXCurve("ARsR");

        public AVFXCurve3Axis Scale = new AVFXCurve3Axis("Scl");
        public AVFXCurve3Axis Rotation = new AVFXCurve3Axis("Rot");
        public AVFXCurve3Axis Position = new AVFXCurve3Axis("Pos");
        public AVFXCurve RotVelX = new AVFXCurve( "VRX" );
        public AVFXCurve RotVelY = new AVFXCurve( "VRY" );
        public AVFXCurve RotVelZ = new AVFXCurve( "VRZ" );
        public AVFXCurve RotVelXRandom = new AVFXCurve( "VRXR" );
        public AVFXCurve RotVelYRandom = new AVFXCurve( "VRYR" );
        public AVFXCurve RotVelZRandom = new AVFXCurve( "VRZR" );

        public AVFXCurveColor Color = new AVFXCurveColor();

        // UVSets
        //=====================//
        public List<AVFXParticleUVSet> UVSets = new List<AVFXParticleUVSet>();

        // Data
        //=====================//
        public ParticleType Type;
        public AVFXParticleData Data;

        // Texture Properties
        //====================//
        public AVFXTextureColor1 TC1 = new AVFXTextureColor1();
        public AVFXTextureColor2 TC2 = new AVFXTextureColor2("TC2");
        public AVFXTextureColor2 TC3 = new AVFXTextureColor2("TC3");
        public AVFXTextureColor2 TC4 = new AVFXTextureColor2("TC4");
        public AVFXTextureNormal TN = new AVFXTextureNormal();
        public AVFXTextureReflection TR = new AVFXTextureReflection();
        public AVFXTextureDistortion TD = new AVFXTextureDistortion();
        public AVFXTexturePalette TP = new AVFXTexturePalette();

        List<Base> Attributes;
        List<Base> Attributes2;

        public AVFXParticle() : base(NAME)
        {
            Attributes = new List<Base>(new Base[]{
                LoopStart,
                LoopEnd,
                ParticleVariety,
                RotationDirectionBaseType,
                RotationOrderType,
                CoordComputeOrderType,
                DrawModeType,
                CullingTypeType,
                EnvLightType,
                DirLightType,
                UvPrecisionType,
                DrawPriority,
                IsDepthTest,
                IsDepthWrite,
                IsSoftParticle,
                CollisionType,
                Bs11,
                IsApplyToneMap,
                IsApplyFog,
                ClipNearEnable,
                ClipFarEnable,
                ClipNearStart,
                ClipNearEnd,
                ClipFarStart,
                ClipFarEnd,
                ClipBasePointType,
                UvSetCount,
                ApplyRateEnvironment,
                ApplyRateDirectional,
                ApplyRateLightBuffer,
                DOTy,
                DepthOffset,
                SimpleAnimEnable,
                Life,
                Simple,
                Gravity,
                GravityRandom,
                AirResistance,
                AirResistanceRandom,
                Scale,
                Rotation,
                Position,
                RotVelX,
                RotVelY,
                RotVelZ,
                RotVelXRandom,
                RotVelYRandom,
                RotVelZRandom,
                Color
            });

            Attributes2 = new List<Base>(new Base[]{
                TC1,
                TC2,
                TC3,
                TC4,
                TN,
                TR,
                TD,
                TP
            });
        }

        public override void Read(AVFXNode node)
        {
            Assigned = true;
            ReadAVFX(Attributes, node);
            Type = ParticleVariety.Value;

            foreach (AVFXNode item in node.Children) 
            {
                switch (item.Name) {
                    // UVSET =================================
                    case AVFXParticleUVSet.NAME:
                        AVFXParticleUVSet UVSet = new AVFXParticleUVSet();
                        UVSet.Read(item);
                        UVSets.Add(UVSet);
                        break;

                    // DATA ==================================
                    case AVFXParticleData.NAME:
                        SetType(Type);
                        ReadAVFX(Data, node);
                        break;
                }
            }
            ReadAVFX(Attributes2, node);
        }

        public AVFXParticleUVSet AddUvSet()
        {
            if (UVSets.Count >= 4) return null;
            AVFXParticleUVSet UvSet = new AVFXParticleUVSet();
            UvSet.ToDefault();
            UVSets.Add(UvSet);
            UvSetCount.GiveValue(UVSets.Count());
            return UvSet;
        }
        public void AddUvSet(AVFXParticleUVSet item ) {
            if( UVSets.Count >= 4 ) return;
            UVSets.Add( item );
            UvSetCount.GiveValue( UVSets.Count() );
        }
        public void RemoveUvSet(int idx)
        {
            UVSets.RemoveAt(idx);
            UvSetCount.GiveValue(UVSets.Count());
        }
        public void RemoveUvSet(AVFXParticleUVSet item ) {
            UVSets.Remove( item );
            UvSetCount.GiveValue( UVSets.Count() );
        }

        public override AVFXNode ToAVFX()
        {
            AVFXNode ptclAvfx = new AVFXNode("Ptcl");

            PutAVFX(ptclAvfx, Attributes);

            // UVSets
            //=======================//
            foreach (AVFXParticleUVSet uvElem in UVSets)
            {
                PutAVFX(ptclAvfx, uvElem);
            }

            PutAVFX(ptclAvfx, Data);
            PutAVFX(ptclAvfx, Attributes2);

            return ptclAvfx;
        }

        public void SetVariety(ParticleType type)
        {
            ParticleVariety.GiveValue(type);
            Type = type;
            SetType(type);
            SetDefault(Data);
        }

        public void SetType(ParticleType type)
        {
            switch (type)
            {
                case ParticleType.Parameter:
                    Data = null;
                    break;
                case ParticleType.Powder:
                    Data = new AVFXParticleDataPowder();
                    break;
                case ParticleType.Windmill:
                    Data = new AVFXParticleDataWindmill();
                    break;
                case ParticleType.Line:
                    Data = new AVFXParticleDataLine();
                    break;
                case ParticleType.Model:
                    Data = new AVFXParticleDataModel();
                    break;
                case ParticleType.Polyline:
                    Data = new AVFXParticleDataPolyline();
                    break;
                case ParticleType.Quad:
                    Data = null;
                    break;
                case ParticleType.Polygon:
                    Data = new AVFXParticleDataPolygon();
                    break;
                case ParticleType.Decal:
                    Data = new AVFXParticleDataDecal();
                    break;
                case ParticleType.DecalRing:
                    Data = new AVFXParticleDataDecalRing();
                    break;
                case ParticleType.Disc:
                    Data = new AVFXParticleDataDisc();
                    break;
                case ParticleType.LightModel:
                    Data = new AVFXParticleDataLightModel();
                    break;
                case ParticleType.Laser:
                    Data = new AVFXParticleDataLaser();
                    break;
                default:
                    Data = null;
                    break;
            }
        }
    }
}
