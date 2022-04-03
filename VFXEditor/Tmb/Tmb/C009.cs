using System;
using System.Collections.Generic;
using System.IO;
using ImGuiNET;
using VFXEditor.Helper;

namespace VFXEditor.Tmb.Tmb {
    // Pap Animation
    public class C009 : TmbItem {
        private int Unk_2 = 50;
        private int Unk_3 = 0;
        private string Path = "";

        public static readonly string DisplayName = "Animation - PAP Only (C009)";
        public override string GetDisplayName() => DisplayName;
        public override string GetName() => "C009";

        public C009() { }
        public C009( BinaryReader reader ) {
            var startPos = reader.BaseStream.Position; // [C009] + 8

            ReadInfo( reader );
            Unk_2 = reader.ReadInt32();
            Unk_3 = reader.ReadInt32();

            var offset = reader.ReadInt32(); // offset: [C009] + offset + 8 = animation
            var savePos = reader.BaseStream.Position;
            reader.BaseStream.Seek( startPos + offset, SeekOrigin.Begin );
            Path = FileHelper.ReadString( reader );
            reader.BaseStream.Seek( savePos, SeekOrigin.Begin );
        }

        public override int GetSize() => 0x18;
        public override int GetExtraSize() => 0;

        public override void Write( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, Dictionary<string, int> stringPositions, int stringPos ) {
            var startPos = ( int )entryWriter.BaseStream.Position + entryPos;
            var endPos = stringPositions[Path] + stringPos;
            var offset = endPos - startPos - 8;

            WriteInfo( entryWriter );
            entryWriter.Write( Unk_2 );
            entryWriter.Write( Unk_3 );

            entryWriter.Write( offset );
        }

        public override void Draw( string id ) {
            DrawInfo( id );
            ImGui.InputInt( $"Uknown 2{id}", ref Unk_2 );
            ImGui.InputInt( $"Uknown 3{id}", ref Unk_3 );
            ImGui.InputText( $"Path{id}", ref Path, 255 );
        }

        public override void PopulateStringList( List<string> stringList ) {
            AddString( Path, stringList );
        }
    }
}
