using Dalamud.Logging;
using ImGuiNET;
using System;
using System.Collections.Generic;
using VFXEditor.AVFXLib.Timeline;

namespace VFXEditor.Avfx.Vfx {
    public class UITimelineItem : UIWorkspaceItem {
        public AVFXTimelineSubItem Item;
        public UITimeline Timeline;
        public bool ClipAssigned;
        public UIInt ClipNumber;
        public UINodeSelect<UIBinder> BinderSelect;
        public UINodeSelect<UIEmitter> EmitterSelect;
        public UINodeSelect<UIEffector> EffectorSelect;
        public UIInt StartTime;
        public UIInt EndTime;
        private readonly List<UIBase> Parameters;

        public UITimelineItem( AVFXTimelineSubItem item, UITimeline timeline ) {
            Item = item;
            Timeline = timeline;

            BinderSelect = new UINodeSelect<UIBinder>( timeline, "Binder Select", Timeline.Main.Binders, Item.BinderIdx );
            EmitterSelect = new UINodeSelect<UIEmitter>( timeline, "Emitter Select", Timeline.Main.Emitters, Item.EmitterIdx );
            EffectorSelect = new UINodeSelect<UIEffector>( timeline, "Effector Select", Timeline.Main.Effectors, Item.EffectorIdx );

            ClipNumber = new UIInt( "Clip Index", Item.ClipNumber );
            ClipAssigned = Item.ClipNumber.IsAssigned();

            Parameters = new List<UIBase> {
                new UICheckbox( "Enabled", Item.Enabled ),
                ( StartTime = new UIInt( "Start Time", Item.StartTime ) ),
                ( EndTime = new UIInt( "End Time", Item.EndTime ) ),
                new UIInt( "Platform", Item.Platform )
            };
        }

        public override void DrawBody( string parentId ) {
            var id = parentId + "/Item";
            DrawRename( id );

            BinderSelect.Draw( id );
            EmitterSelect.Draw( id );
            EffectorSelect.Draw( id );
            DrawList( Parameters, id );

            if( ImGui.Checkbox( "Clip Enabled" + id, ref ClipAssigned ) ) {
                Item.ClipNumber.SetAssigned( ClipAssigned );
            }
            ClipNumber.Draw( id );
        }

        public override string GetDefaultText() => $"{Idx}: Emitter {Item.EmitterIdx.GetValue()}";

        public override string GetWorkspaceId() => $"{Timeline.GetWorkspaceId()}/Item{Idx}";
    }
}
