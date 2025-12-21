using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Parsing;
using Int4 = SharpDX.Int4;

namespace VfxEditor.AvfxFormat {
    public enum ClipType {
        Kill = 0x4B_49_4C_4C, // "KILL"
        Reset = 0x52_45_53_54, // "REST"
        End = 0x45_4E_44_20, // "END "
        FadeIn = 0x46_41_44_49, // "FADI"
        UnlockLoopPoint = 0x55_4C_4C_50, // "ULLP"
        Trigger = 0x54_52_47_20, // "TRG "
        RandomTrigger = 0x52_54_52_47, // "RTRG"
    }

    public class AvfxTimelineClip : AvfxWorkspaceItem {
        public readonly AvfxTimeline Timeline;
        public readonly ParsedEnum<ClipType> Type = new( "Type", ClipType.Kill );
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

        public override void WriteContents( BinaryWriter writer ) {
            Type.Write( writer );
            RawInts.Write( writer );
            RawFloats.Write( writer );
            WritePad( writer, 4 * 32 );
        }

        protected override IEnumerable<AvfxBase> GetChildren() {
            yield break;
        }

        public override void Draw() {
            using var _ = ImRaii.PushId( "Clip" );
            DrawRename();

            Type.Draw();

            if( Type.Value == ClipType.Kill ) DrawKill();
            else if( Type.Value == ClipType.RandomTrigger ) DrawRandomTrigger();

            RawInts.Draw();
            RawFloats.Draw();
        }

        public override string GetDefaultText() => $"Clip {GetIdx()} ({Type.Value})";

        public override string GetWorkspaceId() => $"{Timeline.GetWorkspaceId()}/Clip{GetIdx()}";

        private void DrawKill() {
            var duration = RawInts.Value.X;
            if( ImGui.InputInt( "Fade Out Duration", ref duration ) ) {
                CommandManager.Add( new ParsedSimpleCommand<Int4>( RawInts, RawInts.Value with {
                    X = duration
                } ) );
            }

            var hide = RawInts.Value.W == 1;
            if( ImGui.Checkbox( "Hide", ref hide ) ) {
                CommandManager.Add( new ParsedSimpleCommand<Int4>( RawInts, RawInts.Value with {
                    W = hide ? 1 : 0
                } ) );
            }

            var allowShow = RawFloats.Value.X != -1f;
            if( ImGui.Checkbox( "Allow Show", ref allowShow ) ) {
                CommandManager.Add( new ParsedSimpleCommand<Vector4>( RawFloats, RawFloats.Value with {
                    X = allowShow ? 0 : -1f
                } ) );
            }

            var startHidden = RawFloats.Value.Y != -1f;
            if( ImGui.Checkbox( "Start Hidden", ref startHidden ) ) {
                CommandManager.Add( new ParsedSimpleCommand<Vector4>( RawFloats, RawFloats.Value with {
                    Y = startHidden ? 0 : -1f
                } ) );
            }
        }

        private void DrawRandomTrigger() {
            var min = RawInts.Value.X;
            if( ImGui.InputInt( "Minimum Trigger", ref min ) ) {
                CommandManager.Add( new ParsedSimpleCommand<Int4>( RawInts, RawInts.Value with {
                    X = min
                } ) );
            }

            var max = RawInts.Value.Y;
            if( ImGui.InputInt( "Maximum Trigger", ref max ) ) {
                CommandManager.Add( new ParsedSimpleCommand<Int4>( RawInts, RawInts.Value with {
                    Y = max
                } ) );
            }
        }
    }
}
