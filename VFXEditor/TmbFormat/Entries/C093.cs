using ImGuiNET;
using System.Numerics;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C093 : TmbEntry {
        public const string MAGIC = "C093";
        public const string DISPLAY_NAME = "C093";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x28;
        public override int ExtraSize => 4 * ( 4 + 4 );

        private readonly ParsedInt Duration = new( "Duration" );
        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly ParsedFloat4 Unk2 = new( "Unknown 2" );
        private readonly ParsedFloat4 Unk3 = new( "Unknown 3" );
        private readonly ParsedInt Unk4 = new( "Unknown 4" );

        public C093() : base() {
            Duration.Value = 30;
            Unk2.Value = new( 1 );
            Unk3.Value = new( 1 );
        }

        public C093( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            Duration.Read( reader );
            Unk1.Read( reader );
            Unk2.Value = reader.ReadOffsetVector4();
            Unk3.Value = reader.ReadOffsetVector4();
            Unk4.Read( reader );
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            Duration.Write( writer );
            Unk1.Write( writer );
            writer.WriteExtraVector4( Unk2.Value );
            writer.WriteExtraVector4( Unk3.Value );
            Unk4.Write( writer );
        }

        public override void Draw( string id ) {
            DrawTime( id );
            Duration.Draw( id, CommandManager.Tmb );
            Unk1.Draw( id, CommandManager.Tmb );
            Unk2.Draw( id, CommandManager.Tmb );
            Unk3.Draw( id, CommandManager.Tmb );
            Unk4.Draw( id, CommandManager.Tmb );
        }
    }
}
