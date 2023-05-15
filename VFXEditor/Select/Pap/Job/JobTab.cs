using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;

namespace VfxEditor.Select.Pap.Job {
    public class JobTab : SelectTab<JobRow, JobRowSelected> {
        public JobTab( SelectDialog dialog, string name ) : base( dialog, name, "Job-Pap" ) { }

        public override void LoadData() {
            foreach( var item in SelectUtils.JobAnimationIds ) Items.Add( new JobRow( item.Key, item.Value ) );
        }

        public override void LoadSelection( JobRow item, out JobRowSelected loaded ) {
            var general = new Dictionary<string, Dictionary<string, string>>();
            var poses = new Dictionary<string, Dictionary<string, string>>();
            var autoAttack = new Dictionary<string, Dictionary<string, string>>();

            var jobId = item.Id;
            var movementJobId = SelectUtils.JobMovementOverride.TryGetValue( item.Name, out var _movement ) ? _movement : jobId;
            var drawJobId = SelectUtils.JobDrawOverride.TryGetValue( item.Name, out var _draw ) ? _draw : jobId;
            var autoJobId = SelectUtils.JobAutoOverride.TryGetValue( item.Name, out var _auto ) ? _auto : jobId;

            foreach( var (raceName, raceData) in SelectUtils.RaceAnimationIds ) {
                var skeleton = raceData.SkeletonId;

                // General

                var idlePath = SelectUtils.GetSkeletonPath( skeleton, $"{jobId}/resident/idle.pap" );
                var movePathA = SelectUtils.GetSkeletonPath( skeleton, $"{movementJobId}/resident/move_a.pap" );
                var movePathB = SelectUtils.GetSkeletonPath( skeleton, $"{movementJobId}/resident/move_b.pap" );
                var drawPath = SelectUtils.GetSkeletonPath( skeleton, $"{drawJobId}/resident/sub.pap" );

                var raceGeneral = new Dictionary<string, string>();
                if( Plugin.DataManager.FileExists( idlePath ) ) raceGeneral.Add( "Idle", idlePath );
                if( Plugin.DataManager.FileExists( movePathA ) ) raceGeneral.Add( "Move A", movePathA );
                if( Plugin.DataManager.FileExists( movePathB ) ) raceGeneral.Add( "Move B", movePathB );
                if( Plugin.DataManager.FileExists( drawPath ) ) raceGeneral.Add( "Draw Weapon", drawPath );
                general.Add( raceName, raceGeneral );

                // Pose

                var start = SelectUtils.GetSkeletonPath( skeleton, $"{jobId}/emote/b_pose01_start.pap" );
                var loop = SelectUtils.GetSkeletonPath( skeleton, $"{jobId}/emote/b_pose01_loop.pap" );

                if( Plugin.DataManager.FileExists( start ) && Plugin.DataManager.FileExists( loop ) ) {
                    poses.Add( raceName, new Dictionary<string, string>() {
                        { "Start", start },
                        { "Loop", loop }
                    } );
                }

                // Auto

                var autoPaths = new List<string>();

                for( var i = 1; i <= 3; i++ ) {
                    var autoPath = SelectUtils.GetSkeletonPath( skeleton, $"{autoJobId}/battle/auto_attack{i}.pap" );
                    var autoShotPath = SelectUtils.GetSkeletonPath( skeleton, $"{autoJobId}/battle/auto_attack_shot{i}.pap" );
                    if( Plugin.DataManager.FileExists( autoPath ) ) autoPaths.Add( autoPath );
                    if( Plugin.DataManager.FileExists( autoShotPath ) ) autoPaths.Add( autoShotPath );
                }

                var raceAutos = new Dictionary<string, string>();
                for( var i = 0; i < autoPaths.Count; i++ ) {
                    raceAutos.Add( $"Auto-Attack {i + 1}", autoPaths[i] );
                }

                autoAttack.Add( raceName, raceAutos );
            }

            loaded = new JobRowSelected( general, poses, autoAttack );
        }

        protected override void DrawSelected() {
            using var tabBar = ImRaii.TabBar( "Tabs" );
            if( !tabBar ) return;

            if( ImGui.BeginTabItem( "General" ) ) {
                Dialog.DrawPapsWithHeader( Loaded.General, SelectResultType.GameJob, Selected.Name );
                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "Poses" ) ) {
                Dialog.DrawPapsWithHeader( Loaded.Poses, SelectResultType.GameJob, Selected.Name );
                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "Auto-Attack" ) ) {
                Dialog.DrawPapsWithHeader( Loaded.AutoAttack, SelectResultType.GameJob, Selected.Name );
                ImGui.EndTabItem();
            }
        }

        protected override string GetName( JobRow item ) => item.Name;
    }
}
