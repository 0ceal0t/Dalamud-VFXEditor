using ImGuiNET;
using System;
using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Select2.Tmb.Action;
using VfxEditor.Select2.Tmb.Common;
using VfxEditor.Select2.Tmb.Emote;
using VfxEditor.Select2.Tmb.Npc;

namespace VfxEditor.Select2.Tmb {
    public class TmbSelectDialog : SelectDialog {
        private readonly List<SelectTab> GameTabs;

        public TmbSelectDialog( string id, FileManagerWindow manager, bool isSourceDialog ) : base( id, "tmb", manager, isSourceDialog ) {
            GameTabs = new List<SelectTab>( new SelectTab[]{
                new ActionTab( this, "Action" ),
                new NonPlayerActionTab( this, "Non-Player Action" ),
                new EmoteTab( this, "Emote" ),
                new NpcTmbTab( this, "Npc" ),
                new CommonTab( this, "Common" )
            } );
        }

        public override void Play( string playPath, string parentId ) {
            ImGui.SameLine();
            if( Plugin.ActorAnimationManager.CanReset ) {
                if( ImGui.Button( "Reset##" + parentId ) ) Plugin.ActorAnimationManager.Reset();
            }
            else {
                if( ImGui.Button( "Play##" + parentId ) ) Plugin.ActorAnimationManager.Apply( playPath );
            }
        }

        protected override List<SelectTab> GetTabs() => GameTabs;
    }
}
