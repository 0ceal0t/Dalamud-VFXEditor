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

        public LiteralInt InjectionPositionType = new LiteralInt("injectionPositionType", "SIPT");
        public LiteralInt InjectionDirectionType = new LiteralInt("injectionDirectionType", "SIDT");
        public LiteralInt BaseDirectionType = new LiteralInt("baseDirectionType", "SBDT");
        public LiteralInt CreateCount = new LiteralInt("createCount", "CCnt");
        public LiteralFloat CreateAreaX = new LiteralFloat("createAreaX", "CrAX");
        public LiteralFloat CreateAreaY = new LiteralFloat("createAreaY", "CrAY");
        public LiteralFloat CreateAreaZ = new LiteralFloat("createAreaZ", "CrAZ");
        public LiteralFloat CoordAccuracyX = new LiteralFloat("coordAccuracyX", "CAX");
        public LiteralFloat CoordAccuracyY = new LiteralFloat("coordAccuracyY", "CAY");
        public LiteralFloat CoordAccuracyZ = new LiteralFloat("coordAccuracyZ", "CAZ");
        public LiteralFloat CoordGraX = new LiteralFloat("coordGraX", "CGX");
        public LiteralFloat CoordGraY = new LiteralFloat("coordGraY", "CGY");
        public LiteralFloat CoordGraZ = new LiteralFloat("coordGraZ", "CGZ");
        public LiteralFloat ScaleXStart = new LiteralFloat("scaleXStart", "SBX");
        public LiteralFloat ScaleYStart = new LiteralFloat("scaleYStart", "SBY");
        public LiteralFloat ScaleXEnd = new LiteralFloat("scaleXEnd", "SEX");
        public LiteralFloat ScaleYEnd = new LiteralFloat("scaleYEnd", "SEY");
        public LiteralFloat ScaleCurve = new LiteralFloat("scaleCurve", "SC");
        public LiteralFloat ScaleRandX0 = new LiteralFloat("scaleRandX0", "SRX0");
        public LiteralFloat ScaleRandX1 = new LiteralFloat("scaleRandX1", "SRX1");
        public LiteralFloat ScaleRandY0 = new LiteralFloat("scaleRandY0", "SRY0");
        public LiteralFloat ScaleRandY1 = new LiteralFloat("scaleRandY1", "SRY1");
        public LiteralFloat RotXStart = new LiteralFloat("rotXStart", "RIX");
        public LiteralFloat RotYStart = new LiteralFloat("rotYStart", "RIY");
        public LiteralFloat RotZStart = new LiteralFloat("rotZStart", "RIZ");
        public LiteralFloat RotXAdd = new LiteralFloat("rotXAdd", "RAX");
        public LiteralFloat RotYAdd = new LiteralFloat("rotYAdd", "RAY");
        public LiteralFloat RotZAdd = new LiteralFloat("rotZAdd", "RAZ");
        public LiteralFloat RotXBase = new LiteralFloat("rotXBase", "RBX");
        public LiteralFloat RotYBase = new LiteralFloat("rotYBase", "RBY");
        public LiteralFloat RotZBase = new LiteralFloat("rotZBase", "RBZ");
        public LiteralFloat RotXVel = new LiteralFloat("rotXVel", "RVX");
        public LiteralFloat RotYVel = new LiteralFloat("rotYVel", "RVY");
        public LiteralFloat RotZVel = new LiteralFloat("rotZVel", "RVZ");
        public LiteralFloat VelMin = new LiteralFloat("velMin", "VMin");
        public LiteralFloat VelMax = new LiteralFloat("velMax", "VMax");
        public LiteralFloat VelFlatteryRate = new LiteralFloat("velFlatteryRate", "FltR");
        public LiteralFloat VelFlatterySpeed = new LiteralFloat("velFlatterySpeed", "FltS");
        public LiteralInt UvCellU = new LiteralInt("uvCellU", "UvCU");
        public LiteralInt UvCellV = new LiteralInt("uvCellV", "UvCV");
        public LiteralInt UvInterval = new LiteralInt("uvInterval", "UvIv");
        public LiteralInt UvNoRandom = new LiteralInt("uvNoRandom", "UvNR");
        public LiteralInt UvNoLoopCount = new LiteralInt("uvNoLoopCount", "UvLC");
        public LiteralInt InjectionModelIdx = new LiteralInt("injectionModelIdx", "IJMN");
        public LiteralInt InjectionVertexBindModelIdx = new LiteralInt("injectionVertexBindModelIdx", "VBMN");
        public LiteralInt InjectionRadialDir0 = new LiteralInt("injectionRadialDir0", "IRD0");
        public LiteralInt InjectionRadialDir1 = new LiteralInt("injectionRadialDir1", "IRD1");
        public LiteralFloat PivotX = new LiteralFloat("pivotX", "PvtX");
        public LiteralFloat PivotY = new LiteralFloat("pivotY", "PvtY");
        public LiteralInt BlockNum = new LiteralInt("blockNum", "BlkN");
        public LiteralFloat LineLengthMin = new LiteralFloat("lineLengthMin", "LLin");
        public LiteralFloat LineLengthMax = new LiteralFloat("lineLengthMax", "LLax");
        public LiteralInt CreateIntervalVal = new LiteralInt("createIntervalVal", "CrI");
        public LiteralInt CreateIntervalRandom = new LiteralInt("createIntervalRandom", "CrIR");
        public LiteralInt CreateIntervalCount = new LiteralInt("createIntervalCount", "CrIC");
        public LiteralInt CreateIntervalLife = new LiteralInt("createIntervalLife", "CrIL");
        public LiteralInt CreateNewAfterDelete = new LiteralInt("createNewAfterDelete", "bCrN", size: 1);
        public LiteralInt UvReverse = new LiteralInt("uvReverse", "bRUV", size: 1);
        public LiteralInt ScaleRandomLink = new LiteralInt("scaleRandomLink", "bSRL", size: 1);
        public LiteralInt BindParent = new LiteralInt("bindParent", "bBnP", size: 1);
        public LiteralInt ScaleByParent = new LiteralInt("scaleByParent", "bSnP", size: 1);
        public LiteralInt PolyLineTag = new LiteralInt("polyLineTag", "PolT");

        List<Base> Attributes;

        public ColorStruct Colors;
        public ColorFrames Frames;

        public AVFXParticleSimple(string jsonPath) : base(jsonPath, NAME)
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

        public override JToken toJSON()
        {
            JObject elem = new JObject();
            PutJSON(elem, Attributes);
            elem["colors"] = Colors.GetJSON();
            elem["colorFrames"] = Frames.GetJSON();
            return elem;
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
