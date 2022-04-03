using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VFXEditor.Helper;

namespace VFXEditor.Tmb.Tmb {
    public abstract class TmbItem {
        public short Id { get; protected set; }
        protected short Time = 0;

        public void CalculateId( ref short id ) {
            Id = id++;
        }

        protected void ReadInfo(BinaryReader reader) {
            reader.ReadInt16(); // id
            Time = reader.ReadInt16();
        }

        public abstract int GetSize();
        public abstract int GetExtraSize();
        public abstract string GetName();
        public abstract string GetDisplayName();

        public abstract void Draw( string id );

        protected void DrawInfo(string id) {
            FileHelper.ShortInput( $"Time{id}", ref Time );
        }

        public abstract void PopulateStringList( List<string> stringList );

        public abstract void Write( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, Dictionary<string, int> stringPositions, int stringPos );

        protected void WriteInfo( BinaryWriter entryWriter ) {
            FileHelper.WriteString( entryWriter, GetName() );
            entryWriter.Write( GetSize() );
            entryWriter.Write( Id );
            entryWriter.Write( Time );
        }

        // ===========================

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

        public static Vector3 ListToVec3( List<float> list ) => list.Count == 3 ? new Vector3( list[0], list[1], list[2] ) : new Vector3( 0 );
        public static Vector4 ListToVec4( List<float> list ) => list.Count == 4 ? new Vector4( list[0], list[1], list[2], list[3] ) : new Vector4( 0 );
        public static List<float> Vec3ToList( Vector3 vec ) => new ( new[] { vec.X, vec.Y, vec.Z } );
        public static List<float> Vec4ToList( Vector4 vec ) => new ( new[] { vec.X, vec.Y, vec.Z, vec.W } );
    }
}
