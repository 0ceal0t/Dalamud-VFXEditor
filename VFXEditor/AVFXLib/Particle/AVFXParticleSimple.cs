using System.Collections.Generic;
using System.IO;

namespace VfxEditor.AVFXLib.Particle {
    public class AVFXParticleSimple : AVFXBase {
        public readonly AVFXInt InjectionPositionType = new( "SIPT" );
        public readonly AVFXInt InjectionDirectionType = new( "SIDT" );
        public readonly AVFXInt BaseDirectionType = new( "SBDT" );
        public readonly AVFXInt CreateCount = new( "CCnt" );
        public readonly AVFXFloat CreateAreaX = new( "CrAX" );
        public readonly AVFXFloat CreateAreaY = new( "CrAY" );
        public readonly AVFXFloat CreateAreaZ = new( "CrAZ" );
        public readonly AVFXFloat CoordAccuracyX = new( "CAX" );
        public readonly AVFXFloat CoordAccuracyY = new( "CAY" );
        public readonly AVFXFloat CoordAccuracyZ = new( "CAZ" );
        public readonly AVFXFloat CoordGraX = new( "CGX" );
        public readonly AVFXFloat CoordGraY = new( "CGY" );
        public readonly AVFXFloat CoordGraZ = new( "CGZ" );
        public readonly AVFXFloat ScaleXStart = new( "SBX" );
        public readonly AVFXFloat ScaleYStart = new( "SBY" );
        public readonly AVFXFloat ScaleXEnd = new( "SEX" );
        public readonly AVFXFloat ScaleYEnd = new( "SEY" );
        public readonly AVFXFloat ScaleCurve = new( "SC" );
        public readonly AVFXFloat ScaleRandX0 = new( "SRX0" );
        public readonly AVFXFloat ScaleRandX1 = new( "SRX1" );
        public readonly AVFXFloat ScaleRandY0 = new( "SRY0" );
        public readonly AVFXFloat ScaleRandY1 = new( "SRY1" );
        public readonly AVFXFloat RotXStart = new( "RIX" );
        public readonly AVFXFloat RotYStart = new( "RIY" );
        public readonly AVFXFloat RotZStart = new( "RIZ" );
        public readonly AVFXFloat RotXAdd = new( "RAX" );
        public readonly AVFXFloat RotYAdd = new( "RAY" );
        public readonly AVFXFloat RotZAdd = new( "RAZ" );
        public readonly AVFXFloat RotXBase = new( "RBX" );
        public readonly AVFXFloat RotYBase = new( "RBY" );
        public readonly AVFXFloat RotZBase = new( "RBZ" );
        public readonly AVFXFloat RotXVel = new( "RVX" );
        public readonly AVFXFloat RotYVel = new( "RVY" );
        public readonly AVFXFloat RotZVel = new( "RVZ" );
        public readonly AVFXFloat VelMin = new( "VMin" );
        public readonly AVFXFloat VelMax = new( "VMax" );
        public readonly AVFXFloat VelFlatteryRate = new( "FltR" );
        public readonly AVFXFloat VelFlatterySpeed = new( "FltS" );
        public readonly AVFXInt UvCellU = new( "UvCU" );
        public readonly AVFXInt UvCellV = new( "UvCV" );
        public readonly AVFXInt UvInterval = new( "UvIv" );
        public readonly AVFXInt UvNoRandom = new( "UvNR" );
        public readonly AVFXInt UvNoLoopCount = new( "UvLC" );
        public readonly AVFXInt InjectionModelIdx = new( "IJMN" );
        public readonly AVFXInt InjectionVertexBindModelIdx = new( "VBMN" );
        public readonly AVFXFloat InjectionRadialDir0 = new( "IRD0" );
        public readonly AVFXFloat InjectionRadialDir1 = new( "IRD1" );
        public readonly AVFXFloat PivotX = new( "PvtX" );
        public readonly AVFXFloat PivotY = new( "PvtY" );
        public readonly AVFXInt BlockNum = new( "BlkN" );
        public readonly AVFXFloat LineLengthMin = new( "LLin" );
        public readonly AVFXFloat LineLengthMax = new( "LLax" );
        public readonly AVFXInt CreateIntervalVal = new( "CrI" );
        public readonly AVFXInt CreateIntervalRandom = new( "CrIR" );
        public readonly AVFXInt CreateIntervalCount = new( "CrIC" );
        public readonly AVFXInt CreateIntervalLife = new( "CrIL" );
        public readonly AVFXInt CreateNewAfterDelete = new( "bCrN", size: 1 );
        public readonly AVFXInt UvReverse = new( "bRUV", size: 1 );
        public readonly AVFXInt ScaleRandomLink = new( "bSRL", size: 1 );
        public readonly AVFXInt BindParent = new( "bBnP", size: 1 );
        public readonly AVFXInt ScaleByParent = new( "bSnP", size: 1 );
        public readonly AVFXInt PolyLineTag = new( "PolT" );

        public readonly SimpleColorStruct Colors = new();
        public readonly SimpleColorFrames Frames = new();

        private readonly List<AVFXBase> Children;

        public AVFXParticleSimple() : base( "Smpl" ) {
            Children = new List<AVFXBase> {
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
        }

        public override void ReadContents( BinaryReader reader, int size ) => ReadNested( reader, Children, size );

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Children, assigned );

        protected override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Children );
    }

    public class SimpleColorStruct : AVFXBase {
        public byte[] Colors = new byte[16];

        public SimpleColorStruct() : base( "Cols" ) {
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            for( var i = 0; i < 16; i++ ) {
                Colors[i] = reader.ReadByte();
            }
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            for( var i = 0; i < 16; i++ ) {
                writer.Write( Colors[i] );
            }
        }
    }

    public class SimpleColorFrames : AVFXBase {
        public int[] Frames = new int[4];

        public SimpleColorFrames() : base( "Frms" ) {

        }

        public override void ReadContents( BinaryReader reader, int size ) {
            for( var i = 0; i < 4; i++ ) {
                Frames[i] = reader.ReadInt16();
            }
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            for( var i = 0; i < 4; i++ ) {
                writer.Write( ( short )Frames[i] );
            }
        }
    }
}
