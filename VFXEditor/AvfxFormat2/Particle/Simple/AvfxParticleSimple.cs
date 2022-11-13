using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Utils;
using static VfxEditor.AvfxFormat2.Enums;

namespace VfxEditor.AvfxFormat2 {
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
        public readonly AvfxInt InjectionModelIdx = new( "Injection Model Index", "IJMN" );
        public readonly AvfxInt InjectionVertexBindModelIdx = new( "Injection Model Bind Index", "VBMN" );
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
        public readonly AvfxInt CreateNewAfterDelete = new( "Create New After Death", "bCrN", size: 1 );
        public readonly AvfxInt UvReverse = new( "UV Reverse", "bRUV", size: 1 );
        public readonly AvfxInt ScaleRandomLink = new( "Scale Random Link", "bSRL", size: 1 );
        public readonly AvfxInt BindParent = new( "Bind Parent", "bBnP", size: 1 );
        public readonly AvfxInt ScaleByParent = new( "Scale by Parent", "bSnP", size: 1 );
        public readonly AvfxInt PolyLineTag = new( "Polyline Tag", "PolT" );

        public readonly AvfxSimpleColors Colors = new();
        public readonly AvfxSimpleFrames Frames = new();

        private readonly List<AvfxBase> Children;

        private readonly UiParameters Animation;
        private readonly UiParameters Texture;
        private readonly UiParameters Color;

        public AvfxParticleSimple( AvfxParticle particle ) : base( "TR", particle ) {
            InitNodeSelects();

            Children = new() {
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
                CreateNewAfterDelete,
                UvReverse,
                ScaleRandomLink,
                BindParent,
                ScaleByParent,
                PolyLineTag,
                Colors,
                Frames
            };

            Parameters.Add( InjectionPositionType );
            Parameters.Add( InjectionDirectionType );
            Parameters.Add( BaseDirectionType );
            Parameters.Add( CreateCount );
            Parameters.Add( new UiFloat3( "Create Area", CreateAreaX, CreateAreaY, CreateAreaZ ) );
            Parameters.Add( new UiFloat3( "Coord Accuracy", CoordAccuracyX, CoordAccuracyY, CoordAccuracyZ ) );
            Parameters.Add( new UiFloat3( "Coord Gra", CoordGraX, CoordGraY, CoordGraZ ) );
            Parameters.Add( InjectionRadialDir0 );
            Parameters.Add( InjectionRadialDir1 );
            Parameters.Add( BlockNum );
            Parameters.Add( LineLengthMin );
            Parameters.Add( LineLengthMax );
            Parameters.Add( CreateIntervalVal );
            Parameters.Add( CreateIntervalRandom );
            Parameters.Add( CreateIntervalCount );
            Parameters.Add( CreateIntervalLife );
            Parameters.Add( CreateNewAfterDelete );

            Tabs.Add( Animation = new UiParameters( "Animation" ) );
            Animation.Add( new UiFloat2( "Scale Start", ScaleXStart, ScaleYStart ) );
            Animation.Add( new UiFloat2( "Scale End", ScaleXEnd, ScaleYEnd ) );
            Animation.Add( ScaleCurve );
            Animation.Add( new UiFloat2( "Scale X Random", ScaleRandX0, ScaleRandX1 ) );
            Animation.Add( new UiFloat2( "Scale Y Random", ScaleRandY0, ScaleRandY1 ) );
            Animation.Add( new UiFloat3( "Rotation Add", RotXAdd, RotYAdd, RotZAdd ) );
            Animation.Add( new UiFloat3( "Rotation Base", RotXBase, RotYBase, RotZBase ) );
            Animation.Add( new UiFloat3( "Rotation Velocity", RotXVel, RotYVel, RotZVel ) );
            Animation.Add( VelMin );
            Animation.Add( VelMax );
            Animation.Add( VelFlatteryRate );
            Animation.Add( VelFlatterySpeed );
            Animation.Add( ScaleRandomLink );
            Animation.Add( BindParent );
            Animation.Add( ScaleByParent );
            Animation.Add( PolyLineTag );

            Tabs.Add( Texture = new UiParameters( "Texture" ) );
            Texture.Add( UvCellU );
            Texture.Add( UvCellV );
            Texture.Add( UvInterval );
            Texture.Add( UvNoRandom );
            Texture.Add( UvNoLoopCount );
            Texture.Add( UvReverse );

            Tabs.Add( Color = new UiParameters( "Color" ) );
            Color.Add( new UiSimpleColor( this, 0 ) );
            Color.Add( new UiSimpleColor( this, 1 ) );
            Color.Add( new UiSimpleColor( this, 2 ) );
            Color.Add( new UiSimpleColor( this, 3 ) );
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            ReadNested( reader, Children, size );
            EnableAllSelectors();
        }

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Children, assigned );

        protected override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Children );

        public override void DrawUnassigned( string parentId ) {
            if( ImGui.SmallButton( "+ Simple Animation" + parentId ) ) Assign();
        }

        public override void DrawAssigned( string parentId ) {
            var id = parentId + "/Simple";
            if( UiUtils.RemoveButton( "Delete" + id, small: true ) ) {
                Unassign();
                return;
            }
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            IUiItem.DrawListTabs( Tabs, id );
        }

        public override string GetDefaultText() => "Simple Animation";

        public override List<UiNodeSelect> GetNodeSelects() => new() {
            new UiNodeSelect<AvfxModel>( Particle, "Injection Model", Particle.NodeGroups.Models, InjectionModelIdx ),
            new UiNodeSelect<AvfxModel>( Particle, "Injection Vertex Bind Model", Particle.NodeGroups.Models, InjectionVertexBindModelIdx )
        };
    }
}
