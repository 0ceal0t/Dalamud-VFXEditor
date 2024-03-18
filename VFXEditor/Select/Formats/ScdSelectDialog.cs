using VfxEditor.ScdFormat;
using VfxEditor.Select.Tabs.Actions;
using VfxEditor.Select.Tabs.Bgm;
using VfxEditor.Select.Tabs.BgmQuest;
using VfxEditor.Select.Tabs.Common;
using VfxEditor.Select.Tabs.Instance;
using VfxEditor.Select.Tabs.Mounts;
using VfxEditor.Select.Tabs.Orchestrions;
using VfxEditor.Select.Tabs.Voice;
using VfxEditor.Select.Tabs.Zone;

namespace VfxEditor.Select.Formats {
    public class ScdSelectDialog : SelectDialog {
        public ScdSelectDialog( string id, ScdManager manager, bool isSourceDialog ) : base( id, "scd", manager, isSourceDialog ) {
            GameTabs.AddRange( [
                new ActionTabScd( this, "Actions" ),
                new MountTabScd( this, "Mount" ),
                new OrchestrionTab( this, "Orchestrion" ),
                new ZoneTabScd( this, "Zone" ),
                new BgmTab( this, "BGM" ),
                new BgmQuestTab( this, "Quest BGM" ),
                new InstanceTab( this, "Instance" ),
                new VoiceTab( this, "Voice" ),
                new CommonTabScd( this, "Common" ),
            ] );
        }
    }
}
