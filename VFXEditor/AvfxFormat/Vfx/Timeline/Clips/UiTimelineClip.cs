using ImGuiNET;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.AVFXLib.Timeline;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiTimelineClip : UiWorkspaceItem {
        private static readonly Dictionary<string, string> IdOptions = new() {
            { "LLIK", "Kill" },
            { "TSER", "Reset" },
            { " DNE", "End" },
            { "IDAF", "Fade In" },
            { "PLLU", "Unlock Loop Point" },
            { " GRT", "Trigger" },
            { "GRTR", "Random Trigger" }
        };

        public AVFXTimelineClip Clip;
        public UiTimeline Timeline;

        public UiTimelineClip( AVFXTimelineClip clip, UiTimeline timeline ) {
            Clip = clip;
            Timeline = timeline;
        }

        public override void DrawInline( string parentId ) {
            var id = parentId + "/Clip";
            DrawRename( id );

            var type = Clip.UniqueId;
            if( ImGui.BeginCombo( "Type" + id, IdOptions[type] ) ) {
                foreach( var key in IdOptions.Keys ) {
                    if( ImGui.Selectable( IdOptions[key], type == key ) ) {
                        CommandManager.Avfx.Add( new UiTimelineClipCommand( this, () => {
                            Clip.UniqueId = key;
                        } ) );
                    }
                }
                ImGui.EndCombo();
            }

            if( type == "LLIK" ) {
                var duration = Clip.UnknownInts[0];
                if( ImGui.InputInt( "Fade Out Duration" + id, ref duration ) ) {
                    CommandManager.Avfx.Add( new UiTimelineClipCommand( this, () => {
                        Clip.UnknownInts[0] = duration;
                    } ) );
                }

                var hide = Clip.UnknownInts[3] == 1;
                if( ImGui.Checkbox( "Hide" + id, ref hide ) ) {
                    CommandManager.Avfx.Add( new UiTimelineClipCommand( this, () => {
                        Clip.UnknownInts[3] = hide ? 1 : 0;
                    } ) );
                }

                var allowShow = Clip.UnknownFloats[0] != -1f;
                if( ImGui.Checkbox( "Allow Show" + id, ref allowShow ) ) {
                    CommandManager.Avfx.Add( new UiTimelineClipCommand( this, () => {
                        Clip.UnknownFloats[0] = allowShow ? 0f : -1f;
                    } ) );
                }

                var startHidden = Clip.UnknownFloats[1] != -1f;
                if( ImGui.Checkbox( "Start Hidden" + id, ref startHidden ) ) {
                    CommandManager.Avfx.Add( new UiTimelineClipCommand( this, () => {
                        Clip.UnknownFloats[1] = startHidden ? 0f : -1f;
                    } ) );
                }
            }

            var rawInts = new Vector4( Clip.UnknownInts[0], Clip.UnknownInts[1], Clip.UnknownInts[2], Clip.UnknownInts[3] );
            if( ImGui.InputFloat4( "Raw Integers" + id, ref rawInts ) ) {
                CommandManager.Avfx.Add( new UiTimelineClipCommand( this, () => {
                    Clip.UnknownInts[0] = ( int )rawInts.X;
                    Clip.UnknownInts[1] = ( int )rawInts.Y;
                    Clip.UnknownInts[2] = ( int )rawInts.Z;
                    Clip.UnknownInts[3] = ( int )rawInts.W;
                } ) );
            }

            var rawFloats = new Vector4( Clip.UnknownFloats[0], Clip.UnknownFloats[1], Clip.UnknownFloats[2], Clip.UnknownFloats[3] );
            if( ImGui.InputFloat4( "Raw Floats" + id, ref rawFloats ) ) {
                CommandManager.Avfx.Add( new UiTimelineClipCommand( this, () => {
                    Clip.UnknownFloats[0] = rawFloats.X;
                    Clip.UnknownFloats[1] = rawFloats.Y;
                    Clip.UnknownFloats[2] = rawFloats.Z;
                    Clip.UnknownFloats[3] = rawFloats.W;
                } ) );
            }
        }

        public override string GetDefaultText() => $"{Idx}: {IdOptions[Clip.UniqueId]}";

        public override string GetWorkspaceId() => $"{Timeline.GetWorkspaceId()}/Clip{Idx}";

        public UiTimelineClipState GetState() => new() {
            Type = Clip.UniqueId,
            Unk1 = Clip.UnknownInts[0],
            Unk2 = Clip.UnknownInts[1],
            Unk3 = Clip.UnknownInts[2],
            Unk4 = Clip.UnknownInts[3],
            Unk5 = Clip.UnknownFloats[0],
            Unk6 = Clip.UnknownFloats[1],
            Unk7 = Clip.UnknownFloats[2],
            Unk8 = Clip.UnknownFloats[3]
        };

        public void SetState(UiTimelineClipState state ) {
            Clip.UniqueId = state.Type;
            Clip.UnknownInts[0] = state.Unk1;
            Clip.UnknownInts[1] = state.Unk2;
            Clip.UnknownInts[2] = state.Unk3;
            Clip.UnknownInts[3] = state.Unk4;
            Clip.UnknownFloats[0] = state.Unk5;
            Clip.UnknownFloats[1] = state.Unk6;
            Clip.UnknownFloats[2] = state.Unk7;
            Clip.UnknownFloats[3] = state.Unk8;
        }
    }
}
