using ImGuiNET;
using System.Numerics;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C094 : TmbEntry {
        public const string MAGIC = "C094";
        public const string DISPLAY_NAME = "Invisibility (C094)";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x20;
        public override int ExtraSize => UnkExtraData ? 0x14 : 0;

        private bool UnkExtraData = true;

        private readonly ParsedInt Duration = new( "Duration" );
        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly ParsedFloat StartVisibility = new( "Start Visibility" );
        private readonly ParsedFloat EndVisibility = new( "End Visibility" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedInt Unk3 = new( "Unknown 3" );
        private readonly ParsedInt Unk4 = new( "Unknown 4" );
        private readonly ParsedInt Unk5 = new( "Unknown 5" );
        private readonly ParsedInt Unk6 = new( "Unknown 6" );

        // Unk2 = 1, Unk3 = 8 -> ExtraSize = 0x14

        public C094() : base() { }

        public C094( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            Duration.Read( reader );
            Unk1.Read( reader );
            StartVisibility.Read( reader );
            EndVisibility.Read( reader );

            UnkExtraData = reader.ReadAtOffset( ( binaryReader ) => {
                Unk2.Read( binaryReader );
                Unk3.Read( binaryReader );
                Unk4.Read( binaryReader );
                Unk5.Read( binaryReader );
                Unk6.Read( binaryReader );
            } );
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            Duration.Write( writer );
            Unk1.Write( writer );
            StartVisibility.Write( writer );
            EndVisibility.Write( writer );

            writer.WriteExtra( ( binaryWriter ) => {
                Unk2.Write( binaryWriter );
                Unk3.Write( binaryWriter );
                Unk4.Write( binaryWriter );
                Unk5.Write( binaryWriter );
                Unk6.Write( binaryWriter );
            }, UnkExtraData );
        }

        public override void Draw( string id ) {
            DrawTime( id );
            Duration.Draw( id, CommandManager.Tmb );
            Unk1.Draw( id, CommandManager.Tmb );
            StartVisibility.Draw( id, CommandManager.Tmb );
            EndVisibility.Draw( id, CommandManager.Tmb );

            ImGui.Checkbox( $"Unknown Extra Data{id}", ref UnkExtraData );
            if( UnkExtraData ) {
                Unk2.Draw( id, CommandManager.Tmb );
                Unk3.Draw( id, CommandManager.Tmb );
                Unk4.Draw( id, CommandManager.Tmb );
                Unk5.Draw( id, CommandManager.Tmb );
                Unk6.Draw( id, CommandManager.Tmb );
            }
        }
    }
}
