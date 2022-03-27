using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;

namespace VFXEditor.Avfx.Vfx {
    public class UITimeline : UINode {
        public AVFXTimeline Timeline;
        public AvfxFile Main;
        public List<UITimelineItem> Items;
        public List<UITimelineClip> Clips;
        public UITimelineClipSplitView ClipSplit;
        public UITimelineItemSequencer ItemSplit;
        public UINodeSelect<UIBinder> BinderSelect;
        private readonly List<UIBase> Parameters;

        public UITimeline( AvfxFile main, AVFXTimeline timeline, bool has_dependencies = false ) : base( UINodeGroup.TimelineColor, has_dependencies ) {
            Timeline = timeline;
            Main = main;
            BinderSelect = new UINodeSelect<UIBinder>( this, "Binder Select", Main.Binders, Timeline.BinderIdx );

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

        public override byte[] ToBytes() => Timeline.ToAVFX().ToBytes();
    }
}
