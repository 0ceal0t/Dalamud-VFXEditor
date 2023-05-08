using ImGuiNET;
using System;
using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Select.Vfx.Action;
using VfxEditor.Select.Vfx.Common;
using VfxEditor.Select.Vfx.Cutscene;
using VfxEditor.Select.Vfx.Emote;
using VfxEditor.Select.Vfx.Gimmick;
using VfxEditor.Select.Vfx.Housing;
using VfxEditor.Select.Vfx.Item;
using VfxEditor.Select.Vfx.JournalCutscene;
using VfxEditor.Select.Vfx.Mount;
using VfxEditor.Select.Vfx.Npc;
using VfxEditor.Select.Vfx.Status;
using VfxEditor.Select.Vfx.Zone;

namespace VfxEditor.Select.Vfx {
    public class VfxSelectDialog : SelectDialog {
        private readonly List<SelectTab> GameTabs;

        public VfxSelectDialog( string id, FileManagerWindow manager, bool isSourceDialog ) : base( id, "avfx", manager, isSourceDialog ) {
            GameTabs = new List<SelectTab>( new SelectTab[]{
                new ItemTab( this, "Item" ),
                new StatusTab( this, "Status" ),
                new ActionTab( this, "Action" ),
                new NonPlayerActionTab( this, "Non-Player Action" ),
                new EmoteTab( this, "Emote" ),
                new ZoneTab( this, "Zone" ),
                new GimmickTab( this, "Gimmick" ),
                new HousingTab( this, "Housing" ),
                new NpcVfxTab( this, "Npc" ),
                new MountTab( this, "Mount" ),
                new CutsceneTab( this, "Cutscene" ),
                new JournalCutsceneTab( this, "Journal Cutscene" ),
                new CommonTab( this, "Common" )
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
                    if( !Plugin.InGpose && ImGui.Selectable( "On Self" ) ) Plugin.SpawnOnSelf( playPath );
                    if( ImGui.Selectable( "On Target" ) ) Plugin.SpawnOnTarget( playPath );
                    ImGui.EndPopup();
                }
            }
        }

        protected override List<SelectTab> GetTabs() => GameTabs;
    }
}
