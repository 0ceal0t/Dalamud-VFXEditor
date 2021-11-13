using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.Tmb.Item {
    public abstract class TmbItem {
        public short Id { get; protected set; }

        public abstract int GetSize();
        public abstract int GetExtraSize();
        public abstract string GetName();

        public abstract void Draw( string id );

        public void CalculateId( ref short id ) {
            Id = id++;
        }

        public abstract void PopulateStringList( List<string> stringList );

        public abstract void Write( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, Dictionary<string, int> stringPositions, int stringPos, int timelinePos );

        public static void AddString( string newItem, List<string> stringList ) {
            if( stringList.Contains( newItem ) ) return;
            stringList.Add( newItem );
        }

        public static List<List<float>> ReadPairs(int number, BinaryReader reader, long startPos) {
            var ret = new List<List<float>>();
            for( var i = 0; i < number; i++ ) {
                var subRet = new List<float>();

                var offset2 = reader.ReadInt32(); // offset [C012] + offset + 8
                var count = reader.ReadInt32(); // count

                var savePos2 = reader.BaseStream.Position;
                reader.BaseStream.Seek( startPos + offset2, SeekOrigin.Begin );
                for( var j = 0; j < count; j++ ) {
                    var item = reader.ReadSingle();
                    subRet.Add( item );
                }
                reader.BaseStream.Seek( savePos2, SeekOrigin.Begin );
                ret.Add( subRet );
            }
            return ret;
        }

        public static void WritePairs( BinaryWriter entryWriter, BinaryWriter extraWriter, int extraPos, int startPos, List<List<float>> pairs) {
            foreach( var entry in pairs ) {
                var extraEndPos = ( int )extraWriter.BaseStream.Position + extraPos;
                var extraOffset = extraEndPos - startPos - 8;

                var count = entry.Count;
                entryWriter.Write( count == 0 ? 0 : extraOffset );
                entryWriter.Write( count );

                foreach( var item in entry ) {
                    extraWriter.Write( item );
                }
            }
        }

        public static void DrawPairs(string id, List<List<float>> Pairs) {
            for( var i = 0; i < Pairs.Count; i++ ) {
                var entries = Pairs[i];
                for( var j = 0; j < entries.Count; j++ ) {
                    var item = entries[j];
                    if( ImGui.InputFloat( $"Unknown Pairs ({i},{j}){id}", ref item ) ) {
                        Pairs[i][j] = item;
                    }
                }
            }
        }
    }
}
