using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using VfxEditor.FileManager;
using VfxEditor.SklbFormat.Bones;
using VfxEditor.SklbFormat.Data;
using VfxEditor.SklbFormat.Layers;
using VfxEditor.Utils;

namespace VfxEditor.SklbFormat {
    public class SklbFile : FileManagerFile {
        private readonly string HkxTempLocation;

        private readonly short Version1;
        private readonly short Version2;
        private readonly SklbData Data;
        private readonly SklbLayers Layers;
        private readonly int Padding;

        public readonly SklbBones Bones;

        public readonly HashSet<nint> Handles = [];

        public SklbFile( BinaryReader reader, string hkxTemp, bool init, bool verify ) : base() {
            HkxTempLocation = hkxTemp;

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
                Dalamud.Error( $"Invalid SKLB version: {Version1:X4} {Version2:X4}" );
                return;
            }

            Layers = new( this, reader );

            Padding = ( int )( Data.HavokOffset - reader.BaseStream.Position ); // Do this b/c of modded skeletons like IVCS

            reader.BaseStream.Position = Data.HavokOffset;
            var havokData = reader.ReadBytes( ( int )( reader.BaseStream.Length - Data.HavokOffset ) );
            File.WriteAllBytes( HkxTempLocation, havokData );

            Bones = new( this, HkxTempLocation, init );

            if( verify ) Verified = FileUtils.Verify( reader, ToBytes() );
        }

        public override void Update() {
            base.Update();
            Bones.Write( Handles );
        }

        public override void Write( BinaryWriter writer ) {
            FileUtils.WriteMagic( writer, "sklb" );
            writer.Write( Version1 );
            writer.Write( Version2 );

            if( Data == null ) return;
            var havokOffsetPos = Data.Write( writer );

            Layers.Write( writer );

            FileUtils.Pad( writer, Padding );

            var havokOffset = writer.BaseStream.Position;

            writer.BaseStream.Position = havokOffsetPos;
            if( Data is SklbOldData ) {
                writer.Write( ( short )havokOffset );
            }
            else {
                writer.Write( ( int )havokOffset );
            }

            // Reset position
            writer.BaseStream.Position = havokOffset;

            var data = File.ReadAllBytes( HkxTempLocation );
            writer.Write( data );
        }

        public override void Draw() {
            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            DrawData();
            DrawLayers();
            DrawBones();
            DrawBoneList();
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

        private void DrawBoneList() {
            using var tabItem = ImRaii.TabItem( "Bone List" );
            if( !tabItem ) return;

            using var _ = ImRaii.PushId( "BoneList" );

            Bones?.ListView.Draw();
        }

        private void DrawMappings() {
            using var tabItem = ImRaii.TabItem( "Mappings" );
            if( !tabItem ) return;

            using var _ = ImRaii.PushId( "Mappings" );

            Bones?.MappingView.Draw();
        }

        public override void OnChange() {
            Bones?.Updated();
        }

        public override void Dispose() {
            base.Dispose();
            foreach( var item in Handles ) Marshal.FreeHGlobal( item );
            Handles.Clear();
        }
    }
}
