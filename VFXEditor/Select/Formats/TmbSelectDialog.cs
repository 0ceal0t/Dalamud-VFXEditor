using ImGuiNET;
using Dalamud.Interface.Utility.Raii;
using VfxEditor.Select.Tabs.Actions;
using VfxEditor.Select.Tabs.Common;
using VfxEditor.Select.Tabs.Emotes;
using VfxEditor.Select.Tabs.Npc;
using VfxEditor.Spawn;
using VfxEditor.TmbFormat;

namespace VfxEditor.Select.Formats {
    public class TmbSelectDialog : SelectDialog {
        public TmbSelectDialog( string id, TmbManager manager, bool isSourceDialog ) : base( id, "tmb", manager, isSourceDialog ) {
            GameTabs.AddRange( new SelectTab[]{
                new ActionTabTmb( this, "Action" ),
                new ActionTabTmbNonPlayer( this, "Non-Player Action" ),
                new EmoteTabTmb( this, "Emote" ),
                new NpcTabTmb( this, "Npc" ),
                new CommonTabTmb( this, "Common" )
            } );
        }

        public override void Play( string path ) {
            using var _ = ImRaii.PushId( "Spawn" );

            ImGui.SameLine();
            if( TmbSpawn.CanReset ) {
                if( ImGui.Button( "Reset" ) ) TmbSpawn.Reset();
            }
            else {
                if( ImGui.Button( "Play" ) ) TmbSpawn.Apply( path );
            }
        }
    }
}