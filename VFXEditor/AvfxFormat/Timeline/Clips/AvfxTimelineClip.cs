using ImGuiNET;
using OtterGui.Raii;
using System;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.AvfxFormat {
    public class AvfxTimelineClip : AvfxWorkspaceItem {
        public readonly AvfxTimeline Timeline;

        public readonly AvfxTimelineClipType Type = new();

        public readonly ParsedInt Unk1 = new( "Raw Int 1" );
        public readonly ParsedInt Unk2 = new( "Raw Int 2" );
        public readonly ParsedInt Unk3 = new( "Raw Int 3" );
        public readonly ParsedInt Unk4 = new( "Raw Int 4" );
        public readonly UiParsedInt4 RawInts;

        public readonly ParsedFloat Unk5 = new( "Raw Float 1", defaultValue: -1 );
        public readonly ParsedFloat Unk6 = new( "Raw Float 2" );
        public readonly ParsedFloat Unk7 = new( "Raw Float 3" );
        public readonly ParsedFloat Unk8 = new( "Raw Float 4" );
        public readonly UiParsedFloat4 RawFloats;

        public AvfxTimelineClip( AvfxTimeline timeline ) : base( "Clip" ) {
            Timeline = timeline;
            RawInts = new( "Raw Ints", Unk1, Unk2, Unk3, Unk4 );
            RawFloats = new( "Raw Floats", Unk5, Unk6, Unk7, Unk8 );
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            Type.Read( reader );
            Unk1.Read( reader );
            Unk2.Read( reader );
            Unk3.Read( reader );
            Unk4.Read( reader );
            Unk5.Read( reader );
            Unk6.Read( reader );
            Unk7.Read( reader );
            Unk8.Read( reader );
            reader.ReadBytes( 4 * 32 );
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            Type.Write( writer );
            Unk1.Write( writer );
            Unk2.Write( writer );
            Unk3.Write( writer );
            Unk4.Write( writer );
            Unk5.Write( writer );
            Unk6.Write( writer );
            Unk7.Write( writer );
            Unk8.Write( writer );
            WritePad( writer, 4 * 32 );
        }

        public override void Draw() {
            using var _ = ImRaii.PushId( "Clip" );
            DrawRename();

            Type.Draw();

            // ====== KILL ============

            if( Type.Value == "LLIK" ) {
                var duration = Unk1.Value;
                if( ImGui.InputInt( "Fade Out Duration", ref duration ) ) {
                    CommandManager.Avfx.Add( new ParsedSimpleCommand<int>( Unk1, duration ) );
                }

                var hide = Unk4.Value == 1;
                if( ImGui.Checkbox( "Hide", ref hide ) ) {
                    CommandManager.Avfx.Add( new ParsedSimpleCommand<int>( Unk4, hide ? 1 : 0 ) );
                }

                var allowShow = Unk5.Value != -1f;
                if( ImGui.Checkbox( "Allow Show", ref allowShow ) ) {
                    CommandManager.Avfx.Add( new ParsedSimpleCommand<float>( Unk5, allowShow ? 0 : -1f ) );
                }

                var startHidden = Unk6.Value != -1f;
                if( ImGui.Checkbox( "Start Hidden", ref startHidden ) ) {
                    CommandManager.Avfx.Add( new ParsedSimpleCommand<float>( Unk6, startHidden ? 0 : -1f ) );
                }
            }

            // ======================

            RawInts.Draw( CommandManager.Avfx );
            RawFloats.Draw( CommandManager.Avfx );
        }

        public override string GetDefaultText() => $"Clip {GetIdx()}({Type.Text})";

        public override string GetWorkspaceId() => $"{Timeline.GetWorkspaceId()}/Clip{GetIdx()}";
    }
}
