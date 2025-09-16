using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VfxEditor.FileManager;
using VfxEditor.Formats.SkpFormat.LookAt;
using VfxEditor.Formats.SkpFormat.Slope;
using VfxEditor.Parsing;
using VfxEditor.SklbFormat.Layers;
using VfxEditor.Utils;

namespace VfxEditor.Formats.SkpFormat {
    [Flags]
    public enum SkpFlags {
        Animation = 0x01,
        Look_At = 0x02,
        CCD = 0x04,
        Feet = 0x08,
        Slope = 0x10
    }

    public class SkpFile : FileManagerFile {
        public static List<(int, int)> VerifyIgnore { get; private set; } = null;

        private readonly int Version;
        private bool NewVersion => Version > 1000;
        private readonly ParsedFlag<SkpFlags> Activated = new( "Activated" );

        private readonly SklbLayers Animation = new( null );
        private readonly SkpLookAt LookAt = new();
        private readonly SkpSlope Slope = new();

        public SkpFile( BinaryReader reader, bool verify ) : base() {
            reader.ReadInt32(); // Magic
            Version = int.Parse( FileUtils.Reverse( Encoding.ASCII.GetString( reader.ReadBytes( 4 ) ) ) );
            Activated.Read( reader );

            // header size (0x20 if new, 0x1C otherwise). Also the offset to [Animation] if its activated
            // Offset to [LookAt] if it's activated, 0 otherwise
            // [EOF] if [CCD] is activated, 0 otherwise
            // 0, no examples of this found yet
            // If NEW, offset of Slope if activated, 0 otherwise
            reader.ReadBytes( NewVersion ? 20 : 16 );

            if( Activated.HasFlag( SkpFlags.Animation ) ) Animation.Read( reader );
            if( Activated.HasFlag( SkpFlags.Look_At ) ) LookAt.Read( reader );
            if( Activated.HasFlag( SkpFlags.Feet ) ) Dalamud.Error( "FootIK found, please report this" );
            if( NewVersion && Activated.HasFlag( SkpFlags.Slope ) ) Slope.Read( reader );

            VerifyIgnore = [];
            var data = ToBytes(); // will populate VerifyIgnore
            if( verify ) Verified = FileUtils.Verify( reader, data, VerifyIgnore );
            VerifyIgnore = null;
        }

        public override void Write( BinaryWriter writer ) {
            FileUtils.WriteMagic( writer, "sklp" );
            FileUtils.WriteMagic( writer, Version.ToString() );
            Activated.Write( writer );

            writer.Write( NewVersion ? 0x20 : 0x1C );
            var offsetPosition = writer.BaseStream.Position;
            FileUtils.Pad( writer, NewVersion ? 16 : 12 );

            if( Activated.HasFlag( SkpFlags.Animation ) ) {
                Animation.Write( writer );
            }

            var lookAtOffset = 0;
            if( Activated.HasFlag( SkpFlags.Look_At ) ) {
                lookAtOffset = ( int )writer.BaseStream.Position;
                LookAt.Write( writer );
            }

            var slopeOffset = 0;
            if( NewVersion && Activated.HasFlag( SkpFlags.Slope ) ) {
                slopeOffset = ( int )writer.BaseStream.Position;
                Slope.Write( writer );
            }

            var size = ( int )writer.BaseStream.Length;
            var ccdOffset = Activated.HasFlag( SkpFlags.CCD ) ? size : 0;

            writer.BaseStream.Position = offsetPosition;
            writer.Write( lookAtOffset );
            writer.Write( ccdOffset );
            writer.Write( 0 ); // foot offset, no example of it being used
            if( NewVersion ) writer.Write( slopeOffset );
        }

        public override void Draw() {
            ImGui.Separator();

            ImGui.TextDisabled( $"Version: {Version}" );
            Activated.Draw();

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            DrawAnimation();
            DrawLookAt();
            DrawSlope();
        }

        private void DrawAnimation() {
            if( !Activated.HasFlag( SkpFlags.Animation ) ) return;

            using var tabItem = ImRaii.TabItem( "Animation" );
            if( !tabItem ) return;

            using var _ = ImRaii.PushId( "Animation" );

            Animation.Draw();
        }

        private void DrawLookAt() {
            if( !Activated.HasFlag( SkpFlags.Look_At ) ) return;

            using var tabItem = ImRaii.TabItem( "Look At" );
            if( !tabItem ) return;

            using var _ = ImRaii.PushId( "Look At" );

            LookAt.Draw();
        }

        private void DrawSlope() {
            if( !NewVersion || !Activated.HasFlag( SkpFlags.Slope ) ) return;

            using var tabItem = ImRaii.TabItem( "Slope" );
            if( !tabItem ) return;

            using var _ = ImRaii.PushId( "Slope" );
            using var child = ImRaii.Child( "Child" );

            Slope.Draw();
        }
    }
}
