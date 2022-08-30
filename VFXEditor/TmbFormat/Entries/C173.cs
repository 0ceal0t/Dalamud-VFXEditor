using ImGuiNET;
using System.Numerics;
using VFXEditor.Helper;
using VFXEditor.TmbFormat.Utils;

namespace VFXEditor.TmbFormat.Entries {
    public class C173 : TmbEntry {
        public const string MAGIC = "C173";
        public override string DisplayName => "VFX (C173)";
        public override string Magic => MAGIC;

        public override int Size => 0x44;
        public override int ExtraSize => 0;

        private int Unk1 = 30;
        private int Unk2 = 0;
        private string Path = "";
        private short Bindpoint1 = 1;
        private short Bindpoint2 = 0xFF;
        private int Unk3 = 0;
        private int Unk4 = 0;
        private int Unk5 = 0;
        private int Unk6 = 0;
        private int Unk7 = 0;
        private int Unk8 = 0;
        private int Unk9 = 0;
        private int Unk10 = 0;
        private int Unk11 = 0;
        private int Unk12 = 0;

        public C173() : base() { }

        public C173( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            Unk1 = reader.ReadInt32();
            Unk2 = reader.ReadInt32();
            Path = reader.ReadOffsetString();
            Bindpoint1 = reader.ReadInt16();
            Bindpoint2 = reader.ReadInt16();
            Unk3 = reader.ReadInt32();
            Unk4 = reader.ReadInt32();
            Unk5 = reader.ReadInt32();
            Unk6 = reader.ReadInt32();
            Unk7 = reader.ReadInt32();
            Unk8 = reader.ReadInt32();
            Unk9 = reader.ReadInt32();
            Unk10 = reader.ReadInt32();
            Unk11 = reader.ReadInt32();
            Unk12 = reader.ReadInt32();
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            writer.Write( Unk1 );
            writer.Write( Unk2 );
            writer.WriteOffsetString( Path );
            writer.Write( Bindpoint1 );
            writer.Write( Bindpoint2 );
            writer.Write( Unk3 );
            writer.Write( Unk4 );
            writer.Write( Unk5 );
            writer.Write( Unk6 );
            writer.Write( Unk7 );
            writer.Write( Unk8 );
            writer.Write( Unk9 );
            writer.Write( Unk10 );
            writer.Write( Unk11 );
            writer.Write( Unk12 );
        }

        public override void Draw( string id ) {
            DrawHeader( id );
            ImGui.InputInt( $"Unknown 1{id}", ref Unk1 );
            ImGui.InputInt( $"Unknown 2{id}", ref Unk2 );
            ImGui.InputText( $"Path{id}", ref Path, 255 );
            FileHelper.ShortInput( $"Bind Point 1{id}", ref Bindpoint1 );
            FileHelper.ShortInput( $"Bind Point 2{id}", ref Bindpoint2 );
            ImGui.InputInt( $"Unknown 3{id}", ref Unk3 );
            ImGui.InputInt( $"Unknown 4{id}", ref Unk4 );
            ImGui.InputInt( $"Unknown 5{id}", ref Unk5 );
            ImGui.InputInt( $"Unknown 6{id}", ref Unk6 );
            ImGui.InputInt( $"Unknown 7{id}", ref Unk7 );
            ImGui.InputInt( $"Unknown 8{id}", ref Unk8 );
            ImGui.InputInt( $"Unknown 9{id}", ref Unk9 );
            ImGui.InputInt( $"Unknown 10{id}", ref Unk10 );
            ImGui.InputInt( $"Unknown 11{id}", ref Unk11 );
            ImGui.InputInt( $"Unknown 12{id}", ref Unk12 );
        }
    }
}
