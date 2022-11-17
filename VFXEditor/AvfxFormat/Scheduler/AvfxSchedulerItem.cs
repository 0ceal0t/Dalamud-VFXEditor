using System;
using System.Collections.Generic;
using System.IO;

namespace VfxEditor.AvfxFormat {
    public class AvfxSchedulerItem : GenericWorkspaceItem {
        public readonly AvfxScheduler Scheduler;
        public readonly string Name;

        public readonly AvfxBool Enabled = new( "Enabled", "bEna", defaultValue: true );
        public readonly AvfxInt StartTime = new( "Start Time", "StTm", defaultValue: 0 );
        public readonly AvfxInt TimelineIdx = new( "Timeline Index", "TlNo", defaultValue: -1 );

        private readonly List<AvfxBase> Parsed;

        public UiNodeSelect<AvfxTimeline> TimelineSelect;

        private readonly List<IAvfxUiBase> Display;

        public AvfxSchedulerItem( AvfxScheduler scheduler, string name, bool initNodeSelects ) {
            Scheduler = scheduler;
            Name = name;

            Parsed = new List<AvfxBase> {
                Enabled,
                StartTime,
                TimelineIdx
            };

            if( initNodeSelects ) InitializeNodeSelects();

            Display = new() {
                Enabled,
                StartTime
            };
        }

        public AvfxSchedulerItem( AvfxScheduler scheduler, bool initNodeSelects, BinaryReader reader, string name ) : this( scheduler, name, initNodeSelects ) => AvfxBase.ReadNested( reader, Parsed, 36 );

        public void InitializeNodeSelects() {
            TimelineSelect = new UiNodeSelect<AvfxTimeline>( Scheduler, "Timeline", Scheduler.NodeGroups.Timelines, TimelineIdx );
        }

        public void Write( BinaryWriter writer ) => AvfxBase.WriteNested( writer, Parsed );

        public override void Draw( string parentId ) {
            var id = parentId + "/" + Name;
            DrawRename( id );
            TimelineSelect.Draw( id );
            IAvfxUiBase.DrawList( Display, id );
        }

        public override string GetDefaultText() => $"{GetIdx()}: Timeline {TimelineIdx.GetValue()}";

        public override string GetWorkspaceId() {
            var Type = ( Name == "Item" ) ? "Item" : "Trigger";
            return $"{Scheduler.GetWorkspaceId()}/{Type}{GetIdx()}";
        }
    }
}
