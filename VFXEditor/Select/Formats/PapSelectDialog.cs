using VfxEditor.PapFormat;
using VfxEditor.Select.Tabs.Actions;
using VfxEditor.Select.Tabs.Character;
using VfxEditor.Select.Tabs.Common;
using VfxEditor.Select.Tabs.Emotes;
using VfxEditor.Select.Tabs.Event;
using VfxEditor.Select.Tabs.EventBase;
using VfxEditor.Select.Tabs.Items;
using VfxEditor.Select.Tabs.Job;
using VfxEditor.Select.Tabs.Mounts;
using VfxEditor.Select.Tabs.Npc;

namespace VfxEditor.Select.Formats {
    public class PapSelectDialog : SelectDialog {
        public PapSelectDialog( string id, PapManager manager, bool isSourceDialog ) : base( id, "pap", manager, isSourceDialog ) {
            GameTabs.AddRange( [
                new ItemTabPap( this, "Weapon" ),
                new ActionTabPap( this, "Action" ),
                new ActionTabPapNonPlayer( this, "Non-Player Action" ),
                new EmoteTabPap( this, "Emote" ),
                new NpcTabPap( this, "Npc" ),
                new MountTabPap( this, "Mount" ),
                new CharacterTabPap( this, "Character" ),
                new JobTab( this, "Job" ),
                new EventTab (this, "Cutscene (Human)" ),
                new EventBaseTab (this, "Ambient NPC (Human)" ),
                new CommonTabPap (this, "Common" ),
            ] );
        }
    }
}