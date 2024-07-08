using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.AvfxFormat.Nodes;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxParticle : AvfxNodeWithData<ParticleType> {
        public const string NAME = "Ptcl";

        public readonly AvfxInt LoopStart = new( "Loop Start", "LpSt" );
        public readonly AvfxInt LoopEnd = new( "Loop End", "LpEd" );
        public readonly AvfxEnum<RotationDirectionBase> RotationDirectionBaseType = new( "Rotation Direction Base", "RBDT" );
        public readonly AvfxEnum<RotationOrder> RotationOrderType = new( "Rotation Compute Order", "RoOT" );
        public readonly AvfxEnum<CoordComputeOrder> CoordComputeOrderType = new( "Coord Compute Order", "CCOT" );
        public readonly AvfxEnum<DrawMode> DrawModeType = new( "Draw Mode", "RMT" );
        public readonly AvfxEnum<CullingType> CullingTypeType = new( "Culling Type", "CulT" );
        public readonly AvfxEnum<EnvLight> EnvLightType = new( "Enviornmental Light", "EnvT" );
        public readonly AvfxEnum<DirLight> DirLightType = new( "Directional Light", "DirT" );
        public readonly AvfxEnum<UvPrecision> UvPrecisionType = new( "UV Precision", "UVPT" );
        public readonly AvfxInt DrawPriority = new( "Draw Priority", "DwPr" );
        public readonly AvfxBool IsDepthTest = new( "Depth Test", "DsDt" );
        public readonly AvfxBool IsDepthWrite = new( "Depth Write", "DsDw" );
        public readonly AvfxBool IsSoftParticle = new( "Soft Particle", "DsSp" );
        public readonly AvfxInt CollisionType = new( "Collision Type", "Coll" );
        public readonly AvfxBool S11Enabled = new( "S11 Enabled", "bS11" );

        // New to dawntrail
        public readonly AvfxInt ShUT = new( "ShUT", "ShUT" );
        public readonly AvfxInt ShR = new( "ShR", "ShR" );
        public readonly AvfxInt ShT = new( "ShT", "ShT" );
        public readonly AvfxInt UniV = new( "UniV", "UniV" );
        public readonly AvfxInt HybV = new( "HybV", "HybV" );
        public readonly AvfxBool E24Enabled = new( "E24 Enabled", "bE24" );

        public readonly AvfxBool IsApplyToneMap = new( "Apply Tone Map", "bATM" );
        public readonly AvfxBool IsApplyFog = new( "Apply Fog", "bAFg" );
        public readonly AvfxBool ClipNearEnable = new( "Enable Clip Near", "bNea" );
        public readonly AvfxBool ClipFarEnable = new( "Enable Clip Far", "bFar" );
        public readonly AvfxFloat ClipNearStart = new( "Clip Near Start", "NeSt" ); // float2
        public readonly AvfxFloat ClipNearEnd = new( "Clip Near End", "NeEd" );
        public readonly AvfxFloat ClipFarStart = new( "Clip Far Start", "FaSt" ); // float2
        public readonly AvfxFloat ClipFarEnd = new( "Clip Far End", "FaEd" );
        public readonly AvfxEnum<ClipBasePoint> ClipBasePointType = new( "Clip Base Point", "FaBP" );
        public readonly AvfxInt UvSetCount = new( "UV Set Count", "UvSN" );
        public readonly AvfxInt ApplyRateEnvironment = new( "Apply Rate Environment", "EvAR" );
        public readonly AvfxInt ApplyRateDirectional = new( "Apply Rate Directional", "DlAR" );
        public readonly AvfxInt ApplyRateLightBuffer = new( "Apply Rate Light Buffer", "LBAR" );
        public readonly AvfxBool DOTy = new( "DOTy", "DOTy" );
        public readonly AvfxFloat DepthOffset = new( "Depth Offset", "DpOf" );
        public readonly AvfxBool SimpleAnimEnable = new( "Use Simple Animation", "bSCt" );
        public readonly AvfxLife Life = new();
        public readonly AvfxCurve Gravity = new( "Gravity", "Gra" );
        public readonly AvfxCurve GravityRandom = new( "Gravity Random", "GraR" );
        public readonly AvfxCurve AirResistance = new( "Air Resistance", "ARs", locked: true );
        public readonly AvfxCurve AirResistanceRandom = new( "Air Resistance Random", "ARsR", locked: true );
        public readonly AvfxCurve3Axis Scale = new( "Scale", "Scl", locked: true );
        public readonly AvfxCurve3Axis Rotation = new( "Rotation", "Rot", CurveType.Angle, locked: true );
        public readonly AvfxCurve3Axis Position = new( "Position", "Pos", locked: true );
        public readonly AvfxCurve RotVelX = new( "Rotation Velocity X", "VRX" );
        public readonly AvfxCurve RotVelY = new( "Rotation Velocity Y", "VRY" );
        public readonly AvfxCurve RotVelZ = new( "Rotation Velocity Z", "VRZ" );
        public readonly AvfxCurve RotVelXRandom = new( "Rotation Velocity X Random", "VRXR" );
        public readonly AvfxCurve RotVelYRandom = new( "Rotation Velocity Y Random", "VRYR" );
        public readonly AvfxCurve RotVelZRandom = new( "Rotation Velocity Z Random", "VRZR" );
        public readonly AvfxCurveColor Color = new( "Color", locked: true );

        // initialize these later
        public readonly AvfxParticleTextureColor1 TC1;
        public readonly AvfxParticleTextureColor2 TC2;
        public readonly AvfxParticleTextureColor2 TC3;
        public readonly AvfxParticleTextureColor2 TC4;
        public readonly AvfxParticleTextureNormal TN;
        public readonly AvfxParticleTextureReflection TR;
        public readonly AvfxParticleTextureDistortion TD;
        public readonly AvfxParticleTexturePalette TP;
        public readonly AvfxParticleSimple Simple;

        public readonly List<AvfxParticleUvSet> UvSets = [];
        public readonly UiUvSetSplitView UvView;

        private readonly List<AvfxBase> Parsed;
        private readonly List<AvfxBase> Parsed2;

        public readonly AvfxNodeGroupSet NodeGroups;
        public readonly AvfxDisplaySplitView<AvfxItem> AnimationSplitDisplay;
        public readonly AvfxDisplaySplitView<AvfxItem> TextureDisplaySplit;
        private readonly UiDisplayList Parameters;

        public AvfxParticle( AvfxNodeGroupSet groupSet ) : base( NAME, AvfxNodeGroupSet.ParticleColor, "PrVT" ) {
            NodeGroups = groupSet;

            // Initialize the remaining ones

            TC1 = new AvfxParticleTextureColor1( this );
            TC2 = new AvfxParticleTextureColor2( "Texture Color 2", "TC2", this );
            TC3 = new AvfxParticleTextureColor2( "Texture Color 3", "TC3", this );
            TC4 = new AvfxParticleTextureColor2( "Texture Color 4", "TC4", this );
            TN = new AvfxParticleTextureNormal( this );
            TR = new AvfxParticleTextureReflection( this );
            TD = new AvfxParticleTextureDistortion( this );
            TP = new AvfxParticleTexturePalette( this );
            Simple = new AvfxParticleSimple( this );

            // Parsing

            Parsed = [
                LoopStart,
                LoopEnd,
                Type,
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
                S11Enabled,
                ShUT,
                ShR,
                ShT,
                UniV,
                HybV,
                E24Enabled,
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
            ];

            Parsed2 = [
                TC1,
                TC2,
                TC3,
                TC4,
                TN,
                TR,
                TD,
                TP
            ];

            // Drawing

            Parameters = new( "Parameters", [
                new UiNodeGraphView( this ),
                LoopStart,
                LoopEnd,
                SimpleAnimEnable,
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
                S11Enabled,
                ShUT,
                ShR,
                E24Enabled,
                IsApplyToneMap,
                IsApplyFog,
                ClipNearEnable,
                ClipFarEnable,
                new UiFloat2( "Clip Near", ClipNearStart, ClipNearEnd ),
                new UiFloat2( "Clip Far", ClipFarStart, ClipFarEnd ),
                ClipBasePointType,
                ApplyRateEnvironment,
                ApplyRateDirectional,
                ApplyRateLightBuffer,
                DOTy,
                DepthOffset
            ] );

            AnimationSplitDisplay = new( "Animation", [
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
            ] );

            UvView = new( UvSets );

            TextureDisplaySplit = new( "Textures", [
                TC1,
                TC2,
                TC3,
                TC4,
                TN,
                TR,
                TD,
                TP
            ] );
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            Peek( reader, Parsed, size );
            Peek( reader, Parsed2, size );

            UpdateData(); // TODO: check if moving this here breaks anything

            ReadNested( reader, ( BinaryReader _reader, string _name, int _size ) => {
                if( _name == "Data" ) {
                    Data?.Read( _reader, _size );
                }
                else if( _name == "UvSt" ) {
                    var uvSet = new AvfxParticleUvSet();
                    uvSet.Read( _reader, _size );
                    UvSets.Add( uvSet );
                }
            }, size );

            UvView.UpdateIdx();
        }

        public override void WriteContents( BinaryWriter writer ) {
            UvSetCount.Value = UvSets.Count;
            WriteNested( writer, Parsed );

            foreach( var uvSet in UvSets ) uvSet.Write( writer );

            Data?.Write( writer );
            WriteNested( writer, Parsed2 );
        }

        protected override IEnumerable<AvfxBase> GetChildren() {
            foreach( var item in Parsed ) yield return item;
            if( Data != null ) yield return Data;
            foreach( var item in Parsed2 ) yield return item;
        }

        public override void UpdateData() {
            Data = Type.Value switch {
                ParticleType.Parameter => null,
                ParticleType.Powder => new AvfxParticleDataPowder(),
                ParticleType.Windmill => new AvfxParticleDataWindmill(),
                ParticleType.Line => new AvfxParticleDataLine(),
                ParticleType.Model => new AvfxParticleDataModel( this ),
                ParticleType.Polyline => new AvfxParticleDataPolyline(),
                ParticleType.Quad => new AvfxParticleDataQuad(),
                ParticleType.Polygon => new AvfxParticleDataPolygon(),
                ParticleType.Decal => new AvfxParticleDataDecal(),
                ParticleType.DecalRing => new AvfxParticleDataDecalRing(),
                ParticleType.Disc => new AvfxParticleDataDisc(),
                ParticleType.LightModel => new AvfxParticleDataLightModel( this ),
                ParticleType.Laser => new AvfxParticleDataLaser(),
                ParticleType.ModelSkin => new AvfxParticleDataModelSkin(),
                ParticleType.Dissolve => new AvfxParticleDataDissolve(),
                _ => null,
            };

            Data?.SetAssigned( !Data.Optional );
        }

        public override void Draw() {
            using var _ = ImRaii.PushId( "Particle" );
            DrawRename();
            Type.Draw();
            Data?.DrawEnableCheckbox();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Parameters" ) ) {
                if( tab ) Parameters.Draw();
            }

            DrawData();

            using( var tab = ImRaii.TabItem( "Animation" ) ) {
                if( tab ) AnimationSplitDisplay.Draw();
            }

            using( var tab = ImRaii.TabItem( "UV Sets" ) ) {
                if( tab ) UvView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Textures" ) ) {
                if( tab ) TextureDisplaySplit.Draw();
            }
        }

        private void DrawData() {
            if( Data == null || ( Data.Optional && !Data.IsAssigned() ) ) return;

            using var tabItem = ImRaii.TabItem( "Data" );
            if( !tabItem ) return;

            Data.Draw();
        }

        public override string GetDefaultText() => $"Particle {GetIdx()} ({Type.Value})";

        public override string GetWorkspaceId() => $"Ptcl{GetIdx()}";
    }
}
