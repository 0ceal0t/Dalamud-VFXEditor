using ImGuiNET;
using System.IO;
using VFXEditor.Helper;

namespace VFXEditor.TmbFormat {
    public class TmtrUnknownData {
        public int Unk1;
        public short Unk2;
        public short Unk3;
        public int Unk4;

        public TmtrUnknownData() {
            Unk1 = 0;
            Unk2 = 0;
            Unk3 = 0;
            Unk4 = 0;
        }

        public TmtrUnknownData( BinaryReader reader ) {
            Unk1 = reader.ReadInt32();
            Unk2 = reader.ReadInt16();
            Unk3 = reader.ReadInt16();
            Unk4 = reader.ReadInt32();
        }

        public void Write( BinaryWriter writer ) {
            writer.Write( Unk1 );
            writer.Write( Unk2 );
            writer.Write( Unk3 );
            writer.Write( Unk4 );
        }

        public void Draw( string id ) {
            ImGui.InputInt( $"Unknown 1{id}", ref Unk1 );
            FileHelper.ShortInput( $"Unknown 2{id}", ref Unk2 );
            FileHelper.ShortInput( $"Unknown 3{id}", ref Unk3 );
            ImGui.InputInt( $"Unknown 4{id}", ref Unk4 );
        }
    }
}
