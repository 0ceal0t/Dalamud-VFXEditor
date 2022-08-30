using Dalamud.Logging;
using ImGuiNET;
using System.Numerics;
using VFXEditor.TmbFormat.Utils;

namespace VFXEditor.TmbFormat.Entries {
    public class C075 : TmbEntry {
        public const string MAGIC = "C075";
        public const string DISPLAY_NAME = "C075";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x40;
        public override int ExtraSize => 4 * ( 3 + 3 + 3 + 4 );

        private int Duration;
        private int Unk1;
        private int Unk2;
        private Vector3 Scale = new( 1 );
        private Vector3 Rotation = new( 0 );
        private Vector3 Position = new( 0 );
        private Vector4 RGBA = new( 1 );
        private int Unk3;
        private int Unk4;

        public C075() : base() { }

        public C075( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            Duration = reader.ReadInt32();
            Unk1 = reader.ReadInt32();
            Unk2 = reader.ReadInt32();
            Scale = reader.ReadOffsetVector3();
            Rotation = reader.ReadOffsetVector3();
            Position = reader.ReadOffsetVector3();
            RGBA = reader.ReadOffsetVector4();
            Unk3 = reader.ReadInt32();
            Unk4 = reader.ReadInt32();
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            writer.Write( Duration );
            writer.Write( Unk1 );
            writer.Write( Unk2 );
            writer.WriteExtraVector3( Scale );
            writer.WriteExtraVector3( Rotation );
            writer.WriteExtraVector3( Position );
            writer.WriteExtraVector4( RGBA );
            writer.Write( Unk3 );
            writer.Write( Unk4 );
        }

        public override void Draw( string id ) {
            DrawHeader( id );
            ImGui.InputInt( $"Duration{id}", ref Duration );
            ImGui.InputInt( $"Unknown 1{id}", ref Unk1 );
            ImGui.InputInt( $"Unknown 2{id}", ref Unk2 );
            ImGui.InputFloat3( $"Scale{id}", ref Scale );
            ImGui.InputFloat3( $"Rotation{id}", ref Rotation );
            ImGui.InputFloat3( $"Position{id}", ref Position );
            ImGui.InputFloat4( $"RGBA{id}", ref RGBA );
            ImGui.InputInt( $"Unknown 3{id}", ref Unk3 );
            ImGui.InputInt( $"Unknown 4{id}", ref Unk4 );
        }
    }
}
