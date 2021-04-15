using AVFXLib.AVFX;
using AVFXLib.Main;
using Newtonsoft.Json.Linq;
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

        public LiteralInt InjectionPositionType = new LiteralInt("SIPT");
        public LiteralInt InjectionDirectionType = new LiteralInt("SIDT");
        public LiteralInt BaseDirectionType = new LiteralInt("SBDT");
        public LiteralInt CreateCount = new LiteralInt("CCnt");
        public LiteralFloat CreateAreaX = new LiteralFloat("CrAX");
        public LiteralFloat CreateAreaY = new LiteralFloat("CrAY");
        public LiteralFloat CreateAreaZ = new LiteralFloat("CrAZ");
        public LiteralFloat CoordAccuracyX = new LiteralFloat("CAX");
        public LiteralFloat CoordAccuracyY = new LiteralFloat("CAY");
        public LiteralFloat CoordAccuracyZ = new LiteralFloat("CAZ");
        public LiteralFloat CoordGraX = new LiteralFloat("CGX");
        public LiteralFloat CoordGraY = new LiteralFloat("CGY");
        public LiteralFloat CoordGraZ = new LiteralFloat("CGZ");
        public LiteralFloat ScaleXStart = new LiteralFloat("SBX");
        public LiteralFloat ScaleYStart = new LiteralFloat("SBY");
        public LiteralFloat ScaleXEnd = new LiteralFloat("SEX");
        public LiteralFloat ScaleYEnd = new LiteralFloat("SEY");
        public LiteralFloat ScaleCurve = new LiteralFloat("SC");
        public LiteralFloat ScaleRandX0 = new LiteralFloat("SRX0");
        public LiteralFloat ScaleRandX1 = new LiteralFloat("SRX1");
        public LiteralFloat ScaleRandY0 = new LiteralFloat("SRY0");
        public LiteralFloat ScaleRandY1 = new LiteralFloat("SRY1");
        public LiteralFloat RotXStart = new LiteralFloat("RIX");
        public LiteralFloat RotYStart = new LiteralFloat("RIY");
        public LiteralFloat RotZStart = new LiteralFloat("RIZ");
        public LiteralFloat RotXAdd = new LiteralFloat("RAX");
        public LiteralFloat RotYAdd = new LiteralFloat("RAY");
        public LiteralFloat RotZAdd = new LiteralFloat("RAZ");
        public LiteralFloat RotXBase = new LiteralFloat("RBX");
        public LiteralFloat RotYBase = new LiteralFloat("RBY");
        public LiteralFloat RotZBase = new LiteralFloat("RBZ");
        public LiteralFloat RotXVel = new LiteralFloat("RVX");
        public LiteralFloat RotYVel = new LiteralFloat("RVY");
        public LiteralFloat RotZVel = new LiteralFloat("RVZ");
        public LiteralFloat VelMin = new LiteralFloat("VMin");
        public LiteralFloat VelMax = new LiteralFloat("VMax");
        public LiteralFloat VelFlatteryRate = new LiteralFloat("FltR");
        public LiteralFloat VelFlatterySpeed = new LiteralFloat("FltS");
        public LiteralInt UvCellU = new LiteralInt("UvCU");
        public LiteralInt UvCellV = new LiteralInt("UvCV");
        public LiteralInt UvInterval = new LiteralInt("UvIv");
        public LiteralInt UvNoRandom = new LiteralInt("UvNR");
        public LiteralInt UvNoLoopCount = new LiteralInt("UvLC");
        public LiteralInt InjectionModelIdx = new LiteralInt("IJMN");
        public LiteralInt InjectionVertexBindModelIdx = new LiteralInt("VBMN");
        public LiteralFloat InjectionRadialDir0 = new LiteralFloat( "IRD0");
        public LiteralFloat InjectionRadialDir1 = new LiteralFloat( "IRD1");
        public LiteralFloat PivotX = new LiteralFloat("PvtX");
        public LiteralFloat PivotY = new LiteralFloat("PvtY");
        public LiteralInt BlockNum = new LiteralInt("BlkN");
        public LiteralFloat LineLengthMin = new LiteralFloat("LLin");
        public LiteralFloat LineLengthMax = new LiteralFloat("LLax");
        public LiteralInt CreateIntervalVal = new LiteralInt("CrI");
        public LiteralInt CreateIntervalRandom = new LiteralInt("CrIR");
        public LiteralInt CreateIntervalCount = new LiteralInt("CrIC");
        public LiteralInt CreateIntervalLife = new LiteralInt("CrIL");
        public LiteralInt CreateNewAfterDelete = new LiteralInt("bCrN", size: 1);
        public LiteralInt UvReverse = new LiteralInt("bRUV", size: 1);
        public LiteralInt ScaleRandomLink = new LiteralInt("bSRL", size: 1);
        public LiteralInt BindParent = new LiteralInt("bBnP", size: 1);
        public LiteralInt ScaleByParent = new LiteralInt("bSnP", size: 1);
        public LiteralInt PolyLineTag = new LiteralInt("PolT");

        List<Base> Attributes;

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

        public override void read(AVFXNode node)
        {
            Assigned = true;
            ReadAVFX(Attributes, node);
            foreach (AVFXNode item in node.Children){switch (item.Name){
                case "Cols":
                        Colors = new ColorStruct(((AVFXLeaf)item).Contents);
                    break;
                case "Frms":
                    Frames = new ColorFrames(((AVFXLeaf)item).Contents);
                    break;
                }
            }
        }

        public override void toDefault()
        {
            Assigned = true;
            SetDefault(Attributes);
            Colors = new ColorStruct(new byte[16]);
            Frames = new ColorFrames(new byte[8]);
        }

        public override AVFXNode toAVFX()
        {
            AVFXNode smplAvfx = new AVFXNode("Smpl");

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

        public JArray GetJSON()
        {
            JArray ret = new JArray();
            foreach (byte c in colors)
            {
                ret.Add(new JValue(Util.Bytes1ToInt(new byte[] { c })));
            }
            return ret;
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
            for(int idx = 0; idx < 4; idx++)
            {
                frames[idx] = Util.Bytes2ToInt(new byte[] { rawBytes[2 * idx], rawBytes[2 * idx + 1] });
            }
        }

        public JArray GetJSON()
        {
            JArray ret = new JArray();
            foreach (int f in frames)
            {
                ret.Add(new JValue(f));
            }
            return ret;
        }

        public byte[] GetBytes()
        {
            byte[] bytes = new byte[8];
            int idx = 0;
            foreach (int f in frames)
            {
                byte[] fBytes = Util.IntTo2Bytes(f);
                Buffer.BlockCopy(fBytes, 0, bytes, 2 * idx, 2);
                idx++;
            }
            return bytes;
        }
    }
}
