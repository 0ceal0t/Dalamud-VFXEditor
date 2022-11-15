using System;
using System.Collections.Generic;
using System.IO;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxSchedulerSubItem : GenericWorkspaceItem {
        public readonly AvfxScheduler Scheduler;
        public readonly string Name;

        public readonly AvfxBool Enabled = new( "Enabled", "bEna" );
        public readonly AvfxInt StartTime = new( "Start Time", "StTm" );
        public readonly AvfxInt TimelineIdx = new( "Timeline Index", "TlNo" );

        private readonly List<AvfxBase> Parsed;

        public UiNodeSelect<AvfxTimeline> TimelineSelect;

        private readonly List<IUiBase> Display;

        public AvfxSchedulerSubItem( AvfxScheduler scheduler, string name, bool initNodeSelects ) {
            Scheduler = scheduler;
            Name = name;

            Parsed = new List<AvfxBase> {
                Enabled,
                StartTime,
                TimelineIdx
            };

            Enabled.SetValue( true );
            StartTime.SetValue( 0 );
            TimelineIdx.SetValue( -1 );

            if( initNodeSelects ) InitializeNodeSelects();

            Display = new() {
                Enabled,
                StartTime
            };
        }

        public AvfxSchedulerSubItem( AvfxScheduler scheduler, bool initNodeSelects, BinaryReader reader, string name ) : this( scheduler, name, initNodeSelects ) => AvfxBase.ReadNested( reader, Parsed, 36 );

        public void InitializeNodeSelects() {
            TimelineSelect = new UiNodeSelect<AvfxTimeline>( Scheduler, "Timeline", Scheduler.NodeGroups.Timelines, TimelineIdx );
        }

        public void Write( BinaryWriter writer ) => AvfxBase.WriteNested( writer, Parsed );

        public override void Draw( string parentId ) {
            var id = parentId + "/" + Name;
            DrawRename( id );
            TimelineSelect.Draw( id );
            IUiBase.DrawList( Display, id );
        }

        public override string GetDefaultText() => $"{GetIdx()}: Timeline {TimelineIdx.GetValue()}";

        public override string GetWorkspaceId() {
            var Type = ( Name == "Item" ) ? "Item" : "Trigger";
            return $"{Scheduler.GetWorkspaceId()}/{Type}{GetIdx()}";
        }
    }
}
