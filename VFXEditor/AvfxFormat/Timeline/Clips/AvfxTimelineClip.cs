using ImGuiNET;
using OtterGui.Raii;
using System.IO;
using System.Numerics;
using VfxEditor.Parsing;
using Int4 = SharpDX.Int4;

namespace VfxEditor.AvfxFormat {
    public class AvfxTimelineClip : AvfxWorkspaceItem {
        public readonly AvfxTimeline Timeline;
        public readonly AvfxTimelineClipType Type = new();
        public readonly ParsedInt4 RawInts = new( "Raw Integers" );
        public readonly ParsedFloat4 RawFloats = new( "Raw Floats" );

        public AvfxTimelineClip( AvfxTimeline timeline ) : base( "Clip" ) {
            Timeline = timeline;
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            Type.Read( reader );
            RawInts.Read( reader );
            RawFloats.Read( reader );
            reader.ReadBytes( 4 * 32 );
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            Type.Write( writer );
            RawInts.Write( writer );
            RawFloats.Write( writer );
            WritePad( writer, 4 * 32 );
        }

        public override void Draw() {
            using var _ = ImRaii.PushId( "Clip" );
            DrawRename();

            Type.Draw();

            if( Type.Value == "LLIK" ) DrawKill();
            else if( Type.Value == "GRTR" ) DrawRandomTrigger();

            RawInts.Draw( CommandManager.Avfx );
            RawFloats.Draw( CommandManager.Avfx );
        }

        public override string GetDefaultText() => $"Clip {GetIdx()}({Type.Text})";

        public override string GetWorkspaceId() => $"{Timeline.GetWorkspaceId()}/Clip{GetIdx()}";

        private void DrawKill() {
            var duration = RawInts.Value.X;
            if( ImGui.InputInt( "Fade Out Duration", ref duration ) ) {
                CommandManager.Avfx.Add( new ParsedSimpleCommand<Int4>( RawInts, RawInts.Value with {
                    X = duration
                } ) );
            }

            var hide = RawInts.Value.W == 1;
            if( ImGui.Checkbox( "Hide", ref hide ) ) {
                CommandManager.Avfx.Add( new ParsedSimpleCommand<Int4>( RawInts, RawInts.Value with {
                    W = hide ? 1 : 0
                } ) );
            }

            var allowShow = RawFloats.Value.X != -1f;
            if( ImGui.Checkbox( "Allow Show", ref allowShow ) ) {
                CommandManager.Avfx.Add( new ParsedSimpleCommand<Vector4>( RawFloats, RawFloats.Value with {
                    X = allowShow ? 0 : -1f
                } ) );
            }

            var startHidden = RawFloats.Value.Y != -1f;
            if( ImGui.Checkbox( "Start Hidden", ref startHidden ) ) {
                CommandManager.Avfx.Add( new ParsedSimpleCommand<Vector4>( RawFloats, RawFloats.Value with {
                    Y = startHidden ? 0 : -1f
                } ) );
            }
        }

        private void DrawRandomTrigger() {
            var min = RawInts.Value.X;
            if( ImGui.InputInt( "Minimum Trigger", ref min ) ) {
                CommandManager.Avfx.Add( new ParsedSimpleCommand<Int4>( RawInts, RawInts.Value with {
                    X = min
                } ) );
            }

            var max = RawInts.Value.Y;
            if( ImGui.InputInt( "Maximum Trigger", ref max ) ) {
                CommandManager.Avfx.Add( new ParsedSimpleCommand<Int4>( RawInts, RawInts.Value with {
                    Y = max
                } ) );
            }
        }
    }
}
