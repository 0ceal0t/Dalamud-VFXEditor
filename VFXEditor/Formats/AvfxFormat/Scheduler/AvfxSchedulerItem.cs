using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.AvfxFormat.Base;
using VfxEditor.Formats.AvfxFormat.Components.Bool;
using VfxEditor.Formats.AvfxFormat.Components.Int;
using VfxEditor.Formats.AvfxFormat.Nodes.Select;
using VfxEditor.Formats.AvfxFormat.Timeline;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.AvfxFormat.Scheduler {
    public class AvfxSchedulerItem : IUiItem {
        public readonly AvfxScheduler Scheduler;
        public readonly string Name;

        public readonly AvfxBool Enabled = new( "##Enabled", "bEna", value: true );
        public readonly AvfxInt StartTime = new( "##Start Time", "StTm", value: 0 );
        public readonly AvfxInt TimelineIdx = new( "##TimelineIndex", "TlNo", value: -1 );

        private readonly List<AvfxBase> Parsed;

        public AvfxNodeSelect<AvfxTimeline> TimelineSelect;

        public AvfxSchedulerItem( AvfxScheduler scheduler, string name, bool initNodeSelects ) {
            Scheduler = scheduler;
            Name = name;

            Parsed = [
                Enabled,
                StartTime,
                TimelineIdx
            ];

            if( initNodeSelects ) InitializeNodeSelects();
        }

        public AvfxSchedulerItem( AvfxScheduler scheduler, bool initNodeSelects, BinaryReader reader, string name ) : this( scheduler, name, initNodeSelects ) => AvfxBase.ReadNested( reader, Parsed, 36 );

        public void InitializeNodeSelects() {
            TimelineSelect = new AvfxNodeSelect<AvfxTimeline>( Scheduler, "##Timeline", Scheduler.NodeGroups.Timelines, TimelineIdx );
        }

        public void Write( BinaryWriter writer ) => AvfxBase.WriteNested( writer, Parsed );

        public void Draw() {
            ImGui.TableNextColumn();
            TimelineSelect.Draw( 250 );

            ImGui.TableNextColumn();
            Enabled.Draw();

            ImGui.TableNextColumn();
            ImGui.SetNextItemWidth( 100 );
            StartTime.Draw();
        }
    }
}
