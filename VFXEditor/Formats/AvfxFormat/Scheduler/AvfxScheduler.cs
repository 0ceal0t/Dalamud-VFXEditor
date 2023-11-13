using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Ui.Components.Tables;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.AvfxFormat {
    public class AvfxScheduler : AvfxNode {
        public const string NAME = "Schd";

        public readonly AvfxInt ItemCount = new( "Item Count", "ItCn" );
        public readonly AvfxInt TriggerCount = new( "Trigger Count", "TrCn" );
        public readonly List<AvfxSchedulerItem> Items = new();
        public readonly List<AvfxSchedulerItem> Triggers = new();
        public readonly List<AvfxBase> Parsed;
        public readonly AvfxNodeGroupSet NodeGroups;

        private readonly CommandTable<AvfxSchedulerItem> ItemTable;
        private readonly CommandTable<AvfxSchedulerItem> TriggerTable;

        private static readonly List<(string, ImGuiTableColumnFlags, int)> Columns = new() {
            ( "Name", ImGuiTableColumnFlags.WidthStretch, -1 ),
            ( "Timeline", ImGuiTableColumnFlags.None, -1 ),
            ( "Enabled", ImGuiTableColumnFlags.None, -1 ),
            ( "Start Time", ImGuiTableColumnFlags.None, -1 )
        };

        public AvfxScheduler( AvfxNodeGroupSet groupSet ) : base( NAME, AvfxNodeGroupSet.SchedColor ) {
            NodeGroups = groupSet;

            Parsed = new() {
                ItemCount,
                TriggerCount
            };

            ItemTable = new( "ItEm", true, Items, Columns, () => new( this, "Item", true ),
            ( AvfxSchedulerItem item, bool add ) => {
                if( add ) item.TimelineSelect.Enable();
                else item.TimelineSelect.Disable();
                IIndexUiItem.UpdateIdx( Items );
            } );

            TriggerTable = new( "Trgr", false, Triggers, Columns, () => new( this, "Trgr", true ), null );
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            Peek( reader, Parsed, size );
            AvfxSchedulerItemContainer lastItem = null;
            AvfxSchedulerItemContainer lastTrigger = null;

            ReadNested( reader, ( BinaryReader _reader, string _name, int _size ) => {
                if( _name == "Item" ) {
                    lastItem = new AvfxSchedulerItemContainer( "Item", this );
                    lastItem.Read( _reader, _size );
                }
                else if( _name == "Trgr" ) {
                    lastTrigger = new AvfxSchedulerItemContainer( "Trgr", this );
                    lastTrigger.Read( _reader, _size );
                }
            }, size );

            if( lastItem != null ) {
                Items.AddRange( lastItem.Items );
                Items.ForEach( x => x.InitializeNodeSelects() );
            }

            if( lastTrigger != null ) {
                Triggers.AddRange( lastTrigger.Items.GetRange( lastTrigger.Items.Count - 12, 12 ) );
                Triggers.ForEach( x => x.InitializeNodeSelects() );
            }

            IIndexUiItem.UpdateIdx( Items );
            IIndexUiItem.UpdateIdx( Triggers );
        }

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Parsed, assigned );

        public override void WriteContents( BinaryWriter writer ) {
            ItemCount.Value = Items.Count;
            TriggerCount.Value = Triggers.Count;
            WriteNested( writer, Parsed );

            // Item
            for( var i = 0; i < Items.Count; i++ ) {
                var item = new AvfxSchedulerItemContainer( "Item", this );
                item.Items.AddRange( Items.GetRange( 0, i + 1 ) );
                item.Write( writer );
            }

            // Trgr
            for( var i = 0; i < Triggers.Count; i++ ) {
                var trigger = new AvfxSchedulerItemContainer( "Trgr", this );
                trigger.Items.AddRange( Items );
                trigger.Items.AddRange( Triggers.GetRange( 0, i + 1 ) ); // get 1, then 2, etc.
                trigger.Write( writer );
            }
        }

        public override void Draw() {
            using var _ = ImRaii.PushId( "Scheduler" );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Items" ) ) {
                if( tab ) ItemTable.Draw();
            }

            using( var tab = ImRaii.TabItem( "Triggers" ) ) {
                if( tab ) TriggerTable.Draw();
            }
        }

        public override void GetChildrenRename( Dictionary<string, string> renameDict ) {
            Items.ForEach( item => IWorkspaceUiItem.GetRenamingMap( item, renameDict ) );
            Triggers.ForEach( item => IWorkspaceUiItem.GetRenamingMap( item, renameDict ) );
        }

        public override void SetChildrenRename( Dictionary<string, string> renameDict ) {
            Items.ForEach( item => IWorkspaceUiItem.ReadRenamingMap( item, renameDict ) );
            Triggers.ForEach( item => IWorkspaceUiItem.ReadRenamingMap( item, renameDict ) );
        }

        public override string GetDefaultText() => $"Scheduler {GetIdx()}";

        public override string GetWorkspaceId() => $"Sched{GetIdx()}";
    }
}
