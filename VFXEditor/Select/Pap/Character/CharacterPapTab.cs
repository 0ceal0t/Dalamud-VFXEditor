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

            var idlePath = item.GetPap( "idle" );
            var movePathA = item.GetPap( "move_a" );
            var movePathB = item.GetPap( "move_b" );

            var generalData = new GeneralData() {
                IdlePath = Plugin.DataManager.FileExists( idlePath ) ? idlePath : null,
                MovePathA = Plugin.DataManager.FileExists( movePathA ) ? movePathA : null,
                MovePathB = Plugin.DataManager.FileExists( movePathB ) ? movePathB : null,
            };

            // ChangePoses

            var poseData = new List<PoseData>();
            for( var idx = 1; idx < SelectUtils.MaxChangePoses; idx++ ) {
                var start = item.GetStartPap( idx );
                var loop = item.GetLoopPap( idx );
                if( Plugin.DataManager.FileExists( start ) &&  Plugin.DataManager.FileExists( loop ) ) {
                    poseData.Add( new PoseData() {
                        PoseIndex = idx,
                        Start = start,
                        Loop = loop
                    } );
                }
            }

            loaded = new( poseData, generalData );
        }

        // ===== DRAWING ======

        protected override void DrawSelected( string parentId ) {
            if( ImGui.BeginTabBar( $"{parentId}/Tabs" ) ) {
                if( ImGui.BeginTabItem( $"General{parentId}" ) ) {
                    DrawGeneral( parentId );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( $"Poses{parentId}" ) ) {
                    DrawChangePose( parentId );
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
        }

        private void DrawGeneral( string parentId ) {
            Dialog.DrawPath( $"Idle", Loaded.General.IdlePath, $"{parentId}-Idle", SelectResultType.GameCharacter, $"{Selected.Name} Idle" );
            Dialog.DrawPath( $"Move A", Loaded.General.MovePathA, $"{parentId}-MoveA", SelectResultType.GameCharacter, $"{Selected.Name} Move A" );
            Dialog.DrawPath( $"Move B", Loaded.General.MovePathB, $"{parentId}-MoveB", SelectResultType.GameCharacter, $"{Selected.Name} Move B" );
        }

        private void DrawChangePose( string parentId ) {
            foreach( var pose in Loaded.Poses ) {
                var idx = pose.PoseIndex;
                Dialog.DrawPath( $"Pose #{idx} Start", pose.Start, $"{parentId}-{idx}-Start", SelectResultType.GameCharacter, $"{Selected.Name} #{idx} Start" );
                Dialog.DrawPath( $"Pose #{idx} Loop", pose.Loop, $"{parentId}-{idx}-Loop", SelectResultType.GameCharacter, $"{Selected.Name} #{idx} Loop" );
            }
        }

        protected override string GetName( CharacterRow item ) => item.Name;
    }
}
