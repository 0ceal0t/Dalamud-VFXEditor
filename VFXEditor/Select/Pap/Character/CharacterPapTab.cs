using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.Select.Pap.Character;
using VfxEditor.Select.Shared.Character;

namespace VfxEditor.Select.Pap.IdlePose {
    public class CharacterPapTab : SelectTab<CharacterRow, CharacterRowSelected> {
        public CharacterPapTab( SelectDialog dialog, string name ) : base( dialog, name, "Character-Shared" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            foreach( var item in SelectUtils.RaceAnimationIds ) Items.Add( new( item.Key, item.Value.SkeletonId ) );
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

            loaded = new( general, poses );
        }

        // ===== DRAWING ======

        protected override void DrawSelected( string parentId ) {
            if( ImGui.BeginTabBar( $"{parentId}/Tabs" ) ) {
                if( ImGui.BeginTabItem( $"General{parentId}" ) ) {
                    Dialog.DrawPaps( Loaded.General, SelectResultType.GameCharacter, Selected.Name, parentId );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( $"Poses{parentId}" ) ) {
                    Dialog.DrawPapsWithHeader( Loaded.Poses, SelectResultType.GameCharacter, Selected.Name, parentId );
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
        }

        protected override string GetName( CharacterRow item ) => item.Name;
    }
}
