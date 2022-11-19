using ImGuiNET;
using System;
using System.Collections.Generic;

namespace VfxEditor.Select.VfxSelect {
    public class VfxSelectDialog : SelectDialog {
        private readonly List<SelectTab> GameTabs;

        public VfxSelectDialog( string id, List<SelectResult> recentList, bool showLocal, Action<SelectResult> onSelect ) : base( id, "avfx", recentList, showLocal, onSelect ) {

            GameTabs = new List<SelectTab>( new SelectTab[]{
                new VfxItemSelect( id, "Item", this ),
                new VfxStatusSelect( id, "Status", this ),
                new VfxActionSelect( id, "Action", this ),
                new VfxActionSelect( id, "Non-Player Action", this, nonPlayer:true ),
                new VfxZoneSelect( id, "Zone", this ),
                new VfxNpcSelect( id, "Npc", this ),
                new VfxEmoteSelect( id, "Emote", this ),
                new VfxGimmickSelect( id, "Gimmick", this ),
                new VfxCutsceneSelect( id, "Cutscene", this ),
                new VfxMountSelect( id, "Mount", this),
                new VfxHousingSelect( id, "Housing", this),
                new VfxCommonSelect( id, "Common", this)
            } );
        }

        public override void Play( string playPath, string id = "" ) {
            ImGui.SameLine();
            if( Plugin.SpawnExists() ) {
                if( ImGui.Button( "Remove##" + id ) ) Plugin.RemoveSpawn();
            }
            else {
                if( ImGui.Button( "Spawn##" + id ) ) ImGui.OpenPopup( "Spawn_Popup##" + id );

                if( ImGui.BeginPopup( "Spawn_Popup##" + id ) ) {
                    if( ImGui.Selectable( "On Ground" ) ) Plugin.SpawnOnGround( playPath );
                    if( ImGui.Selectable( "On Self" ) ) Plugin.SpawnOnSelf( playPath );
                    if( ImGui.Selectable( "On Target" ) ) Plugin.SpawnOnTarget( playPath );
                    ImGui.EndPopup();
                }
            }
        }

        protected override List<SelectTab> GetTabs() => GameTabs;
    }
}
