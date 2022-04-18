using System;
using System.Collections.Generic;
using System.IO;
using ImGuiNET;
using VFXEditor.Helper;

namespace VFXEditor.TMB.TMB {
    public class C053 : TMBItem {
        private int Unk_2 = 0;
        private int Unk_3 = 0;
        private short Unk_4 = 0;
        private short Unk_5 = 0;
        private int Unk_6 = 0;

        public static readonly string DisplayName = "C053";
        public override string GetDisplayName() => DisplayName;
        public override string GetName() => "C053";

        public C053() { }
        public C053( BinaryReader reader ) {
            ReadInfo( reader );
            Unk_2 = reader.ReadInt32();
            Unk_3 = reader.ReadInt32();
            Unk_4 = reader.ReadInt16();
            Unk_5 = reader.ReadInt16();
            Unk_6 = reader.ReadInt32();
        }

        public override int GetSize() => 0x1C;
        public override int GetExtraSize() => 0;

        public override void Write( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, Dictionary<string, int> stringPositions, int stringPos ) {
            WriteInfo( entryWriter );
            entryWriter.Write( Unk_2 );
            entryWriter.Write( Unk_3 );
            entryWriter.Write( Unk_4 );
            entryWriter.Write( Unk_5 );
            entryWriter.Write( Unk_6 );
        }

        public override void Draw( string id ) {
            DrawInfo( id );
            ImGui.InputInt( $"Uknown 2{id}", ref Unk_2 );
            ImGui.InputInt( $"Uknown 3{id}", ref Unk_3 );
            FileHelper.ShortInput( $"Unknown 4{id}", ref Unk_4 );
            FileHelper.ShortInput( $"Unknown 5{id}", ref Unk_5 );
            ImGui.InputInt( $"Uknown 6{id}", ref Unk_6 );
        }

        public override void PopulateStringList( List<string> stringList ) {
        }
    }
}
