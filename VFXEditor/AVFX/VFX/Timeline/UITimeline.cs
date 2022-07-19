using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VFXEditor.AVFXLib.Timeline;

namespace VFXEditor.AVFX.VFX {
    public class UITimeline : UINode {
        public readonly AVFXTimeline Timeline;
        public readonly UINodeGroupSet NodeGroups;
        public readonly List<UITimelineItem> Items;
        public readonly List<UITimelineClip> Clips;
        public readonly UITimelineClipSplitView ClipSplit;
        public readonly UITimelineItemSequencer ItemSplit;
        public readonly UINodeSelect<UIBinder> BinderSelect;
        private readonly List<UIBase> Parameters;

        public UITimeline( AVFXTimeline timeline, UINodeGroupSet nodeGroups, bool has_dependencies = false ) : base( UINodeGroup.TimelineColor, has_dependencies ) {
            Timeline = timeline;
            NodeGroups = nodeGroups;
            BinderSelect = new UINodeSelect<UIBinder>( this, "Binder Select", nodeGroups.Binders, Timeline.BinderIdx );

            Items = new List<UITimelineItem>();
            Clips = new List<UITimelineClip>();

            Parameters = new List<UIBase> {
                new UIInt( "Loop Start", Timeline.LoopStart ),
                new UIInt( "Loop End", Timeline.LoopEnd )
            };

            foreach( var item in Timeline.Items ) {
                Items.Add( new UITimelineItem( item, this ) );
            }

            foreach( var clip in Timeline.Clips ) {
                Clips.Add( new UITimelineClip( clip, this ) );
            }

            ClipSplit = new UITimelineClipSplitView( Clips, this );
            ItemSplit = new UITimelineItemSequencer( Items, this );

            HasDependencies = false; // if imported, all set now
        }

        public override void DrawBody( string parentId ) {
            var id = parentId + "/Timeline";
            DrawRename( id );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( ImGui.BeginTabBar( id + "/Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) ) {
                if( ImGui.BeginTabItem( "Parameters" + id ) ) {
                    DrawParameters( id + "/Params" );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Items" + id ) ) {
                    ItemSplit.Draw( id + "/Items" );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Clips" + id ) ) {
                    ClipSplit.Draw( id + "/Clips" );
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
        }

        private void DrawParameters( string id ) {
            ImGui.BeginChild( id );
            BinderSelect.Draw( id );
            DrawList( Parameters, id );
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
