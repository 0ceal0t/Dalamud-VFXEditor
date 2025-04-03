using System.Collections.Generic;
using System.IO;

namespace VfxEditor.AvfxFormat {
    public class AvfxParticleSimple : AvfxParticleAttribute {
        public readonly AvfxInt InjectionPositionType = new( "Injection Position Type", "SIPT" );
        public readonly AvfxInt InjectionDirectionType = new( "Injection Direction Type", "SIDT" );
        public readonly AvfxInt BaseDirectionType = new( "Base Direction Type", "SBDT" );
        public readonly AvfxInt CreateCount = new( "Create Count", "CCnt" );
        public readonly AvfxFloat CreateAreaX = new( "Create Area X", "CrAX" );
        public readonly AvfxFloat CreateAreaY = new( "Create Area Y", "CrAY" );
        public readonly AvfxFloat CreateAreaZ = new( "Create Area Z", "CrAZ" );
        public readonly AvfxFloat CoordAccuracyX = new( "Coord Accuracy X", "CAX" );
        public readonly AvfxFloat CoordAccuracyY = new( "Coord Accuracy Y", "CAY" );
        public readonly AvfxFloat CoordAccuracyZ = new( "Coord Accuracy Z", "CAZ" );
        public readonly AvfxFloat CoordGraX = new( "Coord Gra X", "CGX" );
        public readonly AvfxFloat CoordGraY = new( "Coord Gra Y", "CGY" );
        public readonly AvfxFloat CoordGraZ = new( "Coord Gra Z", "CGZ" );
        public readonly AvfxFloat ScaleXStart = new( "Scale Start X", "SBX" );
        public readonly AvfxFloat ScaleYStart = new( "Scale Start Y", "SBY" );
        public readonly AvfxFloat ScaleXEnd = new( "Scale End X", "SEX" );
        public readonly AvfxFloat ScaleYEnd = new( "Scale End Y", "SEY" );
        public readonly AvfxFloat ScaleCurve = new( "Scale Curve", "SC" );
        public readonly AvfxFloat ScaleRandX0 = new( "Scale Random X 0", "SRX0" );
        public readonly AvfxFloat ScaleRandX1 = new( "Scale Random X 1", "SRX1" );
        public readonly AvfxFloat ScaleRandY0 = new( "Scale Random Y 0 ", "SRY0" );
        public readonly AvfxFloat ScaleRandY1 = new( "Scale Random Y 1", "SRY1" );
        public readonly AvfxFloat RotXStart = new( "Rotation Start X", "RIX" );
        public readonly AvfxFloat RotYStart = new( "Rotation Start Y", "RIY" );
        public readonly AvfxFloat RotZStart = new( "Rotation Start Z", "RIZ" );
        public readonly AvfxFloat RotXAdd = new( "Rotation Add X", "RAX" );
        public readonly AvfxFloat RotYAdd = new( "Rotation Add Y", "RAY" );
        public readonly AvfxFloat RotZAdd = new( "Rotation Add Z", "RAZ" );
        public readonly AvfxFloat RotXBase = new( "Rotation Base X", "RBX" );
        public readonly AvfxFloat RotYBase = new( "Rotation Base Y", "RBY" );
        public readonly AvfxFloat RotZBase = new( "Rotation Base Z", "RBZ" );
        public readonly AvfxFloat RotXVel = new( "Rotation Velocity X", "RVX" );
        public readonly AvfxFloat RotYVel = new( "Rotation Velocity Y", "RVY" );
        public readonly AvfxFloat RotZVel = new( "Rotation Velocity Z", "RVZ" );
        public readonly AvfxFloat VelMin = new( "Velocity Min", "VMin" );
        public readonly AvfxFloat VelMax = new( "Velocity Max", "VMax" );
        public readonly AvfxFloat VelFlatteryRate = new( "Velocity Flattery Rate", "FltR" );
        public readonly AvfxFloat VelFlatterySpeed = new( "Velocity Flattery Speed", "FltS" );
        public readonly AvfxInt UvCellU = new( "UV Cell U", "UvCU" );
        public readonly AvfxInt UvCellV = new( "UV Cell V", "UvCV" );
        public readonly AvfxInt UvInterval = new( "UV Interval", "UvIv" );
        public readonly AvfxInt UvNoRandom = new( "UV Random", "UvNR" );
        public readonly AvfxInt UvNoLoopCount = new( "UV Loop Count", "UvLC" );
        public readonly AvfxInt InjectionModelIdx = new( "Injection Model Index", "IJMN", value: -1 );
        public readonly AvfxInt InjectionVertexBindModelIdx = new( "Injection Model Bind Index", "VBMN", value: -1 );
        public readonly AvfxFloat InjectionRadialDir0 = new( "Injection Radial Direction 0", "IRD0" );
        public readonly AvfxFloat InjectionRadialDir1 = new( "Injection Radial Direction 1", "IRD1" );
        public readonly AvfxFloat PivotX = new( "Pivot X", "PvtX" );
        public readonly AvfxFloat PivotY = new( "Pivot Y", "PvtY" );
        public readonly AvfxInt BlockNum = new( "Block Number", "BlkN" );
        public readonly AvfxFloat LineLengthMin = new( "Line Length Min", "LLin" );
        public readonly AvfxFloat LineLengthMax = new( "Line Length Max", "LLax" );
        public readonly AvfxInt CreateIntervalVal = new( "Create Interval", "CrI" );
        public readonly AvfxInt CreateIntervalRandom = new( "Create Interval Random", "CrIR" );
        public readonly AvfxInt CreateIntervalCount = new( "Create Interval Count", "CrIC" );
        public readonly AvfxInt CreateIntervalLife = new( "Create Interval Life", "CrIL" );
        public readonly AvfxInt CrLR = new( "CrLR", "CrLR" ); // New to DT
        public readonly AvfxBool CreateNewAfterDelete = new( "Create New After Death", "bCrN", size: 1 );
        public readonly AvfxBool UvReverse = new( "UV Reverse", "bRUV", size: 1 );
        public readonly AvfxBool ScaleRandomLink = new( "Scale Random Link", "bSRL", size: 1 );
        public readonly AvfxBool BindParent = new( "Bind Parent", "bBnP", size: 1 );
        public readonly AvfxInt ScaleByParent = new( "Scale by Parent", "bSnP", size: 1 );
        public readonly AvfxInt PolyLineTag = new( "Polyline Tag", "PolT" );

