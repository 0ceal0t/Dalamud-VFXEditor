using ImGuiNET;
using System.Numerics;
using VFXEditor.TmbFormat.Utils;

namespace VFXEditor.TmbFormat.Entries {
    public class C094 : TmbEntry {
        public const string MAGIC = "C094";
        public override string DisplayName => "Invisibility (C094)";
        public override string Magic => MAGIC;

        public override int Size => 0x20;
        public override int ExtraSize => 0x14;

        private int Duration = 0;
        private int Unk1 = 0;
        private float StartVisibility = 0;
        private float EndVisibility = 0;
        private int Unk2 = 0;
        private int Unk3 = 0;
        private int Unk4 = 0;
        private int Unk5 = 0;
        private int Unk6 = 0;

        // Unk2 = 1, Unk3 = 8 -> ExtraSize = 0x14

        public C094() : base() { }

        public C094( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            Duration = reader.ReadInt32();
            Unk1 = reader.ReadInt32();
            StartVisibility = reader.ReadSingle();
            EndVisibility = reader.ReadSingle();

            reader.ReadAtOffset( ( binaryReader ) => {
                Unk2 = binaryReader.ReadInt32();
                Unk3 = binaryReader.ReadInt32();
                Unk4 = binaryReader.ReadInt32();
                Unk5 = binaryReader.ReadInt32();
                Unk6 = binaryReader.ReadInt32();
            } );
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            writer.Write( Duration );
            writer.Write( Unk1 );
            writer.Write( StartVisibility );
            writer.Write( EndVisibility );

            writer.WriteExtra( ( binaryWriter ) => {
                binaryWriter.Write( Unk2 );
                binaryWriter.Write( Unk3 );
                binaryWriter.Write( Unk4 );
                binaryWriter.Write( Unk5 );
                binaryWriter.Write( Unk6 );
            } );
        }

        public override void Draw( string id ) {
            DrawHeader( id );
            ImGui.InputInt( $"Duration{id}", ref Duration );
            ImGui.InputInt( $"Unknown 1{id}", ref Unk1 );
            ImGui.InputFloat( $"Start Visibility{id}", ref StartVisibility );
            ImGui.InputFloat( $"End Visibility{id}", ref EndVisibility );

            ImGui.InputInt( $"Unknown 2{id}", ref Unk2 );
            ImGui.InputInt( $"Unknown 3{id}", ref Unk3 );
            ImGui.InputInt( $"Unknown 4{id}", ref Unk4 );
            ImGui.InputInt( $"Unknown 5{id}", ref Unk5 );
            ImGui.InputInt( $"Unknown 6{id}", ref Unk6 );
        }
    }
}
