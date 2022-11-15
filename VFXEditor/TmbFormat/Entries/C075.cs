using ImGuiNET;
using System.Numerics;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C075 : TmbEntry {
        public const string MAGIC = "C075";
        public const string DISPLAY_NAME = "C075";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x40;
        public override int ExtraSize => 4 * ( 3 + 3 + 3 + 4 );

        private readonly ParsedInt Duration = new( "Duration" );
        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedFloat3 Scale = new( "Scale" );
        private readonly ParsedFloat3 Rotation = new( "Rotation" );
        private readonly ParsedFloat3 Position = new( "Position" );
        private readonly ParsedFloat4 RGBA = new( "RGBA" );
        private readonly ParsedInt Unk3 = new( "Unknown 3" );
        private readonly ParsedInt Unk4 = new( "Unknown 4" );

        public C075() : base() {
            Scale.Value = new( 1 );
            RGBA.Value = new( 1 );
        }

        public C075( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            Duration.Read( reader );
            Unk1.Read( reader );
            Unk2.Read( reader );
            Scale.Value = reader.ReadOffsetVector3();
            Rotation.Value = reader.ReadOffsetVector3();
            Position.Value = reader.ReadOffsetVector3();
            RGBA.Value = reader.ReadOffsetVector4();
            Unk3.Read( reader );
            Unk4.Read( reader );
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            Duration.Write( writer );
            Unk1.Write( writer );
            Unk2.Write( writer );
            writer.WriteExtraVector3( Scale.Value );
            writer.WriteExtraVector3( Rotation.Value );
            writer.WriteExtraVector3( Position.Value );
            writer.WriteExtraVector4( RGBA.Value );
            Unk3.Write( writer );
            Unk4.Write( writer );
        }

        public override void Draw( string id ) {
            DrawTime( id );
            Duration.Draw( id, CommandManager.Tmb );
            Unk1.Draw( id, CommandManager.Tmb );
            Unk2.Draw( id, CommandManager.Tmb );
            Scale.Draw( id, CommandManager.Tmb );
            Rotation.Draw( id, CommandManager.Tmb );
            Position.Draw( id, CommandManager.Tmb );
            RGBA.Draw( id, CommandManager.Tmb );
            Unk3.Draw( id, CommandManager.Tmb );
            Unk4.Draw( id, CommandManager.Tmb );
        }
    }
}
