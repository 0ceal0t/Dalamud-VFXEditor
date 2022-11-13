using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxSchedulerSubItem : GenericWorkspaceItem {
        public readonly AvfxScheduler Scheduler;
        public readonly string Name;

        public readonly AvfxBool Enabled = new( "Enabled", "bEna" );
        public readonly AvfxInt StartTime = new( "Start Time", "StTm" );
        public readonly AvfxInt TimelineIdx = new( "Timeline Index", "TlNo" );

        private readonly List<AvfxBase> Children;

        public readonly UiNodeSelect<AvfxTimeline> TimelineSelect;

        private readonly List<IUiBase> Parameters;

        public AvfxSchedulerSubItem( AvfxScheduler scheduler, string name ) {
            Scheduler = scheduler;
            Name = name;

            Children = new List<AvfxBase> {
                Enabled,
                StartTime,
                TimelineIdx
            };

            Enabled.SetValue( true );
            StartTime.SetValue( 0 );
            TimelineIdx.SetValue( -1 );

            TimelineSelect = new UiNodeSelect<AvfxTimeline>( scheduler, "Timeline", Scheduler.NodeGroups.Timelines, TimelineIdx );

            Parameters = new() {
                Enabled,
                StartTime
            };
        }

        public AvfxSchedulerSubItem( AvfxScheduler scheduler, BinaryReader reader, string name ) : this( scheduler, name ) => AvfxBase.ReadNested( reader, Children, 36 );

        public void Write( BinaryWriter writer ) => AvfxBase.WriteNested( writer, Children );

        public override void Draw( string parentId ) {
            var id = parentId + "/" + Name;
            DrawRename( id );
            TimelineSelect.Draw( id );
            IUiBase.DrawList( Parameters, id );
        }

        public override string GetDefaultText() => $"{GetIdx()}: Timeline {TimelineIdx.GetValue()}";

        public override string GetWorkspaceId() {
            var Type = ( Name == "Item" ) ? "Item" : "Trigger";
            return $"{Scheduler.GetWorkspaceId()}/{Type}{GetIdx()}";
        }
    }
}
