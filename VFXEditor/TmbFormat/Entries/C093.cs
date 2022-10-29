using ImGuiNET;
using System.Numerics;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C093 : TmbEntry {
        public const string MAGIC = "C093";
        public const string DISPLAY_NAME = "C093";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x28;
        public override int ExtraSize => 4 * ( 4 + 4 );

        private int Duration = 30;
        private int Unk1 = 0;
        private Vector4 Unk2 = new( 1 );
        private Vector4 Unk3 = new( 1 );
        private int Unk4 = 0;

        public C093() : base() { }

        public C093( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            Duration = reader.ReadInt32();
            Unk1 = reader.ReadInt32();
            Unk2 = reader.ReadOffsetVector4();
            Unk3 = reader.ReadOffsetVector4();
            Unk4 = reader.Reader.ReadInt32();
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            writer.Write( Duration );
            writer.Write( Unk1 );
            writer.WriteExtraVector4( Unk2 );
            writer.WriteExtraVector4( Unk3 );
            writer.Write( Unk4 );
        }

        public override void Draw( string id ) {
            DrawHeader( id );
            ImGui.InputInt( $"Duration{id}", ref Duration );
            ImGui.InputInt( $"Unknown 1{id}", ref Unk1 );
            ImGui.InputFloat4( $"Unknown 2{id}", ref Unk2 );
            ImGui.InputFloat4( $"Unknown 3{id}", ref Unk3 );
            ImGui.InputInt( $"Unknown 4{id}", ref Unk4 );
        }
    }
}
