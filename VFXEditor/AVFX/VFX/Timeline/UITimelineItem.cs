using ImGuiNET;
using System.Collections.Generic;
using VFXEditor.AVFXLib.Timeline;

namespace VFXEditor.AVFX.VFX {
    public class UITimelineItem : UIWorkspaceItem {
        public readonly AVFXTimelineSubItem Item;
        public readonly UITimeline Timeline;
        public bool ClipAssigned;
        public readonly UIInt ClipNumber;
        public readonly UICheckbox Enabled;
        public readonly UINodeSelect<UIBinder> BinderSelect;
        public readonly UINodeSelect<UIEmitter> EmitterSelect;
        public readonly UINodeSelect<UIEffector> EffectorSelect;
        public readonly UIInt StartTime;
        public readonly UIInt EndTime;
        private readonly List<IUIBase> Parameters;

        public UITimelineItem( AVFXTimelineSubItem item, UITimeline timeline ) {
            Item = item;
            Timeline = timeline;

            BinderSelect = new UINodeSelect<UIBinder>( timeline, "Binder Select", Timeline.NodeGroups.Binders, Item.BinderIdx );
            EmitterSelect = new UINodeSelect<UIEmitter>( timeline, "Emitter Select", Timeline.NodeGroups.Emitters, Item.EmitterIdx );
            EffectorSelect = new UINodeSelect<UIEffector>( timeline, "Effector Select", Timeline.NodeGroups.Effectors, Item.EffectorIdx );

            ClipNumber = new UIInt( "Clip Index", Item.ClipNumber );
            ClipAssigned = Item.ClipNumber.IsAssigned();

            Parameters = new List<IUIBase> {
                ( Enabled = new UICheckbox( "Enabled", Item.Enabled ) ),
                ( StartTime = new UIInt( "Start Time", Item.StartTime ) ),
                ( EndTime = new UIInt( "End Time", Item.EndTime ) ),
                new UIInt( "Platform", Item.Platform )
            };
        }

        public override void DrawInline( string parentId ) {
            var id = parentId + "/Item";
            DrawRename( id );

            BinderSelect.DrawInline( id );
            EmitterSelect.DrawInline( id );
            EffectorSelect.DrawInline( id );
            IUIBase.DrawList( Parameters, id );

            if( ImGui.Checkbox( "Clip Enabled" + id, ref ClipAssigned ) ) {
                Item.ClipNumber.SetAssigned( ClipAssigned );
            }
            ClipNumber.DrawInline( id );
        }

        public override string GetDefaultText() => $"{Idx}: Emitter {Item.EmitterIdx.GetValue()}";

        public override string GetWorkspaceId() => $"{Timeline.GetWorkspaceId()}/Item{Idx}";
    }
}
