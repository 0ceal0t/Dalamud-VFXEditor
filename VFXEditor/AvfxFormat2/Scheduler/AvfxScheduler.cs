using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxScheduler : AvfxNode {
        public const string NAME = "Schd";

        public readonly AvfxInt ItemCount = new( "Item Count", "ItCn" );
        public readonly AvfxInt TriggerCount = new( "Trigger Count", "TrCn" );

        public readonly List<AvfxSchedulerSubItem> Items = new();
        public readonly List<AvfxSchedulerSubItem> Triggers = new();

        public readonly UiSchedulerSplitView ItemSplit;
        public readonly UiSchedulerSplitView TriggerSplit;

        private readonly List<AvfxBase> Parsed;

        public readonly UiNodeGroupSet NodeGroups;

        public AvfxScheduler( UiNodeGroupSet groupSet ) : base( NAME, UiNodeGroup.SchedColor, false ) {
            NodeGroups = groupSet;

            Parsed = new() {
                ItemCount,
                TriggerCount
            };

            ItemSplit = new( Items, this, true );
            TriggerSplit = new( Triggers, this, false );
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            Peek( reader, Parsed, size );
            AvfxSchedulerItem lastItem = null;
            AvfxSchedulerItem lastTrigger = null;

            ReadNested( reader, ( BinaryReader _reader, string _name, int _size ) => {
                if( _name == "Item" ) {
                    lastItem = new AvfxSchedulerItem( "Item", this );
                    lastItem.Read( _reader, _size );
                }
                else if( _name == "Trgr" ) {
                    lastTrigger = new AvfxSchedulerItem( "Trgr", this );
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

            ItemSplit.UpdateIdx();
            TriggerSplit.UpdateIdx();

            DepedencyImportInProgress = false;
        }

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Parsed, assigned );

        protected override void WriteContents( BinaryWriter writer ) {
            ItemCount.SetValue( Items.Count );
            TriggerCount.SetValue( Triggers.Count );
            WriteNested( writer, Parsed );

            // Item
            for( var i = 0; i < Items.Count; i++ ) {
                var item = new AvfxSchedulerItem( "Item", this );
                item.Items.AddRange( Items.GetRange( 0, i + 1 ) );
                item.Write( writer );
            }

            // Trgr
            for( var i = 0; i < Triggers.Count; i++ ) {
                var trigger = new AvfxSchedulerItem( "Trgr", this );
                trigger.Items.AddRange( Items );
                trigger.Items.AddRange( Triggers.GetRange( 0, i + 1 ) ); // get 1, then 2, etc.
                trigger.Write( writer );
            }
        }

        public override void Draw( string parentId ) {
            var id = parentId + "/Scheduler";

            if( ImGui.BeginTabBar( id + "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) ) {
                if( ImGui.BeginTabItem( "Items" + id ) ) {
                    ItemSplit.Draw( id + "/Item" );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Triggers" + id ) ) {
                    TriggerSplit.Draw( id + "/Trigger" );
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
        }

        public override void PopulateWorkspaceMetaChildren( Dictionary<string, string> renameDict ) {
            Items.ForEach( item => IUiWorkspaceItem.PopulateWorkspaceMeta( item, renameDict ) );
            Triggers.ForEach( item => IUiWorkspaceItem.PopulateWorkspaceMeta( item, renameDict ) );
        }

        public override void ReadWorkspaceMetaChildren( Dictionary<string, string> renameDict ) {
            Items.ForEach( item => IUiWorkspaceItem.ReadWorkspaceMeta( item, renameDict ) );
            Triggers.ForEach( item => IUiWorkspaceItem.ReadWorkspaceMeta( item, renameDict ) );
        }

        public override string GetDefaultText() => $"Scheduler {GetIdx()}";

        public override string GetWorkspaceId() => $"Sched{GetIdx()}";
    }
}
