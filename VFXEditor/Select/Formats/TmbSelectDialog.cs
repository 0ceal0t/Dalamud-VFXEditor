using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Select.Tabs.Actions;
using VfxEditor.Select.Tabs.Common;
using VfxEditor.Select.Tabs.Emotes;
using VfxEditor.Select.Tabs.Npc;
using VfxEditor.Select.Tabs.NpcID;
using VfxEditor.Spawn;

namespace VfxEditor.Select.Formats {
    public class TmbSelectDialog : SelectDialog {
        public TmbSelectDialog( string id, FileManagerBase manager, bool isSourceDialog ) : base( id, "tmb", manager, isSourceDialog ) {
            GameTabs.AddRange( [
                new ActionTabTmb( this, "Action" ),
                new ActionTabTmbNonPlayer( this, "Non-Player Action" ),
                new EmoteTabTmb( this, "Emote" ),
                new NpcTabTmb( this, "Npc" ),
                new NpcIDTabTmb( this, "NpcID" ),
                new CommonTabTmb( this, "Common" )
            ] );
        }

        public override bool CanPlay => true;

        public override void PlayButton( string path ) {
            using var font = ImRaii.PushFont( UiBuilder.IconFont );

            if( TmbSpawn.CanReset ) {
                if( ImGui.Button( FontAwesomeIcon.Stop.ToIconString() ) ) TmbSpawn.Reset();
            }
            else {
                if( ImGui.Button( FontAwesomeIcon.Play.ToIconString() ) ) TmbSpawn.Apply( path );
            }
        }
    }
}