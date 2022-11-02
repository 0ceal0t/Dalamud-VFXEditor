using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.AVFXLib.Timeline;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiTimeline : UiNode {
        public readonly AVFXTimeline Timeline;
        public readonly UiNodeGroupSet NodeGroups;
        public readonly List<UiTimelineItem> Items;
        public readonly List<UiTimelineClip> Clips;
        public readonly UiTimelineClipSplitView ClipSplit;
        public readonly UiTimelineItemSequencer ItemSplit;
        public readonly UiNodeSelect<UiBinder> BinderSelect;
        private readonly List<IUiBase> Parameters;

        public UiTimeline( AVFXTimeline timeline, UiNodeGroupSet nodeGroups, bool has_dependencies = false ) : base( UiNodeGroup.TimelineColor, has_dependencies ) {
            Timeline = timeline;
            NodeGroups = nodeGroups;
            BinderSelect = new UiNodeSelect<UiBinder>( this, "Binder Select", nodeGroups.Binders, Timeline.BinderIdx );

            Items = new List<UiTimelineItem>();
            Clips = new List<UiTimelineClip>();

            Parameters = new List<IUiBase> {
                new UiInt( "Loop Start", Timeline.LoopStart ),
                new UiInt( "Loop End", Timeline.LoopEnd )
            };

            foreach( var item in Timeline.Items ) {
                Items.Add( new UiTimelineItem( item, this ) );
            }

            foreach( var clip in Timeline.Clips ) {
                Clips.Add( new UiTimelineClip( clip, this ) );
            }

            ClipSplit = new UiTimelineClipSplitView( Clips, this );
            ItemSplit = new UiTimelineItemSequencer( Items, this );

            HasDependencies = false; // if imported, all set now
        }

        public override void DrawInline( string parentId ) {
            var id = parentId + "/Timeline";
            DrawRename( id );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( ImGui.BeginTabBar( id + "/Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) ) {
                if( ImGui.BeginTabItem( "Parameters" + id ) ) {
                    DrawParameters( id + "/Params" );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Items" + id ) ) {
                    ItemSplit.DrawInline( id + "/Items" );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Clips" + id ) ) {
                    ClipSplit.DrawInline( id + "/Clips" );
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
        }

        private void DrawParameters( string id ) {
            ImGui.BeginChild( id );
            BinderSelect.DrawInline( id );
            IUiBase.DrawList( Parameters, id );
            ImGui.EndChild();
        }

        public override void PopulateWorkspaceMetaChildren( Dictionary<string, string> RenameDict ) {
            Items.ForEach( item => item.PopulateWorkspaceMeta( RenameDict ) );
            Clips.ForEach( item => item.PopulateWorkspaceMeta( RenameDict ) );
        }

        public override void ReadWorkspaceMetaChildren( Dictionary<string, string> RenameDict ) {
            Items.ForEach( item => item.ReadWorkspaceMetaChildren( RenameDict ) );
            Clips.ForEach( item => item.ReadWorkspaceMetaChildren( RenameDict ) );
        }

        public override string GetDefaultText() => $"Timeline {Idx}";

        public override string GetWorkspaceId() => $"Tmln{Idx}";

        public override void Write( BinaryWriter writer ) => Timeline.Write( writer );
    }
}
