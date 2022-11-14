using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxTimeline : AvfxNode {
        public const string NAME = "TmLn";

        public readonly AvfxInt LoopStart = new( "Loop Start", "LpSt" );
        public readonly AvfxInt LoopEnd = new( "Loop End", "LpEd" );
        public readonly AvfxInt BinderIdx = new( "Binder Index", "BnNo" );
        public readonly AvfxInt TimelineCount = new( "Item Count", "TICn" );
        public readonly AvfxInt ClipCount = new( "Clip Count", "CpCn" );

        private readonly List<AvfxBase> Parsed;

        public readonly List<AvfxTimelineClip> Clips = new();
        public readonly List<AvfxTimelineSubItem> Items = new();

        public readonly UiNodeGroupSet NodeGroups;

        public readonly UiNodeSelect<AvfxBinder> BinderSelect;

        public readonly UiTimelineClipSplitView ClipSplit;
        public readonly UiTimelineItemSequencer ItemSplit;
        private readonly List<IUiBase> Display;

        public AvfxTimeline( UiNodeGroupSet groupSet, bool hasDependencies ) : base( NAME, UiNodeGroup.TimelineColor, hasDependencies ) {
            NodeGroups = groupSet;

            Parsed = new List<AvfxBase> {
                LoopStart,
                LoopEnd,
                BinderIdx,
                TimelineCount,
                ClipCount
            };

            Display = new() {
                LoopStart,
                LoopEnd
            };

            BinderSelect = new( this, "Binder Select", groupSet.Binders, BinderIdx );

            ClipSplit = new( Clips, this );
            ItemSplit = new( Items, this );
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            Peek( reader, Parsed, size );

            AvfxTimelineItem lastItem = null;

            ReadNested( reader, ( BinaryReader _reader, string _name, int _size ) => {
                if( _name == "Item" ) {
                    lastItem = new AvfxTimelineItem( this );
                    lastItem.Read( _reader, _size );
                }
                else if( _name == "Clip" ) {
                    var clip = new AvfxTimelineClip( this );
                    clip.Read( _reader, _size );
                    Clips.Add( clip );
                }
            }, size );

            if( lastItem != null ) {
                Items.AddRange( lastItem.Items );
                Items.ForEach( x => x.InitializeNodeSelects() );
            }

            ClipSplit.UpdateIdx();
            ItemSplit.UpdateIdx();

            DepedencyImportInProgress = false; // if imported, all set now
        }

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Parsed, assigned );

        protected override void WriteContents( BinaryWriter writer ) {
            TimelineCount.SetValue( Items.Count );
            ClipCount.SetValue( Clips.Count );
            WriteNested( writer, Parsed );

            // Item
            for( var i = 0; i < Items.Count; i++ ) {
                var item = new AvfxTimelineItem( this );
                item.Items.AddRange( Items.GetRange( 0, i + 1 ) );
                item.Write( writer );
            }

            foreach( var clip in Clips ) clip.Write( writer );
        }

        public override void Draw( string parentId ) {
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
            IUiBase.DrawList( Display, id );
            ImGui.EndChild();
        }

        public override void PopulateWorkspaceMetaChildren( Dictionary<string, string> renameDict ) {
            Items.ForEach( item => IUiWorkspaceItem.PopulateWorkspaceMeta( item, renameDict ) );
            Clips.ForEach( item => IUiWorkspaceItem.PopulateWorkspaceMeta( item, renameDict ) );
        }

        public override void ReadWorkspaceMetaChildren( Dictionary<string, string> renameDict ) {
            Items.ForEach( item => IUiWorkspaceItem.ReadWorkspaceMeta( item, renameDict ) );
            Clips.ForEach( item => IUiWorkspaceItem.ReadWorkspaceMeta( item, renameDict ) );
        }

        public override string GetDefaultText() => $"Timeline {GetIdx()}";

        public override string GetWorkspaceId() => $"Tmln{GetIdx()}";
    }
}
