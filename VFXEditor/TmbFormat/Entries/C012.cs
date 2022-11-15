using ImGuiNET;
using System.Numerics;
using VfxEditor.Utils;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.Parsing;

namespace VfxEditor.TmbFormat.Entries {
    public class C012 : TmbEntry {
        public const string MAGIC = "C012";
        public const string DISPLAY_NAME = "VFX (C012)";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x48;
        public override int ExtraSize => 4 * ( 3 + 3 + 3 + 4 );

        private readonly ParsedInt Duration = new( "Duration" );
        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private string Path = "";
        private readonly ParsedShort BindPoint1 = new( "Bind Point 1" );
        private readonly ParsedShort BindPoint2 = new( "Bind Point 2" );
        private readonly ParsedShort BindPoint3 = new( "Bind Point 3" );
        private readonly ParsedShort BindPoint4 = new( "Bind Point 4" );
        private readonly ParsedFloat3 Scale = new( "Scale" );
        private readonly ParsedFloat3 Rotation = new( "Rotation" );
        private readonly ParsedFloat3 Position = new( "Position" );
        private readonly ParsedFloat4 RGBA = new( "RGBA" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedInt Unk3 = new( "Unknown 3" );

        public C012() : base() {
            Duration.Value = 30;
            BindPoint1.Value = 1;
            BindPoint2.Value = 0xFF;
            BindPoint3.Value = 2;
            BindPoint4.Value = 0xFF;
            Scale.Value = new( 1 );
            RGBA.Value = new( 1 );
        }

        public C012( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            Duration.Read( reader );
            Unk1.Read( reader );
            Path = reader.ReadOffsetString();
            BindPoint1.Read( reader );
            BindPoint2.Read( reader );
            BindPoint3.Read( reader );
            BindPoint4.Read( reader );
            Scale.Value = reader.ReadOffsetVector3();
            Rotation.Value = reader.ReadOffsetVector3();
            Position.Value = reader.ReadOffsetVector3();
            RGBA.Value = reader.ReadOffsetVector4();
            Unk2.Read( reader );
            Unk3.Read( reader );

        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            Duration.Write( writer );
            Unk1.Write( writer );
            writer.WriteOffsetString( Path );
            BindPoint1.Write( writer );
            BindPoint2.Write( writer );
            BindPoint3.Write( writer );
            BindPoint4.Write( writer );
            writer.WriteExtraVector3( Scale.Value );
            writer.WriteExtraVector3( Rotation.Value );
            writer.WriteExtraVector3( Position.Value );
            writer.WriteExtraVector4( RGBA.Value );
            Unk2.Write( writer );
            Unk3.Write( writer );
        }

        public override void Draw( string id ) {
            DrawTime( id );
            Duration.Draw( id, CommandManager.Tmb );
            Unk1.Draw( id, CommandManager.Tmb );
            ImGui.InputText( $"Path{id}", ref Path, 255 );
            BindPoint1.Draw( id, CommandManager.Tmb );
            BindPoint2.Draw( id, CommandManager.Tmb );
            BindPoint3.Draw( id, CommandManager.Tmb );
            BindPoint4.Draw( id, CommandManager.Tmb );
            Scale.Draw( id, CommandManager.Tmb );
            Rotation.Draw( id, CommandManager.Tmb );
            Position.Draw( id, CommandManager.Tmb );
            RGBA.Draw( id, CommandManager.Tmb );
            Unk2.Draw( id, CommandManager.Tmb );
            Unk3.Draw( id, CommandManager.Tmb );
        }
    }
}
