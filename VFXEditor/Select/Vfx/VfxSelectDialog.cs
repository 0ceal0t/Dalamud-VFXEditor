using ImGuiNET;
using OtterGui.Raii;
using VfxEditor.AvfxFormat;
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
using VfxEditor.Spawn;

namespace VfxEditor.Select.Vfx {
    public class VfxSelectDialog : SelectDialog {
        public VfxSelectDialog( string id, AvfxManager manager, bool isSourceDialog ) : base( id, "avfx", manager, isSourceDialog ) {
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
            if( VfxSpawn.Exists ) {
                if( ImGui.Button( "Remove" ) ) VfxSpawn.Remove();
            }
            else {
                if( ImGui.Button( "Spawn" ) ) ImGui.OpenPopup( "SpawnPopup" );

                if( ImGui.BeginPopup( "SpawnPopup" ) ) {
                    if( ImGui.Selectable( "On Ground" ) ) VfxSpawn.OnGround( path );
                    if( ImGui.Selectable( "On Self" ) ) VfxSpawn.OnSelf( path );
                    if( ImGui.Selectable( "On Target" ) ) VfxSpawn.OnTarget( path );
                    ImGui.EndPopup();
                }
            }
        }
    }
}
