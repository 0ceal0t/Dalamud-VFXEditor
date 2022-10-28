using ImGuiNET;
using System;
using System.Collections.Generic;

namespace VFXEditor.Select.VfxSelect {
    public class VfxSelectDialog : SelectDialog {
        private readonly bool ShowSpawn = false;
        private readonly Func<bool> SpawnVfxExists;
        private readonly Action RemoveSpawnVfx;
        private readonly Action<string> SpawnOnGround;
        private readonly Action<string> SpawnOnSelf;
        private readonly Action<string> SpawnOnTarget;
        private readonly List<SelectTab> GameTabs;

        public VfxSelectDialog(
            string id,
            List<SelectResult> recentList,
            bool showLocal,
            Action<SelectResult> onSelect,
            bool showSpawn = false,
            Func<bool> spawnVfxExists = null,
            Action removeSpawnVfx = null,
            Action<string> spawnOnGround = null,
            Action<string> spawnOnSelf = null,
            Action<string> spawnOnTarget = null ) : base( id, "avfx", recentList, showLocal, onSelect ) {

            ShowSpawn = showSpawn;
            SpawnVfxExists = spawnVfxExists;
            RemoveSpawnVfx = removeSpawnVfx;
            SpawnOnGround = spawnOnGround;
            SpawnOnSelf = spawnOnSelf;
            SpawnOnTarget = spawnOnTarget;

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

        public override void Spawn( string spawnPath, string id = "" ) {
            if( !ShowSpawn ) return;
            ImGui.SameLine();
            if( SpawnVfxExists() ) {
                if( ImGui.Button( "Remove##" + id ) ) {
                    RemoveSpawnVfx();
                }
            }
            else {
                if( ImGui.Button( "Spawn##" + id ) ) {
                    ImGui.OpenPopup( "Spawn_Popup##" + id );
                }
                if( ImGui.BeginPopup( "Spawn_Popup##" + id ) ) {
                    if( ImGui.Selectable( "On Ground" ) ) {
                        SpawnOnGround( spawnPath );
                    }
                    if( ImGui.Selectable( "On Self" ) ) {
                        SpawnOnSelf( spawnPath );
                    }
                    if( ImGui.Selectable( "On Target" ) ) {
                        SpawnOnTarget( spawnPath );
                    }
                    ImGui.EndPopup();
                }
            }
        }

        protected override List<SelectTab> GetTabs() => GameTabs;
    }
}
