using AVFXLib.Models;
using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UITimelineItem : UIItem
    {
        public AVFXTimelineSubItem Item;
        public UITimeline Timeline;
        //===========================
        public UIInt BinderIndex;
        public bool ClipAssigned;
        public UIInt ClipNumber;

        public UITimelineItem(AVFXTimelineSubItem item, UITimeline timeline)
        {
            Item = item;
            Timeline = timeline;
            BinderIndex = new UIInt( "Binder Index", Item.BinderIdx );
            ClipNumber = new UIInt( "ClipNumber", Item.ClipNumber );
            ClipAssigned = Item.ClipNumber.Assigned;
            //==================
            Attributes.Add( new UICheckbox( "Enabled", Item.Enabled ) );
            Attributes.Add( new UIInt( "Start Time", Item.StartTime ) );
            Attributes.Add( new UIInt( "End Time", Item.EndTime ) );
            Attributes.Add( BinderIndex );
            Attributes.Add( new UIInt( "Effector Index", Item.EffectorIdx ) );
            Attributes.Add( new UIInt( "Emitter Index", Item.EmitterIdx ) );
            Attributes.Add( new UIInt( "Platform", Item.Platform ) );
        }

        public override void DrawBody( string parentId )
        {
            string id = parentId + "/Item";
            if( UIUtils.RemoveButton( "Delete" + id, small: true ) )
            {
                Timeline.Timeline.removeItem( Item );
                Timeline.ItemSplit.OnDelete( this );
                return;
            }
            DrawAttrs( id );

            if(ImGui.Checkbox("Clip Enabled" + id, ref ClipAssigned ) ) {
                Item.ClipNumber.Assigned = ClipAssigned;
            }
            ClipNumber.Draw( id );
        }

        public override string GetText() {
            return Idx + ": Emitter " + Item.EmitterIdx.Value;
        }
    }
}
