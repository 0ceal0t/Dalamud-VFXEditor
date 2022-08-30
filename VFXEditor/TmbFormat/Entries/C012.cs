using ImGuiNET;
using System.Numerics;
using VFXEditor.Utils;
using VFXEditor.TmbFormat.Utils;

namespace VFXEditor.TmbFormat.Entries {
    public class C012 : TmbEntry {
        public const string MAGIC = "C012";
        public const string DISPLAY_NAME = "VFX (C012)";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x48;
        public override int ExtraSize => 4 * ( 3 + 3 + 3 + 4 );

        private int Duration = 30;
        private int Unk1 = 0;
        private string Path = "";
        private short BindPoint1 = 1;
        private short BindPoint2 = 0xFF;
        private short BindPoint3 = 2;
        private short BindPoint4 = 0xFF;
        private Vector3 Scale = new( 1 );
        private Vector3 Rotation = new( 0 );
        private Vector3 Position = new( 0 );
        private Vector4 RGBA = new( 1 );
        private int Unk2;
        private int Unk3;

        public C012() : base() { }

        public C012( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            Duration = reader.ReadInt32();
            Unk1 = reader.ReadInt32();
            Path = reader.ReadOffsetString();
            BindPoint1 = reader.ReadInt16();
            BindPoint2 = reader.ReadInt16();
            BindPoint3 = reader.ReadInt16();
            BindPoint4 = reader.ReadInt16();
            Scale = reader.ReadOffsetVector3();
            Rotation = reader.ReadOffsetVector3();
            Position = reader.ReadOffsetVector3();
            RGBA = reader.ReadOffsetVector4();
            Unk2 = reader.ReadInt32();
            Unk3 = reader.ReadInt32();

        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            writer.Write( Duration );
            writer.Write( Unk1 );
            writer.WriteOffsetString( Path );
            writer.Write( BindPoint1 );
            writer.Write( BindPoint2 );
            writer.Write( BindPoint3 );
            writer.Write( BindPoint4 );
            writer.WriteExtraVector3( Scale );
            writer.WriteExtraVector3( Rotation );
            writer.WriteExtraVector3( Position );
            writer.WriteExtraVector4( RGBA );
            writer.Write( Unk2 );
            writer.Write( Unk3 );
        }

        public override void Draw( string id ) {
            DrawHeader( id );
            ImGui.InputInt( $"Duration{id}", ref Duration );
            ImGui.InputInt( $"Unknown 1{id}", ref Unk1 );
            ImGui.InputInt( $"Unknown 2{id}", ref Unk2 );
            ImGui.InputInt( $"Unknown 3{id}", ref Unk3 );
            ImGui.InputText( $"Path{id}", ref Path, 255 );
            FileUtils.ShortInput( $"Bind Point 1{id}", ref BindPoint1 );
            FileUtils.ShortInput( $"Bind Point 2{id}", ref BindPoint2 );
            FileUtils.ShortInput( $"Bind Point 3{id}", ref BindPoint3 );
            FileUtils.ShortInput( $"Bind Point 4{id}", ref BindPoint4 );

            ImGui.InputFloat3( $"Scale{id}", ref Scale );
            ImGui.InputFloat3( $"Rotation{id}", ref Rotation );
            ImGui.InputFloat3( $"Position{id}", ref Position );
            ImGui.InputFloat4( $"RGBA{id}", ref RGBA );
        }
    }
}
