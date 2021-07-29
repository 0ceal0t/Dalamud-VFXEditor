using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVFXLib.AVFX;
using AVFXLib.Models;

namespace AVFXLib.Main
{
    public class Reader {
        public static AVFXNode ReadAVFX(BinaryReader reader, out List<string> messages) {
            messages = new List<string>();
            return ReadDefinition(reader, messages)[0];
        }

        static readonly HashSet<string> NESTED = new(new string[]{
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
            "Pow",
            "NPow",
            "RPow",
            "DPow",
            "DOff",
            "POff",
            "Wd",
            "Len",
            "LenR",
            "PnDs",
            "FrC",
            "FrRt",
            "Sft",
            "SftR",
            "Wd",
            "Wdt",
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
            "HBI",
            "HBO",
            "HEI",
            "HEO",
            "CEI",
            "CEO",
            "TC1",
            "TxN",
            "TC2",
            "TC3",
            "TC4",
            "TN",
            "TR",
            "TD",
            "TP",
            "PrpS",
            "Prp1",
            "Prp2",
            "PrpG",
            "DstS",
            "Dst",
            "DstR",
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
            "IjA",
            "OuS",
            "ItEm",
            "Moph",
            "COF",
            "COFR",
            "WID",
            "Amb",
            "Rate",
            "VRX",
            "VRY",
            "VRZ",
            "VRXR",
            "VRYR",
            "VRZR",
            "AnX",
            "AnY",
            "CF",
            "TexN",
            "TexNR"
        });

        static readonly HashSet<string> NOT_NESTED_LARGE = new(new string[]{ // not nested, larger than 8 bytes
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

        static readonly HashSet<string> NESTED_SMALL = new( new string[]{ // smaller than 8 bytes, still nested
            "Data",
            "Col",
            "ColB",
            "ColC",
            "ColE"
        } );

        public static List<AVFXNode> ReadDefinition(BinaryReader reader, List<string> messages)
        {
            var nodes = new List<AVFXNode>();
            if (reader.BaseStream.Position < reader.BaseStream.Length) {
                // GET THE NAME
                var name = BitConverter.GetBytes(reader.ReadInt32()).Reverse().ToArray();
                var nonZero = new List<byte>();
                foreach (var n in name) {
                    if (n != 0) {
                        nonZero.Add(n);
                    }
                }
                var encoding = new ASCIIEncoding();
                var DefName = encoding.GetString(nonZero.ToArray());
                var Size = reader.ReadInt32();

                var Contents = reader.ReadBytes(Size);
                if (NESTED.Contains(DefName) && (Size > 8 || NESTED_SMALL.Contains(DefName)))
                {
                    var nestedReader = new BinaryReader(new MemoryStream(Contents));
                    var nestedNode = new AVFXNode( DefName ) {
                        Children = ReadDefinition( nestedReader, messages )
                    };
                    nodes.Add(nestedNode);
                }
                else  {
                    if (Size > 8 && !NOT_NESTED_LARGE.Contains(DefName)) {
                        messages.Add(string.Format("LARGE BLOCK: {0} {1}", DefName, Size));
                    }

                    var leafNode = new AVFXLeaf(DefName, Size, Contents);
                    nodes.Add(leafNode);
                }
                // PAD
                var pad = Util.RoundUp(Size) - Size;
                reader.ReadBytes(pad);

                // KEEP READING
                return nodes.Concat( ReadDefinition( reader, messages)).ToList();
            }
            return nodes;
        }
    }
}
