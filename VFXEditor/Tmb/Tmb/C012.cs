using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        public static readonly string Name = "VFX (C012)";

        private static List<List<float>> GetDefault() {
            var ret = new List<List<float>>();
            ret.Add( new List<float>( new[] { 1f, 1f, 1f } ) );
            ret.Add( new List<float>( new[] { 0f, 0f, 0f } ) );
            ret.Add( new List<float>( new[] { 0f, 0f, 0f } ) );
            ret.Add( new List<float>( new[] { 1f, 1f, 1f, 1f } ) );
            ret.Add( new List<float>() );
            return ret;
        }

        public C012() {
            Unk_Pairs = GetDefault();
        }
        public C012( BinaryReader reader ) {
            var startPos = reader.BaseStream.Position; // [C012] + 8

            reader.ReadInt16(); // id
            Time = reader.ReadInt16(); // ?
            Unk_2 = reader.ReadInt32(); // 30?
            Unk_3 = reader.ReadInt32(); // 0?

            var offset = reader.ReadInt32(); // path offset: [C012] + offset + 8 = path
            var savePos = reader.BaseStream.Position;
            reader.BaseStream.Seek( startPos + offset, SeekOrigin.Begin );
            Path = TmbFile.ReadString( reader );
            reader.BaseStream.Seek( savePos, SeekOrigin.Begin );

            BindPoint_1 = reader.ReadInt16(); // 1?
            BindPoint_2 = reader.ReadInt16(); // FF?
            BindPoint_3 = reader.ReadInt16(); // 2?
            bindPoint_4 = reader.ReadInt16(); // FF?

            Unk_Pairs = ReadPairs( 5, reader, startPos );
        }

        public override int GetSize() => 0x48;
        public override int GetExtraSize() => 4 * Unk_Pairs.Select(x => x.Count).Sum();

        public override void Write( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, Dictionary<string, int> stringPositions, int stringPos, int timelinePos ) {
            var startPos = ( int )entryWriter.BaseStream.Position + entryPos;
            var endPos = stringPositions[Path] + stringPos;
            var offset = endPos - startPos - 8;

            TmbFile.WriteString( entryWriter, "C012" );
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
        }

        public override string GetName() => Name;

        public override void Draw( string id ) {
            TmbFile.ShortInput( $"Time{id}", ref Time );
            ImGui.InputInt( $"Uknown 2{id}", ref Unk_2 );
            ImGui.InputInt( $"Uknown 3{id}", ref Unk_3 );
            ImGui.InputText( $"Path{id}", ref Path, 255 );
            TmbFile.ShortInput( $"Bind Point 1{id}", ref BindPoint_1 );
            TmbFile.ShortInput( $"Bind Point 2{id}", ref BindPoint_2 );
            TmbFile.ShortInput( $"Bind Point 3{id}", ref BindPoint_3 );
            TmbFile.ShortInput( $"Bind Point 4{id}", ref bindPoint_4 );

            DrawPairs( id, Unk_Pairs );
        }

        public override void PopulateStringList( List<string> stringList ) {
            AddString( Path, stringList );
        }
    }
}