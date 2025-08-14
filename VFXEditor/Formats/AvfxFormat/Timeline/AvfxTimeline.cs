using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
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

        public readonly List<AvfxTimelineClip> Clips = [];
        public readonly List<AvfxTimelineItem> Items = [];

        public readonly AvfxNodeGroupSet NodeGroups;

        public readonly AvfxNodeSelect<AvfxBinder> BinderSelect;

        public readonly UiTimelineClipSplitView ClipSplit;
        public readonly UiTimelineItemSequencer ItemSplit;
        private readonly List<IUiItem> Display;

        public AvfxTimeline( AvfxNodeGroupSet groupSet ) : base( NAME, AvfxNodeGroupSet.TimelineColor ) {
            NodeGroups = groupSet;

            Parsed = [
                LoopStart,
                LoopEnd,
                BinderIdx,
                TimelineCount,
                ClipCount
            ];

            Display = [
                LoopStart,
                LoopEnd
            ];

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

        public override void WriteContents( BinaryWriter writer ) {
            TimelineCount.Value = Items.Count;
            ClipCount.Value = Clips.Count;
            WriteNested( writer, Parsed );

            // Item
            for( var i = 0; i < Items.Count; i++ ) {
                var item = new AvfxTimelineItemContainer( this );
                item.Items.AddRange( Items.GetRange( 0, i + 1 ) );
                item.Write( writer );
            }

            foreach( var clip in Clips ) clip.Write( writer );
        }

        protected override IEnumerable<AvfxBase> GetChildren() {
            foreach( var item in Parsed ) yield return item;
        }

        public override void Draw() {
            using var _ = ImRaii.PushId( "Timeline" );
            DrawRename();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            DrawParameters();

            using( var tab = ImRaii.TabItem( "Items" ) ) {
                if( tab ) ItemSplit.Draw();
            }

            using( var tab = ImRaii.TabItem( "Clips" ) ) {
                if( tab ) ClipSplit.Draw();
            }
        }

        private void DrawParameters() {
            using var tabItem = ImRaii.TabItem( "Parameters" );
            if( !tabItem ) return;

            using var child = ImRaii.Child( "Child" );
            BinderSelect.Draw();
            DrawItems( Display );
        }

        public override void GetChildrenRename( Dictionary<string, string> renameDict ) {
            Items.ForEach( item => IWorkspaceUiItem.GetRenamingMap( item, renameDict ) );
            Clips.ForEach( item => IWorkspaceUiItem.GetRenamingMap( item, renameDict ) );
        }

        public override void SetChildrenRename( Dictionary<string, string> renameDict ) {
            Items.ForEach( item => IWorkspaceUiItem.ReadRenamingMap( item, renameDict ) );
            Clips.ForEach( item => IWorkspaceUiItem.ReadRenamingMap( item, renameDict ) );
        }

        public override string GetDefaultText() => $"Timeline {GetIdx()}";

        public override string GetWorkspaceId() => $"Tmln{GetIdx()}";
    }
}
