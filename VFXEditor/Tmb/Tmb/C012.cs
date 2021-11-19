using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VFXEditor.Helper;

namespace VFXEditor.Tmb.Tmb {
    public class C012 : TmbItem {
        private short Time = 0;
        private int Unk_2 = 30;
        private int Unk_3 = 0;
        private string Path = "";
        private short BindPoint_1 = 1;
        private short BindPoint_2 = 0xFF;
        private short BindPoint_3 = 2;
        private short bindPoint_4 = 0xFF;
        private readonly List<List<float>> Unk_Pairs;
        private int Unk_4;
        private int Unk_5;

        public static readonly string Name = "VFX (C012)";

        private static List<List<float>> GetDefault() {
            var ret = new List<List<float>> {
                new List<float>( new[] { 1f, 1f, 1f } ),
                new List<float>( new[] { 0f, 0f, 0f } ),
                new List<float>( new[] { 0f, 0f, 0f } ),
                new List<float>( new[] { 1f, 1f, 1f, 1f } ),
            };
            return ret;
        }

        public C012() {
            Unk_Pairs = GetDefault();
        }
        public C012( BinaryReader reader ) {
            var startPos = reader.BaseStream.Position; // [C012] + 8

            reader.ReadInt16(); // id
            Time = reader.ReadInt16();
            Unk_2 = reader.ReadInt32();
            Unk_3 = reader.ReadInt32();

            var offset = reader.ReadInt32(); // path offset: [C012] + offset + 8 = path
            var savePos = reader.BaseStream.Position;
            reader.BaseStream.Seek( startPos + offset, SeekOrigin.Begin );
            Path = FileHelper.ReadString( reader );
            reader.BaseStream.Seek( savePos, SeekOrigin.Begin );

            BindPoint_1 = reader.ReadInt16();
            BindPoint_2 = reader.ReadInt16();
            BindPoint_3 = reader.ReadInt16();
            bindPoint_4 = reader.ReadInt16();

            Unk_Pairs = ReadPairs( 4, reader, startPos );

            Unk_4 = reader.ReadInt32();
            Unk_5 = reader.ReadInt32();
        }

        public override int GetSize() => 0x48;
        public override int GetExtraSize() => 4 * Unk_Pairs.Select(x => x.Count).Sum();

        public override void Write( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, Dictionary<string, int> stringPositions, int stringPos ) {
            var startPos = ( int )entryWriter.BaseStream.Position + entryPos;
            var endPos = stringPositions[Path] + stringPos;
            var offset = endPos - startPos - 8;

            FileHelper.WriteString( entryWriter, "C012" );
            entryWriter.Write( GetSize() );
            entryWriter.Write( Id );
            entryWriter.Write( Time );
            entryWriter.Write( Unk_2 );
            entryWriter.Write( Unk_3 );

            entryWriter.Write( offset );

            entryWriter.Write( BindPoint_1 );
            entryWriter.Write( BindPoint_2 );
            entryWriter.Write( BindPoint_3 );
            entryWriter.Write( bindPoint_4 );

            WritePairs( entryWriter, extraWriter, extraPos, startPos, Unk_Pairs );

            entryWriter.Write( Unk_4 );
            entryWriter.Write( Unk_5 );
        }

        public override string GetName() => Name;

        public override void Draw( string id ) {
            FileHelper.ShortInput( $"Time{id}", ref Time );
            ImGui.InputInt( $"Uknown 2{id}", ref Unk_2 );
            ImGui.InputInt( $"Uknown 3{id}", ref Unk_3 );
            ImGui.InputInt( $"Uknown 4{id}", ref Unk_4 );
            ImGui.InputInt( $"Uknown 5{id}", ref Unk_5 );
            ImGui.InputText( $"Path{id}", ref Path, 255 );
            FileHelper.ShortInput( $"Bind Point 1{id}", ref BindPoint_1 );
            FileHelper.ShortInput( $"Bind Point 2{id}", ref BindPoint_2 );
            FileHelper.ShortInput( $"Bind Point 3{id}", ref BindPoint_3 );
            FileHelper.ShortInput( $"Bind Point 4{id}", ref bindPoint_4 );

            DrawPairs( id, Unk_Pairs );
        }

        public override void PopulateStringList( List<string> stringList ) {
            AddString( Path, stringList );
        }
    }
}