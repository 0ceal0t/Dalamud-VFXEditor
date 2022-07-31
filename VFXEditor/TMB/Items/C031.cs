using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VFXEditor.Helper;

namespace VFXEditor.TMB.Items {
    public class C031 : TMBItem {
        private int Duration = 0;
        private int Unk_3 = 0;
        private short AnimationId = 0;
        private short Unk_5 = 2;

        public static readonly string DisplayName = "Summon Animation (C031)";
        public override string GetDisplayName() => DisplayName;
        public override string GetName() => "C031";

        public C031() { }
        public C031( BinaryReader reader ) {
            ReadInfo( reader );
            Duration = reader.ReadInt32();
            Unk_3 = reader.ReadInt32();
            AnimationId = reader.ReadInt16();
            Unk_5 = reader.ReadInt16();
        }

        public override int GetSize() => 0x18;
        public override int GetExtraSize() => 0;

        public override void Write( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, Dictionary<string, int> stringPositions, int stringPos ) {
            WriteInfo( entryWriter );
            entryWriter.Write( Duration );
            entryWriter.Write( Unk_3 );
            entryWriter.Write( AnimationId );
            entryWriter.Write( Unk_5 );
        }

        public override void Draw( string id ) {
            DrawInfo( id );
            ImGui.InputInt( $"Duration{id}", ref Duration );
            ImGui.InputInt( $"Unknown 3{id}", ref Unk_3 );
            FileHelper.ShortInput( $"Animation Id{id}", ref AnimationId );
            FileHelper.ShortInput( $"Unknown 5{id}", ref Unk_5 );
        }

        public override void PopulateStringList( List<string> stringList ) {
        }
    }
}
