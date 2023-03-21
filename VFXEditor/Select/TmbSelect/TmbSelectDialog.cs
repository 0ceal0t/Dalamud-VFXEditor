using ImGuiNET;
using System;
using System.Collections.Generic;
using VfxEditor.FileManager;

namespace VfxEditor.Select.TmbSelect {
    public class TmbSelectDialog : SelectDialog {
        private readonly List<SelectTab> GameTabs;

        public TmbSelectDialog( string id, FileManagerWindow manager, bool isSourceDialog ) : base( id, "tmb", manager, isSourceDialog ) {

            GameTabs = new List<SelectTab>( new SelectTab[]{
                new TmbActionSelect( "Action", this ),
                new TmbActionSelect( "Non-Player Action", this, nonPlayer:true ),
                new TmbEmoteSelect( "Emote", this ),
                new TmbNpcSelect( "Npc", this ),
                new TmbCommonSelect( "Common", this )
            } );
        }

        public override void Play( string playPath, string parentId ) {
            var reset = Plugin.ActorAnimationManager.CanReset;

            ImGui.SameLine();
            if( reset ) {
                if( ImGui.Button( "Reset##" + parentId ) ) Plugin.ActorAnimationManager.Reset();
            }
            else {
                if( ImGui.Button( "Play##" + parentId ) ) Plugin.ActorAnimationManager.Apply( playPath );
            }
        }

        protected override List<SelectTab> GetTabs() => GameTabs;
    }
}
