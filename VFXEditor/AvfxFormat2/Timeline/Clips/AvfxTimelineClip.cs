using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxTimelineClip : AvfxWorkspaceItem {
        private static readonly Dictionary<string, string> IdOptions = new() {
            { "LLIK", "Kill" },
            { "TSER", "Reset" },
            { " DNE", "End" },
            { "IDAF", "Fade In" },
            { "PLLU", "Unlock Loop Point" },
            { " GRT", "Trigger" },
            { "GRTR", "Random Trigger" }
        };

        public readonly AvfxTimeline Timeline;

        public string UniqueId = "LLIK";
        public readonly int[] UnknownInts = new int[] { 0, 0, 0, 0 };
        public readonly float[] UnknownFloats = new float[] { -1, 0, 0, 0 };

        public AvfxTimelineClip( AvfxTimeline timeline ) : base( "Clip" ) {
            Timeline = timeline;
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            UniqueId = Encoding.ASCII.GetString( reader.ReadBytes( 4 ) );
            for( var i = 0; i < 4; i++ ) UnknownInts[i] = reader.ReadInt32();
            for( var i = 0; i < 4; i++ ) UnknownFloats[i] = reader.ReadSingle();
            reader.ReadBytes( 4 * 32 );
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            writer.Write( Encoding.ASCII.GetBytes( UniqueId ) );
            for( var i = 0; i < 4; i++ ) writer.Write( UnknownInts[i] );
            for( var i = 0; i < 4; i++ ) writer.Write( UnknownFloats[i] );
            WritePad( writer, 4 * 32 );
        }

        public override void Draw( string parentId ) {
            var id = parentId + "/Clip";
            DrawRename( id );

            var type = UniqueId;
            if( ImGui.BeginCombo( "Type" + id, IdOptions[type] ) ) {
                foreach( var key in IdOptions.Keys ) {
                    if( ImGui.Selectable( IdOptions[key], type == key ) ) {
                        CommandManager.Avfx.Add( new AvfxTimelineClipCommand( this, () => {
                            UniqueId = key;
                        } ) );
                    }
                }
                ImGui.EndCombo();
            }

            if( type == "LLIK" ) {
                var duration = UnknownInts[0];
                if( ImGui.InputInt( "Fade Out Duration" + id, ref duration ) ) {
                    CommandManager.Avfx.Add( new AvfxTimelineClipCommand( this, () => {
                        UnknownInts[0] = duration;
                    } ) );
                }

                var hide = UnknownInts[3] == 1;
                if( ImGui.Checkbox( "Hide" + id, ref hide ) ) {
                    CommandManager.Avfx.Add( new AvfxTimelineClipCommand( this, () => {
                        UnknownInts[3] = hide ? 1 : 0;
                    } ) );
                }

                var allowShow = UnknownFloats[0] != -1f;
                if( ImGui.Checkbox( "Allow Show" + id, ref allowShow ) ) {
                    CommandManager.Avfx.Add( new AvfxTimelineClipCommand( this, () => {
                        UnknownFloats[0] = allowShow ? 0f : -1f;
                    } ) );
                }

                var startHidden = UnknownFloats[1] != -1f;
                if( ImGui.Checkbox( "Start Hidden" + id, ref startHidden ) ) {
                    CommandManager.Avfx.Add( new AvfxTimelineClipCommand( this, () => {
                        UnknownFloats[1] = startHidden ? 0f : -1f;
                    } ) );
                }
            }

            var rawInts = new Vector4( UnknownInts[0], UnknownInts[1], UnknownInts[2], UnknownInts[3] );
            if( ImGui.InputFloat4( "Raw Integers" + id, ref rawInts ) ) {
                CommandManager.Avfx.Add( new AvfxTimelineClipCommand( this, () => {
                    UnknownInts[0] = ( int )rawInts.X;
                    UnknownInts[1] = ( int )rawInts.Y;
                    UnknownInts[2] = ( int )rawInts.Z;
                    UnknownInts[3] = ( int )rawInts.W;
                } ) );
            }

            var rawFloats = new Vector4( UnknownFloats[0], UnknownFloats[1], UnknownFloats[2], UnknownFloats[3] );
            if( ImGui.InputFloat4( "Raw Floats" + id, ref rawFloats ) ) {
                CommandManager.Avfx.Add( new AvfxTimelineClipCommand( this, () => {
                    UnknownFloats[0] = rawFloats.X;
                    UnknownFloats[1] = rawFloats.Y;
                    UnknownFloats[2] = rawFloats.Z;
                    UnknownFloats[3] = rawFloats.W;
                } ) );
            }
        }

        public override string GetDefaultText() => $"{GetIdx()}: {IdOptions[UniqueId]}";

        public override string GetWorkspaceId() => $"{Timeline.GetWorkspaceId()}/Clip{GetIdx()}";

        public AvfxTimelineClipState GetState() => new() {
            Type = UniqueId,
            Unk1 = UnknownInts[0],
            Unk2 = UnknownInts[1],
            Unk3 = UnknownInts[2],
            Unk4 = UnknownInts[3],
            Unk5 = UnknownFloats[0],
            Unk6 = UnknownFloats[1],
            Unk7 = UnknownFloats[2],
            Unk8 = UnknownFloats[3]
        };

        public void SetState( AvfxTimelineClipState state ) {
            UniqueId = state.Type;
            UnknownInts[0] = state.Unk1;
            UnknownInts[1] = state.Unk2;
            UnknownInts[2] = state.Unk3;
            UnknownInts[3] = state.Unk4;
            UnknownFloats[0] = state.Unk5;
            UnknownFloats[1] = state.Unk6;
            UnknownFloats[2] = state.Unk7;
            UnknownFloats[3] = state.Unk8;
        }
    }
}
