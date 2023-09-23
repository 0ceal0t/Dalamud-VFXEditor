using Dalamud.Logging;
using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VfxEditor.FileManager;
using VfxEditor.Formats.SkpFormat.LookAt;
using VfxEditor.Parsing;
using VfxEditor.Utils;

namespace VfxEditor.Formats.SkpFormat {
    [Flags]
    public enum SkpFlags {
        Animation = 0x01,
        LookAt = 0x02,
        CCD_Unsupported = 0x04,
        Feet_Unknown = 0x08,
        Slope = 0x10
    }

    public class SkpFile : FileManagerFile {
        public static List<(int, int)> VerifyIgnore { get; private set; } = null;

        private readonly int Version;
        private bool NewVersion => Version > 1000;
        private readonly ParsedFlag<SkpFlags> Activated = new( "Activated" );

        private readonly SkpLookAt LookAt = new();

        public SkpFile( BinaryReader reader, bool verify ) : base( new( Plugin.SkpManager ) ) {
            reader.ReadInt32(); // Magic
            Version = int.Parse( FileUtils.Reverse( Encoding.ASCII.GetString( reader.ReadBytes( 4 ) ) ) );
            Activated.Read( reader );

            // header size (0x20 if new, 0x1C otherwise). Also the offset to [Animation] if its activated
            // Offset to [LookAt] if it's activated, 0 otherwise
            // [EOF] if [CCD] is activated, 0 otherwise
            // 0, no examples of this found yet
            // If NEW, offset of Slope if activated, 0 otherwise
            reader.ReadBytes( NewVersion ? 20 : 16 );

            if( Activated.HasFlag( SkpFlags.Animation ) ) {
                // TODO
            }

            if( Activated.HasFlag( SkpFlags.LookAt ) ) LookAt.Read( reader );
            if( Activated.HasFlag( SkpFlags.Feet_Unknown ) ) PluginLog.Error( "FootIK found, please report this" );

            if( NewVersion && Activated.HasFlag( SkpFlags.Slope ) ) {
                // TODO
            }

            VerifyIgnore = new();
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
                // TODO
            }

            var lookAtOffset = 0;
            if( Activated.HasFlag( SkpFlags.LookAt ) ) {
                lookAtOffset = ( int )writer.BaseStream.Position;
                LookAt.Write( writer );
            }

            var slopeOffset = 0;
            if( NewVersion && Activated.HasFlag( SkpFlags.Slope ) ) {
                slopeOffset = ( int )writer.BaseStream.Position;
                // TODO
            }

            var size = ( int )writer.BaseStream.Length;
            var ccdOffset = Activated.HasFlag( SkpFlags.CCD_Unsupported ) ? size : 0;

            writer.BaseStream.Seek( offsetPosition, SeekOrigin.Begin );
            writer.Write( lookAtOffset );
            writer.Write( ccdOffset );
            writer.Write( 0 ); // footOffset
            if( NewVersion ) writer.Write( slopeOffset );
        }

        public override void Draw() {
            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            DrawParameters();
            DrawAnimation();
            DrawLookAt();
            DrawSlope();
        }

        private void DrawParameters() {
            using var tabItem = ImRaii.TabItem( "Parameters" );
            if( !tabItem ) return;

            ImGui.TextDisabled( $"Version {Version}" );
            Activated.Draw( CommandManager.Skp );
        }

        private void DrawAnimation() {
            if( !Activated.HasFlag( SkpFlags.Animation ) ) return;

            using var tabItem = ImRaii.TabItem( "Animation" );
            if( !tabItem ) return;

            using var _ = ImRaii.PushId( "Animation" );

            // TODO
        }

        private void DrawLookAt() {
            if( !Activated.HasFlag( SkpFlags.LookAt ) ) return;

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

            // TODO
        }
    }
}
