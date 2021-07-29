using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UITimeline : UINode {
        public AVFXTimeline Timeline;
        public UIMain Main;
        //=====================
        public List<UITimelineItem> Items;
        //=====================
        public List<UITimelineClip> Clips;
        //=====================
        public UITimelineClipSplitView ClipSplit;
        public UITimelineItemSequencer ItemSplit;

        public UINodeSelect<UIBinder> BinderSelect;

        public UITimeline(UIMain main, AVFXTimeline timeline, bool has_dependencies = false ) : base( UINodeGroup.TimelineColor, has_dependencies ) {
            Timeline = timeline;
            Main = main;
            BinderSelect = new UINodeSelect<UIBinder>( this, "Binder Select", Main.Binders, Timeline.BinderIdx );
            //===============
            Items = new List<UITimelineItem>();
            Clips = new List<UITimelineClip>();
            //========================
            Attributes.Add( new UIInt( "Loop Start", Timeline.LoopStart ) );
            Attributes.Add( new UIInt( "Loop End", Timeline.LoopEnd ) );
            //========================
            foreach( var item in Timeline.Items ) {
                Items.Add( new UITimelineItem( item, this ) );
            }
            //==========================
            foreach( var clip in Timeline.Clips ) {
                Clips.Add( new UITimelineClip( clip, this ) );
            }
            //==========================
            ClipSplit = new UITimelineClipSplitView( Clips, this );
            ItemSplit = new UITimelineItemSequencer( Items, this );

            HasDependencies = false; // if imported, all set now
        }

        public override void DrawBody( string parentId ) {
            var id = parentId + "/Timeline";
            DrawRename( id );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            //=====================
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
            DrawAttrs( id );
            ImGui.EndChild();
        }

        public override string GetDefaultText() {
            return "Timeline " + Idx;
        }

        public override string GetWorkspaceId() {
            return $"Tmln{Idx}";
        }

        public override void PopulateWorkspaceMetaChildren( Dictionary<string, string> RenameDict ) {
            Items.ForEach( item => item.PopulateWorkspaceMeta( RenameDict ) );
            Clips.ForEach( item => item.PopulateWorkspaceMeta( RenameDict ) );
        }

        public override void ReadWorkspaceMetaChildren( Dictionary<string, string> RenameDict ) {
            Items.ForEach( item => item.ReadWorkspaceMetaChildren( RenameDict ) );
            Clips.ForEach( item => item.ReadWorkspaceMetaChildren( RenameDict ) );
        }

        public override byte[] ToBytes() {
            return Timeline.ToAVFX().ToBytes();
        }
    }
}
