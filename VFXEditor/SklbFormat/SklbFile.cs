using Dalamud.Logging;
using ImGuiNET;
using OtterGui.Raii;
using System.IO;
using VfxEditor.FileManager;
using VfxEditor.SklbFormat.Bones;
using VfxEditor.SklbFormat.Data;
using VfxEditor.SklbFormat.Layers;
using VfxEditor.Utils;

namespace VfxEditor.SklbFormat {
    public class SklbFile : FileManagerFile {
        public readonly string HkxTempLocation;

        public readonly short Version1;
        public readonly short Version2;

        public readonly SklbData Data;
        public readonly SklbLayers Layers;
        public readonly SklbBones Bones;

        private readonly bool FinishedLoading = false;

        public SklbFile( BinaryReader reader, string hkxTemp, bool checkOriginal = true ) : base( new( Plugin.SklbManager, () => Plugin.SklbManager.CurrentFile?.Updated() ) ) {
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

            Layers = new( this, reader );

            reader.BaseStream.Seek( Data.HavokOffset, SeekOrigin.Begin );
            var havokData = reader.ReadBytes( ( int )( reader.BaseStream.Length - Data.HavokOffset ) );
            File.WriteAllBytes( HkxTempLocation, havokData );

            Bones = new( this, HkxTempLocation );

            if( checkOriginal ) Verified = FileUtils.CompareFiles( original, ToBytes(), out var _ );

            FinishedLoading = true;
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( 0x736B6C62 );
            writer.Write( Version1 );
            writer.Write( Version2 );

            if( Data == null ) return;
            var havokOffsetPos = Data.Write( writer );

            Layers.Write( writer );

            if( Data is SklbNewData ) FileUtils.PadTo( writer, 16 );

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

            if( FinishedLoading ) Bones.Write();
            var data = File.ReadAllBytes( HkxTempLocation );
            writer.Write( data );
        }

        public override void Draw() {
            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            DrawData();
            DrawLayers();
            DrawBones();
            DrawMappings();
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

            Bones?.Draw();
        }

        private void DrawMappings() {
            using var tabItem = ImRaii.TabItem( "Mappings" );
            if( !tabItem ) return;

            using var _ = ImRaii.PushId( "Mappings" );

            Bones?.MappingView.Draw();
        }

        public void Updated() {
            Bones?.Updated();
        }

        public override void Dispose() {
            Bones?.Dispose();
        }
    }
}
