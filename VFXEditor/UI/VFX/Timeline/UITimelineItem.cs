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
    public class UITimelineItem : UIBase
    {
        public AVFXTimelineSubItem Item;
        public UITimeline Timeline;
        //===========================
        public UIInt ClipNumber;
        public bool ClipAssigned;

        public UITimelineItem(AVFXTimelineSubItem item, UITimeline timeline)
        {
            Item = item;
            Timeline = timeline;
            Init();
        }
        public override void Init()
        {
            base.Init();
            //==================
            Attributes.Add(new UICheckbox("Enabled", Item.Enabled));
            Attributes.Add(new UIInt("Start Time", Item.StartTime));
            Attributes.Add(new UIInt("End Time", Item.EndTime));
            Attributes.Add(new UIInt("Binder Index", Item.BinderIdx));
            Attributes.Add(new UIInt("Effector Index", Item.EffectorIdx));
            Attributes.Add(new UIInt("Emitter Index", Item.EmitterIdx));
            Attributes.Add(new UIInt("Platform", Item.Platform));
            ClipNumber = new UIInt("ClipNumber", Item.ClipNumber);
            ClipAssigned = Item.ClipNumber.Assigned;
        }

        public override void Draw( string parentId )
        {
        }
        public override void DrawSelect( string parentId, ref UIBase selected )
        {
            if( !Assigned )
            {
                return;
            }
            string text = Idx + ": Emitter " + Item.EmitterIdx.Value;
            if( ImGui.Selectable( text + parentId, selected == this ) )
            {
                selected = this;
            }
        }
        public override void DrawBody( string parentId )
        {
            string id = parentId + "/Item" + Idx;
            if( UIUtils.RemoveButton( "Delete" + id, small: true ) )
            {
                Timeline.Timeline.removeItem( Idx );
                Timeline.Init();
                return;
            }
            DrawAttrs( id );

            if(ImGui.Checkbox("Clip Enabled" + id, ref ClipAssigned ) ) {
                Item.ClipNumber.Assigned = ClipAssigned;
            }
            ClipNumber.Draw( id );
        }
    }
}
