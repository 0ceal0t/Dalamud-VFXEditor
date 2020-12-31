using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVFXLib.AVFX;
using AVFXLib.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace AVFXLib.Main
{
    public class Reader
    {
        public static AVFXNode readAVFX(byte[] bytes, out List<string> messages)
        {
            BinaryReader nestedReader = new BinaryReader(new MemoryStream(bytes));
            return readAVFX(nestedReader, out messages);
        }

        public static AVFXNode readAVFX(BinaryReader reader, out List<string> messages)
        {
            AVFXNode.ResetLog();
            var result = readDef(reader)[0];
            messages = new List<string>(AVFXNode.LogMessages);
            return result;
        }

        public static AVFXNode readAVFX(string AVFX_PATH, out List<string> messages)
        {
            AVFXNode.ResetLog();
            if (File.Exists(AVFX_PATH))
            {
                using (BinaryReader reader = new BinaryReader(File.Open(AVFX_PATH, FileMode.Open)))
                {
                    var result = readDef(reader)[0];
                    messages = new List<string>(AVFXNode.LogMessages);
                    return result;
                }
            }
            else
            {
                AVFXNode.LogMessages.Add("File does not exist");
            }
            messages = new List<string>(AVFXNode.LogMessages);
            return null;
        }

        static readonly HashSet<string> NESTED = new HashSet<string>(new string[]{
            "RGB",
            "SclR",
            "SclG",
            "SclB",
            "SclA",
            "RanR",
            "RanG",
            "RanB",
            "RanA",
            "Bri",
            "RBri",
            "Col",
            "X",
            "Y",
            "Z",
            "RX",
            "RY",
            "RZ",
            "Life",
            "Rad",
            "Rads",
            "XA",
            "YA",
            "ZA",
            "SjI",
            "Scr",
            "RotR",
            "NPow",
            "RPow",
            "DPow",
            "Doff",
            "Wd",
            "Len",
            "FrC",
            "FrRt",
            "Sft",
            "ColB",
            "ColE",
            "Wd",
            "WdR",
            "WdB",
            "WdC",
            "WdE",
            "Len",
            "ColB",
            "ColC",
            "ColE",
            "CoEB",
            "CoEC",
            "CoEE",
            "Cnt",
            "Ang",
            "WB",
            "WE",
            "RB",
            "RE",
            "CEI",
            "CEO",
            "TC1",
            "TC2",
            "TC3",
            "TC4",
            "TN",
            "TR",
            "TD",
            "TP",
            "PrpS",
            "PrpG",
            "DstS",
            "Len",
            "Str",
            "Smpl",
            "ARs",
            "ARsR",
            "Gra",
            "GraR",
            "IRad",
            "ORad",
            "Att",
            "RdO",
            "RdI",
            "Rot",
            "Pos",
            "Schd",
            "TmLn",
            "Emit",
            "Ptcl",
            "Efct",
            "Bind",
            "Modl",
            "AVFX",
            "Item",
            "A",
            "Data",
            "UvSt",
            "Scl",
            "ItPr",
            "XR",
            "YR",
            "ZR",
            "CrI",
            "CrIR",
            "CrC",
            "Trgr",
            "SpS",
            "IjS",
            "IjSR",
            "ItEm",
            "Moph",
            "COF",
            "COFR",
            "WID"
        });

        static readonly HashSet<string> ALLOW = new HashSet<string>(new string[]{
            "Clip",
            "Keys",
            "Tex",
            "VDrw",
            "VIdx",
            "VNum",
            "VEmt",
            "Cols",
            "SdNm"
        });

        public static List<AVFXNode> readDef(BinaryReader reader)
        {
            List<AVFXNode> r = new List<AVFXNode>();
            if (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                // GET THE NAME
                byte[] name = BitConverter.GetBytes(reader.ReadInt32()).Reverse().ToArray();
                List<byte> nonZero = new List<byte>();
                foreach (byte n in name) // parse out 00 bytes
                {
                    if (n != (byte)0)
                    {
                        nonZero.Add(n);
                    }
                }
                ASCIIEncoding encoding = new ASCIIEncoding();
                string DefName = encoding.GetString(nonZero.ToArray());
                int Size = reader.ReadInt32();

                byte[] Contents = reader.ReadBytes(Size);
                if (NESTED.Contains(DefName) && Size > 8)
                {
                    BinaryReader nestedReader = new BinaryReader(new MemoryStream(Contents));
                    AVFXNode nestedNode = new AVFXNode(DefName);
                    nestedNode.Children = readDef(nestedReader);
                    r.Add(nestedNode);
                }
                else
                {
                    if (Size > 8 && !(ALLOW.Contains(DefName)))
                    {
                        AVFXNode.LogMessages.Add(string.Format("LARGE BLOCK: {0} {1}", DefName, Size));
                    }

                    AVFXLeaf leafNode = new AVFXLeaf(DefName, Size, Contents);
                    r.Add(leafNode);
                }
                // PAD
                int pad = Util.RoundUp(Size) - Size;
                reader.ReadBytes(pad);

                // KEEP READING
                return r.Concat(readDef(reader)).ToList();
            }
            return r;
        }
    }
}