        public readonly AvfxSimpleColors Colors = new();
        public readonly AvfxSimpleFrames Frames = new();

        private readonly List<AvfxBase> Parsed;

        private readonly UiDisplayList AnimationDisplay;
        private readonly UiDisplayList TextureDisplay;
        private readonly UiDisplayList ColorDisplay;

        public AvfxParticleSimple( AvfxParticle particle ) : base( "Smpl", particle ) {
            InitNodeSelects();

            Parsed = [
                InjectionPositionType,
                InjectionDirectionType,
                BaseDirectionType,
                CreateCount,
                CreateAreaX,
                CreateAreaY,
                CreateAreaZ,
                CoordAccuracyX,
                CoordAccuracyY,
                CoordAccuracyZ,
                CoordGraX,
                CoordGraY,
                CoordGraZ,
                ScaleXStart,
                ScaleYStart,
                ScaleXEnd,
                ScaleYEnd,
                ScaleCurve,
                ScaleRandX0,
                ScaleRandX1,
                ScaleRandY0,
                ScaleRandY1,
                RotXStart,
                RotYStart,
                RotZStart,
                RotXAdd,
                RotYAdd,
                RotZAdd,
                RotXBase,
                RotYBase,
                RotZBase,
                RotXVel,
                RotYVel,
                RotZVel,
                VelMin,
                VelMax,
                VelFlatteryRate,
                VelFlatterySpeed,
                UvCellU,
                UvCellV,
                UvInterval,
                UvNoRandom,
                UvNoLoopCount,
                InjectionModelIdx,
                InjectionVertexBindModelIdx,
                InjectionRadialDir0,
                InjectionRadialDir1,
                PivotX,
                PivotY,
                BlockNum,
                LineLengthMin,
                LineLengthMax,
                CreateIntervalVal,
                CreateIntervalRandom,
                CreateIntervalCount,
                CreateIntervalLife,
                CrLR,
                CreateNewAfterDelete,
                UvReverse,
                ScaleRandomLink,
                BindParent,
                ScaleByParent,
                PolyLineTag,
                Colors,
                Frames
            ];

            Display.Add( InjectionPositionType );
            Display.Add( InjectionDirectionType );
            Display.Add( BaseDirectionType );
            Display.Add( CreateCount );
            Display.Add( new UiFloat3( "Create Area", CreateAreaX, CreateAreaY, CreateAreaZ ) );
            Display.Add( new UiFloat3( "Coord Accuracy", CoordAccuracyX, CoordAccuracyY, CoordAccuracyZ ) );
            Display.Add( new UiFloat3( "Coord Gra", CoordGraX, CoordGraY, CoordGraZ ) );
            Display.Add( InjectionRadialDir0 );
            Display.Add( InjectionRadialDir1 );
            Display.Add( BlockNum );
            Display.Add( LineLengthMin );
            Display.Add( LineLengthMax );
            Display.Add( CreateIntervalVal );
            Display.Add( CreateIntervalRandom );
            Display.Add( CreateIntervalCount );
            Display.Add( CreateIntervalLife );
            Display.Add( CrLR );
            Display.Add( CreateNewAfterDelete );

            DisplayTabs.Add( AnimationDisplay = new UiDisplayList( "Animation" ) );
            AnimationDisplay.Add( new UiFloat2( "Scale Start", ScaleXStart, ScaleYStart ) );
            AnimationDisplay.Add( new UiFloat2( "Scale End", ScaleXEnd, ScaleYEnd ) );
            AnimationDisplay.Add( ScaleCurve );
            AnimationDisplay.Add( new UiFloat2( "Scale X Random", ScaleRandX0, ScaleRandX1 ) );
            AnimationDisplay.Add( new UiFloat2( "Scale Y Random", ScaleRandY0, ScaleRandY1 ) );
            AnimationDisplay.Add( new UiFloat3( "Rotation Add", RotXAdd, RotYAdd, RotZAdd ) );
            AnimationDisplay.Add( new UiFloat3( "Rotation Base", RotXBase, RotYBase, RotZBase ) );
            AnimationDisplay.Add( new UiFloat3( "Rotation Velocity", RotXVel, RotYVel, RotZVel ) );
            AnimationDisplay.Add( VelMin );
            AnimationDisplay.Add( VelMax );
            AnimationDisplay.Add( VelFlatteryRate );
            AnimationDisplay.Add( VelFlatterySpeed );
            AnimationDisplay.Add( ScaleRandomLink );
            AnimationDisplay.Add( BindParent );
            AnimationDisplay.Add( ScaleByParent );
            AnimationDisplay.Add( PolyLineTag );

            DisplayTabs.Add( TextureDisplay = new UiDisplayList( "Texture" ) );
            TextureDisplay.Add( UvCellU );
            TextureDisplay.Add( UvCellV );
            TextureDisplay.Add( UvInterval );
            TextureDisplay.Add( UvNoRandom );
            TextureDisplay.Add( UvNoLoopCount );
            TextureDisplay.Add( UvReverse );

            DisplayTabs.Add( ColorDisplay = new UiDisplayList( "Color" ) );
            ColorDisplay.Add( new UiSimpleColor( Frames.Frame1, Colors.Color1 ) );
            ColorDisplay.Add( new UiSimpleColor( Frames.Frame2, Colors.Color2 ) );
            ColorDisplay.Add( new UiSimpleColor( Frames.Frame3, Colors.Color3 ) );
            ColorDisplay.Add( new UiSimpleColor( Frames.Frame4, Colors.Color4 ) );
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            ReadNested( reader, Parsed, size );
            EnableAllSelectors();
        }

        public override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Parsed );

        protected override IEnumerable<AvfxBase> GetChildren() {
            foreach( var item in Parsed ) yield return item;
        }

        public override void DrawBody() {
            DrawNamedItems( DisplayTabs );
        }

        public override string GetDefaultText() => "Simple Animation";

        public override List<AvfxNodeSelect> GetNodeSelects() => [
            new AvfxNodeSelect<AvfxModel>( Particle, "Injection Model", Particle.NodeGroups.Models, InjectionModelIdx ),
            new AvfxNodeSelect<AvfxModel>( Particle, "Injection Vertex Bind Model", Particle.NodeGroups.Models, InjectionVertexBindModelIdx )
        ];
    }
}
