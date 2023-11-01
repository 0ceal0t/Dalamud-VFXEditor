using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;

namespace VfxEditor.Select.Tabs.Character {
    public class SelectedPap {
        // Idle, MoveA, MoveB
        public Dictionary<string, string> General;
        // Pose # -> Start, Loop
        public Dictionary<string, Dictionary<string, string>> Poses;
        public Dictionary<string, string> FacePaths;
    }

    public class CharacterTabPap : SelectTab<CharacterRow, SelectedPap> {
        public CharacterTabPap( SelectDialog dialog, string name ) : base( dialog, name, "Character", SelectResultType.GameCharacter ) { }

        // ===== LOADING =====

        public override void LoadData() => CharacterTab.Load( Items );

        public override void LoadSelection( CharacterRow item, out SelectedPap loaded ) {
            // General
            var general = new Dictionary<string, string>();
            var idlePath = item.GetPap( "idle" );
            var movePathA = item.GetPap( "move_a" );
            var movePathB = item.GetPap( "move_b" );
            if( Dalamud.DataManager.FileExists( idlePath ) ) general.Add( "Idle", idlePath );
            if( Dalamud.DataManager.FileExists( movePathA ) ) general.Add( "Move A", movePathA );
            if( Dalamud.DataManager.FileExists( movePathB ) ) general.Add( "Move B", movePathB );

            // Poses
            var poses = new Dictionary<string, Dictionary<string, string>>();
            for( var i = 1; i <= SelectDataUtils.MaxChangePoses; i++ ) {
                var start = item.GetStartPap( i );
                var loop = item.GetLoopPap( i );
                if( Dalamud.DataManager.FileExists( start ) && Dalamud.DataManager.FileExists( loop ) ) {
                    poses.Add( $"Pose {i}", new Dictionary<string, string>() {
                        { "Start", start },
                        { "Loop", loop }
                    } );
                }
            }

            // Faces
            var facePaths = new Dictionary<string, string>();
            foreach( var face in item.GetFaceIds() ) {
                facePaths[$"Face {face}"] = $"chara/human/{item.SkeletonId}/animation/f{face:D4}/resident/face.pap";
            }

            loaded = new SelectedPap {
                General = general,
                Poses = poses,
                FacePaths = SelectDataUtils.FileExistsFilter( facePaths ),
            };
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            using var tabBar = ImRaii.TabBar( "Tabs" );
            if( !tabBar ) return;

            if( ImGui.BeginTabItem( "General" ) ) {
                DrawPaps( Loaded.General, Selected.Name );
                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "Poses" ) ) {
                DrawWithHeader( Loaded.Poses, Selected.Name );
                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "Faces" ) ) {
                DrawPaths( Loaded.FacePaths, Selected.Name );
                ImGui.EndTabItem();
            }
        }

        protected override string GetName( CharacterRow item ) => item.Name;
    }
}