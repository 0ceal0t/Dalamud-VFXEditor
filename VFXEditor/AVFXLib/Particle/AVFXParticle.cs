using System.Collections.Generic;
using System.IO;
using VfxEditor.AVFXLib.Curve;

namespace VfxEditor.AVFXLib.Particle {
    public class AVFXParticle : AVFXBase {
        public const string NAME = "Ptcl";

        public readonly AVFXInt LoopStart = new( "LpSt" );
        public readonly AVFXInt LoopEnd = new( "LpEd" );
        public readonly AVFXEnum<ParticleType> ParticleVariety = new( "PrVT" );
        public readonly AVFXEnum<RotationDirectionBase> RotationDirectionBaseType = new( "RBDT" );
        public readonly AVFXEnum<RotationOrder> RotationOrderType = new( "RoOT" );
        public readonly AVFXEnum<CoordComputeOrder> CoordComputeOrderType = new( "CCOT" );
        public readonly AVFXEnum<DrawMode> DrawModeType = new( "RMT" );
        public readonly AVFXEnum<CullingType> CullingTypeType = new( "CulT" );
        public readonly AVFXEnum<EnvLight> EnvLightType = new( "EnvT" );
        public readonly AVFXEnum<DirLight> DirLightType = new( "DirT" );
        public readonly AVFXEnum<UVPrecision> UvPrecisionType = new( "UVPT" );
        public readonly AVFXInt DrawPriority = new( "DwPr" );
        public readonly AVFXBool IsDepthTest = new( "DsDt" );
        public readonly AVFXBool IsDepthWrite = new( "DsDw" );
        public readonly AVFXBool IsSoftParticle = new( "DsSp" );
        public readonly AVFXInt CollisionType = new( "Coll" );
        public readonly AVFXBool Bs11 = new( "bS11" );
        public readonly AVFXBool IsApplyToneMap = new( "bATM" );
        public readonly AVFXBool IsApplyFog = new( "bAFg" );
        public readonly AVFXBool ClipNearEnable = new( "bNea" );
        public readonly AVFXBool ClipFarEnable = new( "bFar" );
        public readonly AVFXFloat ClipNearStart = new( "NeSt" );
        public readonly AVFXFloat ClipNearEnd = new( "NeEd" );
        public readonly AVFXFloat ClipFarStart = new( "FaSt" );
        public readonly AVFXFloat ClipFarEnd = new( "FaEd" );
        public readonly AVFXEnum<ClipBasePoint> ClipBasePointType = new( "FaBP" );
        public readonly AVFXInt UVSetCount = new( "UvSN" );
        public readonly AVFXInt ApplyRateEnvironment = new( "EvAR" );
        public readonly AVFXInt ApplyRateDirectional = new( "DlAR" );
        public readonly AVFXInt ApplyRateLightBuffer = new( "LBAR" );
        public readonly AVFXBool DOTy = new( "DOTy" );
        public readonly AVFXFloat DepthOffset = new( "DpOf" );
        public readonly AVFXBool SimpleAnimEnable = new( "bSCt" );
        public readonly AVFXLife Life = new();
        public readonly AVFXParticleSimple Simple = new();
        public readonly AVFXCurve Gravity = new( "Gra" );
        public readonly AVFXCurve GravityRandom = new( "GraR" );
        public readonly AVFXCurve AirResistance = new( "ARs" );
        public readonly AVFXCurve AirResistanceRandom = new( "ARsR" );
        public readonly AVFXCurve3Axis Scale = new( "Scl" );
        public readonly AVFXCurve3Axis Rotation = new( "Rot" );
        public readonly AVFXCurve3Axis Position = new( "Pos" );
        public readonly AVFXCurve RotVelX = new( "VRX" );
        public readonly AVFXCurve RotVelY = new( "VRY" );
        public readonly AVFXCurve RotVelZ = new( "VRZ" );
        public readonly AVFXCurve RotVelXRandom = new( "VRXR" );
        public readonly AVFXCurve RotVelYRandom = new( "VRYR" );
        public readonly AVFXCurve RotVelZRandom = new( "VRZR" );
        public readonly AVFXCurveColor Color = new();

        public readonly List<AVFXParticleUVSet> UvSets = new();

        public ParticleType Type;
        public AVFXBase Data;

        public readonly AVFXParticleTextureColor1 TC1 = new();
        public readonly AVFXParticleTextureColor2 TC2 = new( "TC2" );
        public readonly AVFXParticleTextureColor2 TC3 = new( "TC3" );
        public readonly AVFXParticleTextureColor2 TC4 = new( "TC4" );
        public readonly AVFXParticleTextureNormal TN = new();
        public readonly AVFXParticleTextureReflection TR = new();
        public readonly AVFXParticleTextureDistortion TD = new();
        public readonly AVFXParticleTexturePalette TP = new();

        private readonly List<AVFXBase> Children;
        private readonly List<AVFXBase> Children2;

        public AVFXParticle() : base( NAME ) {
            Children = new List<AVFXBase> {
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
                UVSetCount,
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
            };

            Children2 = new List<AVFXBase> {
                TC1,
                TC2,
                TC3,
                TC4,
                TN,
                TR,
                TD,
                TP
            };
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            Peek( reader, Children, size );
            Peek( reader, Children2, size );
            Type = ParticleVariety.GetValue();

            ReadNested( reader, ( BinaryReader _reader, string _name, int _size ) => {
                if( _name == "Data" ) {
                    SetData( Type );
                    Data?.Read( _reader, _size );
                }
                else if( _name == "UvSt" ) {
                    var uvSet = new AVFXParticleUVSet();
                    uvSet.Read( _reader, _size );
                    UvSets.Add( uvSet );
                }
            }, size );
        }

        protected override void RecurseChildrenAssigned( bool assigned ) {
            RecurseAssigned( Children, assigned );
            RecurseAssigned( Data, assigned );
            RecurseAssigned( Children2, assigned );
        }

        protected override void WriteContents( BinaryWriter writer ) {
            WriteNested( writer, Children );

            foreach( var uvSet in UvSets ) {
                uvSet.Write( writer );
            }

            Data?.Write( writer );
            WriteNested( writer, Children2 );
        }

        private void SetData( ParticleType type ) {
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

        public void SetType( ParticleType type ) {
            ParticleVariety.SetValue( type );
            Type = type;
            SetData( type );
            Data?.SetAssigned( true );
        }

        public AVFXParticleUVSet AddUvSet() {
            if( UvSets.Count >= 4 ) return null;
            var UvSet = new AVFXParticleUVSet();
            UvSets.Add( UvSet );
            UVSetCount.SetValue( UvSets.Count );
            return UvSet;
        }

        public void AddUvSet( AVFXParticleUVSet item ) {
            if( UvSets.Count >= 4 ) return;
            UvSets.Add( item );
            UVSetCount.SetValue( UvSets.Count );
        }

        public void RemoveUvSet( int idx ) {
            UvSets.RemoveAt( idx );
            UVSetCount.SetValue( UvSets.Count );
        }

        public void RemoveUvSet( AVFXParticleUVSet item ) {
            UvSets.Remove( item );
            UVSetCount.SetValue( UvSets.Count );
        }
    }
}
