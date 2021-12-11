using System.Collections.Generic;
using System.IO;
using ImGuiNET;
using VFXEditor.Helper;

namespace VFXEditor.Tmb.Tmb {
    public class C094 : TmbItem {
        private short Time = 0;
        private int Unk_2 = 0;
        private int Unk_3 = 0;
        private int Unk_4 = 0;
        private int Unk_5 = 0;

        private int Unk2_1 = 0;
        private int Unk2_2 = 0;
        private int Unk2_3 = 0;
        private int Unk2_4 = 0;
        private int Unk2_5 = 0;

        public static readonly string Name = "C094";

        public C094() { }
        public C094( BinaryReader reader ) {
            var startPos = reader.BaseStream.Position; // [C012] + 8

            reader.ReadInt16(); // id
            Time = reader.ReadInt16();
            Unk_2 = reader.ReadInt32();
            Unk_3 = reader.ReadInt32();
            Unk_4 = reader.ReadInt32();
            Unk_5 = reader.ReadInt32();

            var offset = reader.ReadInt32();
            var savePos = reader.BaseStream.Position;
            reader.BaseStream.Seek( startPos + offset, SeekOrigin.Begin );

            Unk2_1 = reader.ReadInt32();
            Unk2_2 = reader.ReadInt32();
            Unk2_3 = reader.ReadInt32();
            Unk2_4 = reader.ReadInt32();
            Unk2_5 = reader.ReadInt32();

            reader.BaseStream.Seek( savePos, SeekOrigin.Begin );
        }

        public override int GetSize() => 0x20;
        public override int GetExtraSize() => 0x14;

        public override void Write( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, Dictionary<string, int> stringPositions, int stringPos ) {
            var startPos = ( int )entryWriter.BaseStream.Position + entryPos;

            FileHelper.WriteString( entryWriter, "C094" );
            entryWriter.Write( GetSize() );
            entryWriter.Write( Id );
            entryWriter.Write( Time );
            entryWriter.Write( Unk_2 );
            entryWriter.Write( Unk_3 );
            entryWriter.Write( Unk_4 );
            entryWriter.Write( Unk_5 );

            var extraEndPos = ( int )extraWriter.BaseStream.Position + extraPos;
            var extraOffset = extraEndPos - startPos - 8;
            entryWriter.Write( extraOffset );

            extraWriter.Write( Unk2_1 );
            extraWriter.Write( Unk2_2 );
            extraWriter.Write( Unk2_3 );
            extraWriter.Write( Unk2_4 );
            extraWriter.Write( Unk2_5 );
        }

        public override string GetName() => Name;

        public override void Draw( string id ) {
            FileHelper.ShortInput( $"Time{id}", ref Time );
            ImGui.InputInt( $"Uknown 2{id}", ref Unk_2 );
            ImGui.InputInt( $"Uknown 3{id}", ref Unk_3 );
            ImGui.InputInt( $"Uknown 4{id}", ref Unk_4 );
            ImGui.InputInt( $"Uknown 5{id}", ref Unk_5 );
        }

        public override void PopulateStringList( List<string> stringList ) {
        }
    }
}
