using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models {
    public class AVFXParticle : Base {
        public const string NAME = "Ptcl";

        public LiteralInt LoopStart = new( "LpSt" );
        public LiteralInt LoopEnd = new( "LpEd" );
        public LiteralEnum<ParticleType> ParticleVariety = new( "PrVT" );
        public LiteralEnum<RotationDirectionBase> RotationDirectionBaseType = new( "RBDT" );
        public LiteralEnum<RotationOrder> RotationOrderType = new( "RoOT" );
        public LiteralEnum<CoordComputeOrder> CoordComputeOrderType = new( "CCOT" );
        public LiteralEnum<DrawMode> DrawModeType = new( "RMT" );
        public LiteralEnum<CullingType> CullingTypeType = new( "CulT" );
        public LiteralEnum<EnvLight> EnvLightType = new( "EnvT" );
        public LiteralEnum<DirLight> DirLightType = new( "DirT" );
        public LiteralEnum<UVPrecision> UvPrecisionType = new( "UVPT" );
        public LiteralInt DrawPriority = new( "DwPr" );
        public LiteralBool IsDepthTest = new( "DsDt" );
        public LiteralBool IsDepthWrite = new( "DsDw" );
        public LiteralBool IsSoftParticle = new( "DsSp" );
        public LiteralInt CollisionType = new( "Coll" );
        public LiteralBool Bs11 = new( "bS11" );
        public LiteralBool IsApplyToneMap = new( "bATM" );
        public LiteralBool IsApplyFog = new( "bAFg" );
        public LiteralBool ClipNearEnable = new( "bNea" );
        public LiteralBool ClipFarEnable = new( "bFar" );
        public LiteralFloat ClipNearStart = new( "NeSt" );
        public LiteralFloat ClipNearEnd = new( "NeEd" );
        public LiteralFloat ClipFarStart = new( "FaSt" );
        public LiteralFloat ClipFarEnd = new( "FaEd" );
        public LiteralEnum<ClipBasePoint> ClipBasePointType = new( "FaBP" );
        public LiteralInt UvSetCount = new( "UvSN" );
        public LiteralInt ApplyRateEnvironment = new( "EvAR" );
        public LiteralInt ApplyRateDirectional = new( "DlAR" );
        public LiteralInt ApplyRateLightBuffer = new( "LBAR" );
        public LiteralBool DOTy = new( "DOTy" );
        public LiteralFloat DepthOffset = new( "DpOf" );
        public LiteralBool SimpleAnimEnable = new( "bSCt" );
        public AVFXLife Life = new();
        public AVFXParticleSimple Simple = new();

        public AVFXCurve Gravity = new( "Gra" );
        public AVFXCurve GravityRandom = new( "GraR" );
        public AVFXCurve AirResistance = new( "ARs" );
        public AVFXCurve AirResistanceRandom = new( "ARsR" );

        public AVFXCurve3Axis Scale = new( "Scl" );
        public AVFXCurve3Axis Rotation = new( "Rot" );
        public AVFXCurve3Axis Position = new( "Pos" );
        public AVFXCurve RotVelX = new( "VRX" );
        public AVFXCurve RotVelY = new( "VRY" );
        public AVFXCurve RotVelZ = new( "VRZ" );
        public AVFXCurve RotVelXRandom = new( "VRXR" );
        public AVFXCurve RotVelYRandom = new( "VRYR" );
        public AVFXCurve RotVelZRandom = new( "VRZR" );

        public AVFXCurveColor Color = new();

        // UVSets
        //=====================//
        public List<AVFXParticleUVSet> UVSets = new();

        // Data
        //=====================//
        public ParticleType Type;
        public AVFXParticleData Data;

        // Texture Properties
        //====================//
        public AVFXTextureColor1 TC1 = new();
        public AVFXTextureColor2 TC2 = new( "TC2" );
        public AVFXTextureColor2 TC3 = new( "TC3" );
        public AVFXTextureColor2 TC4 = new( "TC4" );
        public AVFXTextureNormal TN = new();
        public AVFXTextureReflection TR = new();
        public AVFXTextureDistortion TD = new();
        public AVFXTexturePalette TP = new();
        private readonly List<Base> Attributes;
        private readonly List<Base> Attributes2;

        public AVFXParticle() : base( NAME ) {
            Attributes = new List<Base>( new Base[]{
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
            } );

            Attributes2 = new List<Base>( new Base[]{
                TC1,
                TC2,
                TC3,
                TC4,
                TN,
                TR,
                TD,
                TP
            } );
        }

        public override void Read( AVFXNode node ) {
            Assigned = true;
            ReadAVFX( Attributes, node );
            Type = ParticleVariety.Value;

            foreach( var item in node.Children ) {
                switch( item.Name ) {
                    // UVSET =================================
                    case AVFXParticleUVSet.NAME:
                        var UVSet = new AVFXParticleUVSet();
                        UVSet.Read( item );
                        UVSets.Add( UVSet );
                        break;

                    // DATA ==================================
                    case AVFXParticleData.NAME:
                        SetType( Type );
                        ReadAVFX( Data, node );
                        break;
                }
            }
            ReadAVFX( Attributes2, node );
        }

        public AVFXParticleUVSet AddUvSet() {
            if( UVSets.Count >= 4 ) return null;
            var UvSet = new AVFXParticleUVSet();
            UvSet.ToDefault();
            UVSets.Add( UvSet );
            UvSetCount.GiveValue( UVSets.Count );
            return UvSet;
        }
        public void AddUvSet( AVFXParticleUVSet item ) {
            if( UVSets.Count >= 4 ) return;
            UVSets.Add( item );
            UvSetCount.GiveValue( UVSets.Count );
        }
        public void RemoveUvSet( int idx ) {
            UVSets.RemoveAt( idx );
            UvSetCount.GiveValue( UVSets.Count );
        }
        public void RemoveUvSet( AVFXParticleUVSet item ) {
            UVSets.Remove( item );
            UvSetCount.GiveValue( UVSets.Count );
        }

        public override AVFXNode ToAVFX() {
            var ptclAvfx = new AVFXNode( "Ptcl" );

            PutAVFX( ptclAvfx, Attributes );

            // UVSets
            //=======================//
            foreach( var uvElem in UVSets ) {
                PutAVFX( ptclAvfx, uvElem );
            }

            PutAVFX( ptclAvfx, Data );
            PutAVFX( ptclAvfx, Attributes2 );

            return ptclAvfx;
        }

        public void SetVariety( ParticleType type ) {
            ParticleVariety.GiveValue( type );
            Type = type;
            SetType( type );
            SetDefault( Data );
        }

        public void SetType( ParticleType type ) {
            Data = type switch {
                ParticleType.Parameter => null,
                ParticleType.Powder => new AVFXParticleDataPowder(),
                ParticleType.Windmill => new AVFXParticleDataWindmill(),
                ParticleType.Line => new AVFXParticleDataLine(),
                ParticleType.Model => new AVFXParticleDataModel(),
                ParticleType.Polyline => new AVFXParticleDataPolyline(),
                ParticleType.Quad => null,
                ParticleType.Polygon => new AVFXParticleDataPolygon(),
                ParticleType.Decal => new AVFXParticleDataDecal(),
                ParticleType.DecalRing => new AVFXParticleDataDecalRing(),
                ParticleType.Disc => new AVFXParticleDataDisc(),
                ParticleType.LightModel => new AVFXParticleDataLightModel(),
                ParticleType.Laser => new AVFXParticleDataLaser(),
                _ => null,
            };
        }
    }
}
