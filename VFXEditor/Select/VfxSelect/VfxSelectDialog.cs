using ImGuiNET;
using System;
using System.Collections.Generic;

namespace VfxEditor.Select.VfxSelect {
    public class VfxSelectDialog : SelectDialog {
        private readonly List<SelectTab> GameTabs;

        public VfxSelectDialog(
            string id,
            List<SelectResult> recentList,
            bool showLocal,
            Action<SelectResult> onSelect
         ) : base( id, "avfx", recentList, Plugin.Configuration.FavoriteVfx, showLocal, onSelect ) {

            GameTabs = new List<SelectTab>( new SelectTab[]{
                new VfxItemSelect( "Item", this ),
                new VfxStatusSelect( "Status", this ),
                new VfxActionSelect( "Action", this ),
                new VfxActionSelect( "Non-Player Action", this, nonPlayer:true ),
                new VfxZoneSelect( "Zone", this ),
                new VfxNpcSelect( "Npc", this ),
                new VfxEmoteSelect( "Emote", this ),
                new VfxGimmickSelect( "Gimmick", this ),
                new VfxCutsceneSelect( "Cutscene", this ),
                new VfxMountSelect( "Mount", this),
                new VfxHousingSelect( "Housing", this),
                new VfxCommonSelect( "Common", this)
            } );
        }

        public override void Play( string playPath, string parentId ) {
            ImGui.SameLine();
            if( Plugin.SpawnExists() ) {
                if( ImGui.Button( "Remove##" + parentId ) ) Plugin.RemoveSpawn();
            }
            else {
                if( ImGui.Button( "Spawn##" + parentId ) ) ImGui.OpenPopup( "Spawn_Popup##" + parentId );

                if( ImGui.BeginPopup( "Spawn_Popup##" + parentId ) ) {
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
