using ImGuiNET;
using System;
using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Select2.Vfx.Action;
using VfxEditor.Select2.Vfx.Common;
using VfxEditor.Select2.Vfx.Cutscene;
using VfxEditor.Select2.Vfx.Emote;
using VfxEditor.Select2.Vfx.Gimmick;
using VfxEditor.Select2.Vfx.Housing;
using VfxEditor.Select2.Vfx.Item;
using VfxEditor.Select2.Vfx.JournalCutscene;
using VfxEditor.Select2.Vfx.Mount;
using VfxEditor.Select2.Vfx.Npc;
using VfxEditor.Select2.Vfx.Status;
using VfxEditor.Select2.Vfx.Zone;

namespace VfxEditor.Select2.Vfx {
    public class VfxSelectDialog : SelectDialog {
        private readonly List<SelectTab> GameTabs;

        public VfxSelectDialog( string id, FileManagerWindow manager, bool isSourceDialog ) : base( id, "avfx", manager, isSourceDialog ) {
            GameTabs = new List<SelectTab>( new SelectTab[]{
                new ItemTab( this, "Item" ),
                new StatusTab( this, "Status" ),
                new ActionTab( this, "Action" ),
                new NonPlayerActionTab( this, "Non-Player Action" ),
                new ZoneTab( this, "Zone" ),
                new NpcVfxTab( this, "Npc" ),
                new EmoteTab( this, "Emote" ),
                new GimmickTab( this, "Gimmick" ),
                new CutsceneTab( this, "Cutscene" ),
                new JournalCutsceneTab( this, "Journal Cutscene" ),
                new MountTab( this, "Mount" ),
                new HousingTab( this, "Housing" ),
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
