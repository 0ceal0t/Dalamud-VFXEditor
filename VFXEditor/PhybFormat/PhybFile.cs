using ImGuiNET;
using OtterGui.Raii;
using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Parsing;
using VfxEditor.PhybFormat.Collision;
using VfxEditor.PhybFormat.Simulator;
using VfxEditor.Utils;

namespace VfxEditor.PhybFormat {
    public class PhybFile : FileManagerFile {
        public readonly ParsedUInt Version = new( "Version" );
        public readonly ParsedUInt DataType = new( "Data Type" );

        public readonly PhybCollision Collision;
        public readonly PhybSimulator Simulator;

        public PhybFile( BinaryReader reader, bool checkOriginal = true ) : base( new( Plugin.PhybManager ) ) {
            var original = checkOriginal ? FileUtils.GetOriginal( reader ) : null;

            Version.Read( reader );
            DataType.Read( reader );

            // chara/human/c0801/skeleton/met/m0173/phy_c0801m0173.phyb < Only 12 bytes long
            // 0x0 0x0C 0x0C

            var collisionOffset = reader.ReadUInt32();
            var simulatorOffset = reader.ReadUInt32();

            reader.BaseStream.Seek( collisionOffset, SeekOrigin.Begin );
            Collision = new( this, reader );

            reader.BaseStream.Seek( simulatorOffset, SeekOrigin.Begin );
            Simulator = new( this, reader );

            if( checkOriginal ) Verified = FileUtils.CompareFiles( original, ToBytes(), out var _ );
        }

        public override void Write( BinaryWriter writer ) {
            writer.BaseStream.Seek( 0, SeekOrigin.Begin );

            Version.Write( writer );
            DataType.Write( writer );

            var offsetPos = writer.BaseStream.Position; // coming back here later
            writer.Write( 0 ); // placeholders
            writer.Write( 0 );

            var collisionOffset = writer.BaseStream.Position;
            Collision.Write( writer );

            var simulatorOffset = writer.BaseStream.Position;
            Simulator.Write( writer );

            writer.BaseStream.Seek( offsetPos, SeekOrigin.Begin );
            writer.Write( ( int )collisionOffset );
            writer.Write( ( int )simulatorOffset );
        }

        public override void Draw() {
            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Collision" ) ) {
                if( tab ) Collision.Draw();
            }

            using( var tab = ImRaii.TabItem( "Simulator" ) ) {
                if( tab ) Simulator.Draw();
            }
        }
    }
}
