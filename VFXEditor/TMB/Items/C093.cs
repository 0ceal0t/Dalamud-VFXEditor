using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VFXEditor.TMB.TMB {
    public class C093 : TMBItem {
        private int Unk_2 = 30;
        private int Unk_3 = 0;
        private readonly List<List<float>> Unk_Pairs;
        private int Unk_4 = 0;

        public static readonly string DisplayName = "C093";
        public override string GetDisplayName() => DisplayName;
        public override string GetName() => "C093";

        private static List<List<float>> GetDefault() {
            var ret = new List<List<float>> {
                new List<float>( new[] { 1f, 1f, 1f, 1f } ),
                new List<float>( new[] { 1f, 1f, 1f, 1f } )
            };
            return ret;
        }

        public C093() {
            Unk_Pairs = GetDefault();
        }
        public C093( BinaryReader reader ) {
            var startPos = reader.BaseStream.Position; // [C075] + 8

            ReadInfo( reader );
            Unk_2 = reader.ReadInt32();
            Unk_3 = reader.ReadInt32();

            Unk_Pairs = ReadPairs( 2, reader, startPos );

            Unk_4 = reader.ReadInt32();
        }

        public override int GetSize() => 0x28;
        public override int GetExtraSize() => 4 * Unk_Pairs.Select( x => x.Count ).Sum();

        public override void Write( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, Dictionary<string, int> stringPositions, int stringPos ) {
            var startPos = ( int )entryWriter.BaseStream.Position + entryPos;

            WriteInfo( entryWriter );
            entryWriter.Write( Unk_2 );
            entryWriter.Write( Unk_3 );

            WritePairs( entryWriter, extraWriter, extraPos, startPos, Unk_Pairs );

            entryWriter.Write( Unk_4 );
        }

        public override void Draw( string id ) {
            DrawInfo( id );
            ImGui.InputInt( $"Unknown 2{id}", ref Unk_2 );
            ImGui.InputInt( $"Unknown 3{id}", ref Unk_3 );
            ImGui.InputInt( $"Unknown 4{id}", ref Unk_4 );

            DrawPairs( id, Unk_Pairs );
        }

        public override void PopulateStringList( List<string> stringList ) {
        }
    }
}