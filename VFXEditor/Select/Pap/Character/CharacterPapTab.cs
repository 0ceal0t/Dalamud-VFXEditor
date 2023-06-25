using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using VfxEditor.Select.Shared.Character;

namespace VfxEditor.Select.Pap.IdlePose {
    public class CharacterRowSelected {
        // Idle, MoveA, MoveB
        public Dictionary<string, string> General;
        // Pose # -> Start, Loop
        public Dictionary<string, Dictionary<string, string>> Poses;
        public Dictionary<string, string> FacePaths;
    }

    public class CharacterPapTab : SelectTab<CharacterRow, CharacterRowSelected> {
        public CharacterPapTab( SelectDialog dialog, string name ) : base( dialog, name, "Character-Shared" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            foreach( var item in SelectUtils.RaceAnimationIds ) Items.Add( new( item.Key, item.Value ) );
        }

        public override void LoadSelection( CharacterRow item, out CharacterRowSelected loaded ) {
            // General

            var general = new Dictionary<string, string>();

            var idlePath = item.GetPap( "idle" );
            var movePathA = item.GetPap( "move_a" );
            var movePathB = item.GetPap( "move_b" );
            if( Plugin.DataManager.FileExists( idlePath ) ) general.Add( "Idle", idlePath );
            if( Plugin.DataManager.FileExists( movePathA ) ) general.Add( "Move A", movePathA );
            if( Plugin.DataManager.FileExists( movePathB ) ) general.Add( "Move B", movePathB );

            // Poses

            var poses = new Dictionary<string, Dictionary<string, string>>();

            for( var i = 1; i <= SelectUtils.MaxChangePoses; i++ ) {
                var start = item.GetStartPap( i );
                var loop = item.GetLoopPap( i );
                if( Plugin.DataManager.FileExists( start ) && Plugin.DataManager.FileExists( loop ) ) {
                    poses.Add( $"Pose {i}", new Dictionary<string, string>() {
                        { "Start", start },
                        { "Loop", loop }
                    } );
                }
            }

            // Faces

            var facePaths = new Dictionary<string, string>();

            for( var face = 1; face < SelectUtils.MaxFaces; face++ ) {
                var faceString = $"f{face:D4}";
                facePaths[$"Face {face}"] = $"chara/human/{item.SkeletonId}/animation/{faceString}/resident/face.pap";
            }

            loaded = new CharacterRowSelected {
                General = general,
                Poses = poses,
                FacePaths = SelectUtils.FileExistsFilter( facePaths ),
            };
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            using var tabBar = ImRaii.TabBar( "Tabs" );
            if( !tabBar ) return;

            if( ImGui.BeginTabItem( "General" ) ) {
                Dialog.DrawPaps( Loaded.General, SelectResultType.GameCharacter, Selected.Name );
                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "Poses" ) ) {
                Dialog.DrawPapsWithHeader( Loaded.Poses, SelectResultType.GameCharacter, Selected.Name );
                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "Faces" ) ) {
                Dialog.DrawPaths( Loaded.FacePaths, SelectResultType.GameCharacter, Selected.Name );
                ImGui.EndTabItem();
            }
        }

        protected override string GetName( CharacterRow item ) => item.Name;
    }
}
