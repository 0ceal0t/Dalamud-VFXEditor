using AVFXLib.AVFX;
using AVFXLib.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXParticleSimple : Base
    {
        public const string NAME = "Smpl";

        public LiteralInt InjectionPositionType = new("SIPT");
        public LiteralInt InjectionDirectionType = new("SIDT");
        public LiteralInt BaseDirectionType = new("SBDT");
        public LiteralInt CreateCount = new("CCnt");
        public LiteralFloat CreateAreaX = new("CrAX");
        public LiteralFloat CreateAreaY = new("CrAY");
        public LiteralFloat CreateAreaZ = new("CrAZ");
        public LiteralFloat CoordAccuracyX = new("CAX");
        public LiteralFloat CoordAccuracyY = new("CAY");
        public LiteralFloat CoordAccuracyZ = new("CAZ");
        public LiteralFloat CoordGraX = new("CGX");
        public LiteralFloat CoordGraY = new("CGY");
        public LiteralFloat CoordGraZ = new("CGZ");
        public LiteralFloat ScaleXStart = new("SBX");
        public LiteralFloat ScaleYStart = new("SBY");
        public LiteralFloat ScaleXEnd = new("SEX");
        public LiteralFloat ScaleYEnd = new("SEY");
        public LiteralFloat ScaleCurve = new("SC");
        public LiteralFloat ScaleRandX0 = new("SRX0");
        public LiteralFloat ScaleRandX1 = new("SRX1");
        public LiteralFloat ScaleRandY0 = new("SRY0");
        public LiteralFloat ScaleRandY1 = new("SRY1");
        public LiteralFloat RotXStart = new("RIX");
        public LiteralFloat RotYStart = new("RIY");
        public LiteralFloat RotZStart = new("RIZ");
        public LiteralFloat RotXAdd = new("RAX");
        public LiteralFloat RotYAdd = new("RAY");
        public LiteralFloat RotZAdd = new("RAZ");
        public LiteralFloat RotXBase = new("RBX");
        public LiteralFloat RotYBase = new("RBY");
        public LiteralFloat RotZBase = new("RBZ");
        public LiteralFloat RotXVel = new("RVX");
        public LiteralFloat RotYVel = new("RVY");
        public LiteralFloat RotZVel = new("RVZ");
        public LiteralFloat VelMin = new("VMin");
        public LiteralFloat VelMax = new("VMax");
        public LiteralFloat VelFlatteryRate = new("FltR");
        public LiteralFloat VelFlatterySpeed = new("FltS");
        public LiteralInt UvCellU = new("UvCU");
        public LiteralInt UvCellV = new("UvCV");
        public LiteralInt UvInterval = new("UvIv");
        public LiteralInt UvNoRandom = new("UvNR");
        public LiteralInt UvNoLoopCount = new("UvLC");
        public LiteralInt InjectionModelIdx = new("IJMN");
        public LiteralInt InjectionVertexBindModelIdx = new("VBMN");
        public LiteralFloat InjectionRadialDir0 = new( "IRD0");
        public LiteralFloat InjectionRadialDir1 = new( "IRD1");
        public LiteralFloat PivotX = new("PvtX");
        public LiteralFloat PivotY = new("PvtY");
        public LiteralInt BlockNum = new("BlkN");
        public LiteralFloat LineLengthMin = new("LLin");
        public LiteralFloat LineLengthMax = new("LLax");
        public LiteralInt CreateIntervalVal = new("CrI");
        public LiteralInt CreateIntervalRandom = new("CrIR");
        public LiteralInt CreateIntervalCount = new("CrIC");
        public LiteralInt CreateIntervalLife = new("CrIL");
        public LiteralInt CreateNewAfterDelete = new("bCrN", size: 1);
        public LiteralInt UvReverse = new("bRUV", size: 1);
        public LiteralInt ScaleRandomLink = new("bSRL", size: 1);
        public LiteralInt BindParent = new("bBnP", size: 1);
        public LiteralInt ScaleByParent = new("bSnP", size: 1);
        public LiteralInt PolyLineTag = new("PolT");
        readonly List<Base> Attributes;

        public ColorStruct Colors;
        public ColorFrames Frames;

        public AVFXParticleSimple() : base(NAME)
        {
            Attributes = new List<Base>(new Base[]{
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
                PolyLineTag
            });
        }

        public override void Read(AVFXNode node)
        {
            Assigned = true;
            ReadAVFX(Attributes, node);
            foreach (var item in node.Children){switch (item.Name){
                case "Cols":
                        Colors = new ColorStruct(((AVFXLeaf)item).Contents);
                    break;
                case "Frms":
                    Frames = new ColorFrames(((AVFXLeaf)item).Contents);
                    break;
                }
            }
        }

        public override void ToDefault()
        {
            Assigned = true;
            SetDefault(Attributes);
            Colors = new ColorStruct(new byte[16]);
            Frames = new ColorFrames(new byte[8]);
        }

        public override AVFXNode ToAVFX()
        {
            var smplAvfx = new AVFXNode("Smpl");

            PutAVFX(smplAvfx, Attributes);

            smplAvfx.Children.Add(new AVFXLeaf("Cols", 16, Colors.GetBytes()));
            smplAvfx.Children.Add(new AVFXLeaf("Frms", 8, Frames.GetBytes()));

            return smplAvfx;
        }
    }

    // ColorStruct is 16 bytes -> 4x4, each byte is color channel
    // [ FF, FF, FF, ....]
    public class ColorStruct
    {
        public byte[] colors;

        public ColorStruct(byte[] rawBytes)
        {
            colors = rawBytes;
        }

        public byte[] GetBytes()
        {
            return colors;
        }
    }

    // ColorFrames is 8 bytes -> 4x2, each frame is 2 bytes
    // [ FF FF, FF FF, ... ]
    public class ColorFrames
    {
        public int[] frames;

        public ColorFrames(byte[] rawBytes)
        {
            frames = new int[4];
            for(var idx = 0; idx < 4; idx++)
            {
                frames[idx] = Util.Bytes2ToInt(new byte[] { rawBytes[2 * idx], rawBytes[2 * idx + 1] });
            }
        }

        public byte[] GetBytes()
        {
            var bytes = new byte[8];
            var idx = 0;
            foreach (var f in frames)
            {
                var fBytes = Util.IntTo2Bytes(f);
                Buffer.BlockCopy(fBytes, 0, bytes, 2 * idx, 2);
                idx++;
            }
            return bytes;
        }
    }
}
