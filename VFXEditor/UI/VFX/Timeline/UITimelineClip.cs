using AVFXLib.Models;
using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UITimelineClip : UIItem
    {
        public AVFXTimelineClip Clip;
        public UITimeline Timeline;
        //===============================
        public string UniqueId;

        public Vector4 UnknownInts;
        public Vector4 UnknownFloats;

        public UITimelineClip(AVFXTimelineClip clip, UITimeline timeline)
        {
            Clip = clip;
            Timeline = timeline;
            //=====================
            UniqueId = Clip.UniqueId;
            UnknownInts = new Vector4( Clip.UnknownInts[0], Clip.UnknownFloats[1], Clip.UnknownInts[2], Clip.UnknownInts[3] );
            UnknownFloats = new Vector4( Clip.UnknownFloats[0], Clip.UnknownFloats[1], Clip.UnknownFloats[2], Clip.UnknownFloats[3] );
        }

        public static Dictionary<string, string> IdOptions = new Dictionary<string, string>()
        {
            { "LLIK", "Kill" },
            { "TSER", "Rest" },
            { " DNE", "End" },
            { "IDAF", "Fade In" },
            { "PLLU", "Unlock Loop Point" },
            { " GRT", "Trigger" },
            { "GRTR", "Random Trigger" }
        };
        public override void DrawBody( string parentId )
        {
            string id = parentId + "/Clip";
            if( UIUtils.RemoveButton( "Delete" + id, small: true ) )
            {
                Timeline.Timeline.removeClip( Clip );
                Timeline.ClipSplit.OnDelete( this );
                return;
            }
            if( ImGui.InputFloat4( "Unknown Ints" + id, ref UnknownInts ) )
            {
                Clip.UnknownInts[0] = ( int )UnknownInts.X;
                Clip.UnknownInts[1] = ( int )UnknownInts.Y;
                Clip.UnknownInts[2] = ( int )UnknownInts.Z;
                Clip.UnknownInts[3] = ( int )UnknownInts.W;
            }
            if( ImGui.InputFloat4( "Unknown Floats" + id, ref UnknownFloats ) )
            {
                Clip.UnknownFloats[0] = UnknownFloats.X;
                Clip.UnknownFloats[1] = UnknownFloats.Y;
                Clip.UnknownFloats[2] = UnknownFloats.Z;
                Clip.UnknownFloats[3] = UnknownFloats.W;
            }

            if( ImGui.BeginCombo( "Unique Id" + id, IdOptions[UniqueId] ) )
            {
                foreach( var key in IdOptions.Keys )
                {
                    if( ImGui.Selectable( IdOptions[key], UniqueId == key ) )
                    {
                        UniqueId = key;
                        Clip.UniqueId = key;
                    }
                }
                ImGui.EndCombo();
            }
        }

        public override void DrawSelect( string parentId, ref UIItem selected ) {
            if( ImGui.Selectable( GetText() + parentId, selected == this ) ) {
                selected = this;
            }
        }

        public override string GetText() {
            return Idx + ": " + IdOptions[Clip.UniqueId];
        }
    }
}
