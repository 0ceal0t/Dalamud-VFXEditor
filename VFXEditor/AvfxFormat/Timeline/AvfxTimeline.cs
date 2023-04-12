using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.AvfxFormat {
    public class AvfxTimeline : AvfxNode {
        public const string NAME = "TmLn";

        public readonly AvfxInt LoopStart = new( "Loop Start", "LpSt" );
        public readonly AvfxInt LoopEnd = new( "Loop End", "LpEd" );
        public readonly AvfxInt BinderIdx = new( "Binder Index", "BnNo" );
        public readonly AvfxInt TimelineCount = new( "Item Count", "TICn" );
        public readonly AvfxInt ClipCount = new( "Clip Count", "CpCn" );

        private readonly List<AvfxBase> Parsed;

        public readonly List<AvfxTimelineClip> Clips = new();
        public readonly List<AvfxTimelineItem> Items = new();

        public readonly UiNodeGroupSet NodeGroups;

        public readonly UiNodeSelect<AvfxBinder> BinderSelect;

        public readonly UiTimelineClipSplitView ClipSplit;
        public readonly UiTimelineItemSequencer ItemSplit;
        private readonly List<IAvfxUiBase> Display;

        public AvfxTimeline( UiNodeGroupSet groupSet ) : base( NAME, UiNodeGroup.TimelineColor ) {
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

            AvfxTimelineItemContainer lastItem = null;

            ReadNested( reader, ( BinaryReader _reader, string _name, int _size ) => {
                if( _name == "Item" ) {
                    lastItem = new AvfxTimelineItemContainer( this );
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
        }

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Parsed, assigned );

        protected override void WriteContents( BinaryWriter writer ) {
            TimelineCount.SetValue( Items.Count );
            ClipCount.SetValue( Clips.Count );
            WriteNested( writer, Parsed );

            // Item
            for( var i = 0; i < Items.Count; i++ ) {
                var item = new AvfxTimelineItemContainer( this );
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
            IAvfxUiBase.DrawList( Display, id );
            ImGui.EndChild();
        }

        public override void GetChildrenRename( Dictionary<string, string> renameDict ) {
            Items.ForEach( item => IWorkspaceUiItem.PopulateMeta( item, renameDict ) );
            Clips.ForEach( item => IWorkspaceUiItem.PopulateMeta( item, renameDict ) );
        }

        public override void SetChildrenRename( Dictionary<string, string> renameDict ) {
            Items.ForEach( item => IWorkspaceUiItem.ReadMeta( item, renameDict ) );
            Clips.ForEach( item => IWorkspaceUiItem.ReadMeta( item, renameDict ) );
        }

        public override string GetDefaultText() => $"Timeline {GetIdx()}";

        public override string GetWorkspaceId() => $"Tmln{GetIdx()}";
    }
}
