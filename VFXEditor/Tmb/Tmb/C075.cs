using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VFXEditor.Tmb.Tmb {
    public class C075 : TmbItem {
        private short Time = 0;
        private int Unk_2 = 30;
        private int Unk_3 = 0;
        private int Unk_4 = 0;
        private readonly List<List<float>> Unk_Pairs;

        public static readonly string Name = "C075";

        private static List<List<float>> GetDefault() {
            var ret = new List<List<float>>();
            ret.Add( new List<float>( new[] { 1f, 1f, 1f } ) );
            ret.Add( new List<float>( new[] { 0f, 0f, 0f } ) );
            ret.Add( new List<float>( new[] { 0f, 0f, 0f } ) );
            ret.Add( new List<float>( new[] { 1f, 1f, 1f, 1f } ) );
            ret.Add( new List<float>() );
            return ret;
        }

        public C075() {
            Unk_Pairs = GetDefault();
        }
        public C075( BinaryReader reader ) {
            var startPos = reader.BaseStream.Position; // [C075] + 8

            reader.ReadInt16(); // id
            Time = reader.ReadInt16();
            Unk_2 = reader.ReadInt32();
            Unk_3 = reader.ReadInt32();
            Unk_4 = reader.ReadInt32();

            Unk_Pairs = ReadPairs( 5, reader, startPos );
        }

        public override int GetSize() => 0x40;
        public override int GetExtraSize() => 4 * Unk_Pairs.Select( x => x.Count ).Sum();

        public override void Write( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, Dictionary<string, int> stringPositions, int stringPos, int timelinePos ) {
            var startPos = ( int )entryWriter.BaseStream.Position + entryPos;

            TmbFile.WriteString( entryWriter, "C075" );
            entryWriter.Write( GetSize() );
            entryWriter.Write( Id );
            entryWriter.Write( Time );
            entryWriter.Write( Unk_2 );
            entryWriter.Write( Unk_3 );
            entryWriter.Write( Unk_4 );

            WritePairs( entryWriter, extraWriter, extraPos, startPos, Unk_Pairs );
        }

        public override string GetName() => Name;

        public override void Draw( string id ) {
            TmbFile.ShortInput( $"Time{id}", ref Time );
            ImGui.InputInt( $"Uknown 2{id}", ref Unk_2 );
            ImGui.InputInt( $"Uknown 3{id}", ref Unk_3 );
            ImGui.InputInt( $"Uknown 4{id}", ref Unk_4 );

            DrawPairs( id, Unk_Pairs );
        }

        public override void PopulateStringList( List<string> stringList ) {
        }
    }
}