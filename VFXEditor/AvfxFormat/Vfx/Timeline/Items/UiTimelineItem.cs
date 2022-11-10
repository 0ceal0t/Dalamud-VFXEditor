using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.AVFXLib.Timeline;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiTimelineItem : UiWorkspaceItem {
        public readonly AVFXTimelineSubItem Item;
        public readonly UiTimeline Timeline;
        public bool ClipAssigned;
        public readonly UiInt ClipNumber;
        public readonly UiCheckbox Enabled;
        public readonly UiNodeSelect<UiBinder> BinderSelect;
        public readonly UiNodeSelect<UiEmitter> EmitterSelect;
        public readonly UiNodeSelect<UiEffector> EffectorSelect;
        public readonly UiInt StartTime;
        public readonly UiInt EndTime;
        private readonly List<IUiBase> Parameters;

        public UiTimelineItem( AVFXTimelineSubItem item, UiTimeline timeline ) {
            Item = item;
            Timeline = timeline;

            BinderSelect = new UiNodeSelect<UiBinder>( timeline, "Binder Select", Timeline.NodeGroups.Binders, Item.BinderIdx );
            EmitterSelect = new UiNodeSelect<UiEmitter>( timeline, "Emitter Select", Timeline.NodeGroups.Emitters, Item.EmitterIdx );
            EffectorSelect = new UiNodeSelect<UiEffector>( timeline, "Effector Select", Timeline.NodeGroups.Effectors, Item.EffectorIdx );

            ClipNumber = new UiInt( "Clip Index", Item.ClipNumber );
            ClipAssigned = Item.ClipNumber.IsAssigned();

            Parameters = new List<IUiBase> {
                ( Enabled = new UiCheckbox( "Enabled", Item.Enabled ) ),
                ( StartTime = new UiInt( "Start Time", Item.StartTime ) ),
                ( EndTime = new UiInt( "End Time", Item.EndTime ) ),
                new UiInt( "Platform", Item.Platform )
            };
        }

        public override void DrawInline( string parentId ) {
            var id = parentId + "/Item";
            DrawRename( id );

            BinderSelect.DrawInline( id );
            EmitterSelect.DrawInline( id );
            EffectorSelect.DrawInline( id );
            IUiBase.DrawList( Parameters, id );

            if( ImGui.Checkbox( "Clip Enabled" + id, ref ClipAssigned ) ) CommandManager.Avfx.Add( new UiAssignableCommand( Item.ClipNumber, ClipAssigned ) );
            ClipNumber.DrawInline( id );
        }

        public override string GetDefaultText() => $"{Idx}: Emitter {Item.EmitterIdx.GetValue()}";

        public override string GetWorkspaceId() => $"{Timeline.GetWorkspaceId()}/Item{Idx}";
    }
}
