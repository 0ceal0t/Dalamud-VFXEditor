using Dalamud.Logging;
using ImGuiNET;
using OtterGui.Raii;
using System.IO;
using VfxEditor.FileManager;
using VfxEditor.SklbFormat.Data;
using VfxEditor.SklbFormat.Layers;
using VfxEditor.Utils;

namespace VfxEditor.SklbFormat {
    public class SklbFile : FileManagerFile {
        public readonly string HkxTempLocation;

        private readonly short Version1;
        private readonly short Version2;

        private readonly SklbData Data;
        private readonly SklbLayers Layers;

        public SklbFile( BinaryReader reader, string hkxTemp, bool checkOriginal = true ) : base( new( Plugin.SklbManager ) ) {
            HkxTempLocation = hkxTemp;

            var original = checkOriginal ? FileUtils.GetOriginal( reader ) : null;

            reader.ReadInt32(); // Magic
            Version1 = reader.ReadInt16();
            Version2 = reader.ReadInt16();

            if( Version2 == 0x3133 ) {
                Data = new SklbNewData( reader );
            }
            else if( Version2 == 0x3132 ) { // Old
                Data = new SklbOldData( reader );
            }
            else {
                PluginLog.Error( $"Invalid SKLB version: {Version1:X8} {Version2:X8}" );
                return;
            }

            Layers = new( reader );

            reader.BaseStream.Seek( Data.HavokOffset, SeekOrigin.Begin );
            var havokData = reader.ReadBytes( ( int )( reader.BaseStream.Length - Data.HavokOffset ) );
            File.WriteAllBytes( HkxTempLocation, havokData );

            // TODO: read havok

            if( checkOriginal ) Verified = FileUtils.CompareFiles( original, ToBytes(), out var _ );
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( 0x736B6C62 );
            writer.Write( Version1 );
            writer.Write( Version2 );

            if( Data == null ) return;
            var havokOffsetPos = Data.Write( writer );

            Layers.Write( writer );
            var havokOffset = writer.BaseStream.Position;

            writer.BaseStream.Seek( havokOffsetPos, SeekOrigin.Begin );
            if( Data is SklbOldData ) {
                writer.Write( ( short )havokOffset );
            }
            else {
                writer.Write( ( int )havokOffset );
            }

            // Reset position
            writer.BaseStream.Seek( havokOffset, SeekOrigin.Begin );

            // TODO: update havok

            var data = File.ReadAllBytes( HkxTempLocation );
            writer.Write( data );
        }

        public override void Draw() {
            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            DrawData();
            DrawLayers();
            DrawBones();
        }

        private void DrawData() {
            using var tabItem = ImRaii.TabItem( "Data" );
            if( !tabItem ) return;

            using var _ = ImRaii.PushId( "Data" );
            using var child = ImRaii.Child( "Child " );

            Data?.Draw();
        }

        private void DrawLayers() {
            using var tabItem = ImRaii.TabItem( "Layers" );
            if( !tabItem ) return;

            using var _ = ImRaii.PushId( "Layers" );

            Layers?.Draw();
        }

        private void DrawBones() {
            using var tabItem = ImRaii.TabItem( "Bones" );
            if( !tabItem ) return;

            using var _ = ImRaii.PushId( "Bones" );
        }

        public override void Dispose() {

        }
    }
}
