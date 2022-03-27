using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace VFXEditor.Avfx.Vfx {
    public class UITimelineClip : UIWorkspaceItem {
        public AVFXTimelineClip Clip;
        public UITimeline Timeline;
        public string Type;
        public Vector4 RawInts;
        public Vector4 RawFloats;

        public UITimelineClip( AVFXTimelineClip clip, UITimeline timeline ) {
            Clip = clip;
            Timeline = timeline;
            Type = Clip.UniqueId;
            RawInts = new Vector4( Clip.UnknownInts[0], Clip.UnknownFloats[1], Clip.UnknownInts[2], Clip.UnknownInts[3] );
            RawFloats = new Vector4( Clip.UnknownFloats[0], Clip.UnknownFloats[1], Clip.UnknownFloats[2], Clip.UnknownFloats[3] );
        }

        public static Dictionary<string, string> IdOptions = new()
        {
            { "LLIK", "Kill" },
            { "TSER", "Reset" },
            { " DNE", "End" },
            { "IDAF", "Fade In" },
            { "PLLU", "Unlock Loop Point" },
            { " GRT", "Trigger" },
            { "GRTR", "Random Trigger" }
        };

        public override void DrawBody( string parentId ) {
            var id = parentId + "/Clip";
            DrawRename( id );

            if( ImGui.BeginCombo( "Type" + id, IdOptions[Type] ) ) {
                foreach( var key in IdOptions.Keys ) {
                    if( ImGui.Selectable( IdOptions[key], Type == key ) ) {
                        Type = key;
                        Clip.UniqueId = key;
                    }
                }
                ImGui.EndCombo();
            }

            if( Type == "LLIK" ) {
                var duration = RawInts.X;
                if( ImGui.InputFloat( "Fade Out Duration" + id, ref duration ) ) {
                    RawInts.X = duration;
                    Clip.UnknownInts[0] = ( int )RawInts.X;
                }

                var hide = RawInts.W == 1.0f;
                if( ImGui.Checkbox( "Hide" + id, ref hide ) ) {
                    RawInts.W = hide ? 1.0f : 0.0f;
                    Clip.UnknownInts[3] = ( int )RawInts.W;
                }

                var allowShow = RawFloats.X != -1f;
                if( ImGui.Checkbox( "Allow Show" + id, ref allowShow ) ) {
                    RawFloats.X = allowShow ? 0f : -1f;
                    Clip.UnknownFloats[0] = RawFloats.X;
                }

                var startHidden = RawFloats.Y != -1f;
                if( ImGui.Checkbox( "Start Hidden" + id, ref startHidden ) ) {
                    RawFloats.Y = startHidden ? 0f : -1f;
                    Clip.UnknownFloats[1] = RawFloats.Y;
                }
            }

            if( ImGui.InputFloat4( "Raw Integers" + id, ref RawInts ) ) {
                Clip.UnknownInts[0] = ( int )RawInts.X;
                Clip.UnknownInts[1] = ( int )RawInts.Y;
                Clip.UnknownInts[2] = ( int )RawInts.Z;
                Clip.UnknownInts[3] = ( int )RawInts.W;
            }
            if( ImGui.InputFloat4( "Raw Floats" + id, ref RawFloats ) ) {
                Clip.UnknownFloats[0] = RawFloats.X;
                Clip.UnknownFloats[1] = RawFloats.Y;
                Clip.UnknownFloats[2] = RawFloats.Z;
                Clip.UnknownFloats[3] = RawFloats.W;
            }
        }

        public override string GetDefaultText() => $"{Idx}: {IdOptions[Clip.UniqueId]}";

        public override string GetWorkspaceId() => $"{Timeline.GetWorkspaceId()}/Clip{Idx}";
    }
}
