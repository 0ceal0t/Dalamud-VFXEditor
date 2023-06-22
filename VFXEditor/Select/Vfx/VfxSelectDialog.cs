using ImGuiNET;
using OtterGui.Raii;
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
        public VfxSelectDialog( string id, FileManagerWindow manager, bool isSourceDialog ) : base( id, "avfx", manager, isSourceDialog ) {
            GameTabs.AddRange( new SelectTab[]{
                new VfxItemTab( this, "Item" ),
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

        public override void Play( string path ) {
            using var _ = ImRaii.PushId( "Spawn" );

            ImGui.SameLine();
            if( Plugin.SpawnExists() ) {
                if( ImGui.Button( "Remove" ) ) Plugin.RemoveSpawn();
            }
            else {
                if( ImGui.Button( "Spawn" ) ) ImGui.OpenPopup( "SpawnPopup" );

                if( ImGui.BeginPopup( "SpawnPopup" ) ) {
                    if( ImGui.Selectable( "On Ground" ) ) Plugin.SpawnOnGround( path );
                    if( !Plugin.InGpose && ImGui.Selectable( "On Self" ) ) Plugin.SpawnOnSelf( path );
                    if( ImGui.Selectable( "On Target" ) ) Plugin.SpawnOnTarget( path );
                    ImGui.EndPopup();
                }
            }
        }
    }
}
