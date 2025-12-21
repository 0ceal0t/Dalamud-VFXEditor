using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;

namespace VfxEditor.Select.Tabs.Job {
    public class SelectedJob {
        // Race -> Idle, MoveA, MoveB
        public Dictionary<string, Dictionary<string, string>> General;
        // Race -> Start, Loop
        public Dictionary<string, Dictionary<string, string>> Poses;
        // Race -> Auto1, Auto2, ...
        public Dictionary<string, Dictionary<string, string>> AutoAttack;
    }

    public class JobTab : SelectTab<JobRow, SelectedJob> {
        public JobTab( SelectDialog dialog, string name ) : base( dialog, name, "Job" ) { }

        public override void LoadData() {
            foreach( var item in SelectDataUtils.JobAnimationIds ) Items.Add( new JobRow( item.Key, item.Value ) );
        }

        public override void LoadSelection( JobRow item, out SelectedJob loaded ) {
            var general = new Dictionary<string, Dictionary<string, string>>();
            var poses = new Dictionary<string, Dictionary<string, string>>();
            var autoAttack = new Dictionary<string, Dictionary<string, string>>();

            var jobId = item.Id;
            var movementJobId = SelectDataUtils.JobMovementOverride.TryGetValue( item.Name, out var _movement ) ? _movement : jobId;
            var drawJobId = SelectDataUtils.JobDrawOverride.TryGetValue( item.Name, out var _draw ) ? _draw : jobId;
            var autoJobId = SelectDataUtils.JobAutoOverride.TryGetValue( item.Name, out var _auto ) ? _auto : jobId;

            foreach( var race in SelectDataUtils.CharacterRaces ) {
                var skeleton = race.Id;

                // General

                var idlePath = SelectDataUtils.GetSkeletonPath( skeleton, $"{jobId}/resident/idle.pap" );
                var movePathA = SelectDataUtils.GetSkeletonPath( skeleton, $"{movementJobId}/resident/move_a.pap" );
                var movePathB = SelectDataUtils.GetSkeletonPath( skeleton, $"{movementJobId}/resident/move_b.pap" );
                var drawPath = SelectDataUtils.GetSkeletonPath( skeleton, $"{drawJobId}/resident/sub.pap" );

                var raceGeneral = new Dictionary<string, string>();
                if( Dalamud.DataManager.FileExists( idlePath ) ) raceGeneral.Add( "Idle", idlePath );
                if( Dalamud.DataManager.FileExists( movePathA ) ) raceGeneral.Add( "Move A", movePathA );
                if( Dalamud.DataManager.FileExists( movePathB ) ) raceGeneral.Add( "Move B", movePathB );
                if( Dalamud.DataManager.FileExists( drawPath ) ) raceGeneral.Add( "Draw Weapon", drawPath );
                general.Add( race.Name, raceGeneral );

                // Pose

                var start = SelectDataUtils.GetSkeletonPath( skeleton, $"{jobId}/emote/b_pose01_start.pap" );
                var loop = SelectDataUtils.GetSkeletonPath( skeleton, $"{jobId}/emote/b_pose01_loop.pap" );

                if( Dalamud.DataManager.FileExists( start ) && Dalamud.DataManager.FileExists( loop ) ) {
                    poses.Add( race.Name, new Dictionary<string, string>() {
                        { "Start", start },
                        { "Loop", loop }
                    } );
                }

                // Auto

                var autoPaths = new List<string>();

                for( var i = 1; i <= 3; i++ ) {
                    var autoPath = SelectDataUtils.GetSkeletonPath( skeleton, $"{autoJobId}/battle/auto_attack{i}.pap" );
                    var autoShotPath = SelectDataUtils.GetSkeletonPath( skeleton, $"{autoJobId}/battle/auto_attack_shot{i}.pap" );
                    if( Dalamud.DataManager.FileExists( autoPath ) ) autoPaths.Add( autoPath );
                    if( Dalamud.DataManager.FileExists( autoShotPath ) ) autoPaths.Add( autoShotPath );
                }

                var raceAutos = new Dictionary<string, string>();
                for( var i = 0; i < autoPaths.Count; i++ ) {
                    raceAutos.Add( $"Auto-Attack {i + 1}", autoPaths[i] );
                }

                autoAttack.Add( race.Name, raceAutos );
            }

            loaded = new() {
                General = general,
                Poses = poses,
                AutoAttack = autoAttack
            };
        }

        protected override void DrawSelected() {
            using var tabBar = ImRaii.TabBar( "Tabs" );
            if( !tabBar ) return;

            if( ImGui.BeginTabItem( "General" ) ) {
                Dialog.DrawPaths( Loaded.General, Selected.Name, SelectResultType.GameJob );
                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "Poses" ) ) {
                Dialog.DrawPaths( Loaded.Poses, Selected.Name, SelectResultType.GameJob );
                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "Auto-Attack" ) ) {
                Dialog.DrawPaths( Loaded.AutoAttack, Selected.Name, SelectResultType.GameJob );
                ImGui.EndTabItem();
            }
        }
    }
}