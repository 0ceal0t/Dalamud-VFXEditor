using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Select.Pap.Job {
    public class JobTab : SelectTab<JobRow, JobRowSelected> {
        public JobTab( SelectDialog dialog, string name ) : base( dialog, name, "Job-Pap" ) { }

        public override void LoadData() {
            foreach( var item in SelectUtils.JobAnimationIds ) Items.Add( new JobRow( item.Key, item.Value ) );
        }

        public override void LoadSelection( JobRow item, out JobRowSelected loaded ) {
            var general = new Dictionary<string, GeneralData>();
            var pose = new Dictionary<string, PoseData>();
            var autoAttack = new Dictionary<string, List<string>>();

            var jobId = item.Id;
            var movementJobId = SelectUtils.JobMovementOverride.TryGetValue( item.Name, out var _movement ) ? _movement : jobId;
            var drawJobId = SelectUtils.JobDrawOverride.TryGetValue( item.Name, out var _draw ) ? _draw : jobId;
            var autoJobId = SelectUtils.JobAutoOverride.TryGetValue( item.Name, out var _auto ) ? _auto : jobId;

            foreach( var race in SelectUtils.RaceAnimationIds ) {
                var skeleton = race.Value.SkeletonId;

                // General

                var idlePath = SelectUtils.GetSkeletonPath( skeleton, $"{jobId}/resident/idle.pap" );
                var movePathA = SelectUtils.GetSkeletonPath( skeleton, $"{movementJobId}/resident/move_a.pap" );
                var movePathB = SelectUtils.GetSkeletonPath( skeleton, $"{movementJobId}/resident/move_b.pap" );
                var drawPath = SelectUtils.GetSkeletonPath( skeleton, $"{drawJobId}/resident/sub.pap" );

                var generalData = new GeneralData() {
                    IdlePath = Plugin.DataManager.FileExists( idlePath ) ? idlePath : null,
                    MovePathA = Plugin.DataManager.FileExists( movePathA ) ? movePathA : null,
                    MovePathB = Plugin.DataManager.FileExists( movePathB ) ? movePathB : null,
                    DrawWeaponPath = Plugin.DataManager.FileExists( drawPath ) ? drawPath : null,
                };
                if( generalData.HasData ) general.Add( race.Key, generalData );

                // Pose

                var start = SelectUtils.GetSkeletonPath( skeleton, $"{jobId}/emote/b_pose01_start.pap" );
                var loop = SelectUtils.GetSkeletonPath( skeleton, $"{jobId}/emote/b_pose01_loop.pap" );

                if( Plugin.DataManager.FileExists( start ) && Plugin.DataManager.FileExists( loop ) ) {
                    pose.Add( race.Key, new PoseData() {
                        Start = start,
                        Loop = loop
                    } );
                }

                // Auto

                var autoPaths = new List<string>();

                for( var idx = 1; idx <= 3; idx++ ) {
                    var autoPath = SelectUtils.GetSkeletonPath( skeleton, $"{autoJobId}/battle/auto_attack{idx}.pap" );
                    var autoShotPath = SelectUtils.GetSkeletonPath( skeleton, $"{autoJobId}/battle/auto_attack_shot{idx}.pap" );

                    if( Plugin.DataManager.FileExists( autoPath ) ) autoPaths.Add( autoPath );
                    if( Plugin.DataManager.FileExists( autoShotPath ) ) autoPaths.Add( autoShotPath );
                }

                if( autoPaths.Count > 0 ) autoAttack.Add( race.Key, autoPaths );
            }

            loaded = new JobRowSelected( general, pose, autoAttack );
        }

        protected override void DrawSelected( string parentId ) {
            if( ImGui.BeginTabBar( $"{parentId}/Tabs" ) ) {
                if( ImGui.BeginTabItem( $"General{parentId}" ) ) {
                    DrawGeneral( $"{parentId}/General" );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( $"Poses{parentId}" ) ) {
                    DrawChangePose( $"{parentId}/Pose" );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( $"Auto-Attack{parentId}" ) ) {
                    DrawAutoAttack( $"{parentId}/Auto" );
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
        }

        private void DrawGeneral( string parentId ) {
            foreach( var general in Loaded.General ) {
                var name = general.Key;
                var value = general.Value;

                if( ImGui.CollapsingHeader( $"{name}{parentId}" ) ) {
                    ImGui.Indent();

                    Dialog.DrawPath( $"Idle", value.IdlePath, $"{parentId}-{name}-Idle", SelectResultType.GameJob, $"{Selected.Name} Idle ({name})" );
                    Dialog.DrawPath( $"Move A", value.MovePathA, $"{parentId}-{name}-MoveA", SelectResultType.GameJob, $"{Selected.Name} Move A ({name})" );
                    Dialog.DrawPath( $"Move B", value.MovePathB, $"{parentId}-{name}-MoveB", SelectResultType.GameJob, $"{Selected.Name} Move B ({name})" );
                    Dialog.DrawPath( $"Draw Weapon", value.DrawWeaponPath, $"{parentId}-{name}-Draw", SelectResultType.GameJob, $"{Selected.Name} Draw ({name})" );

                    ImGui.Unindent();
                }
            }
        }

        private void DrawChangePose( string parentId ) {
            foreach( var pose in Loaded.Pose ) {
                var name = pose.Key;
                var value = pose.Value;

                if( ImGui.CollapsingHeader( $"{name}{parentId}" ) ) {
                    ImGui.Indent();

                    Dialog.DrawPath( $"Start", value.Start, $"{parentId}-{name}-Start", SelectResultType.GameJob, $"{Selected.Name} Start ({name})" );
                    Dialog.DrawPath( $"Loop", value.Loop, $"{parentId}-{name}-Loop", SelectResultType.GameJob, $"{Selected.Name} Loop ({name})" );

                    ImGui.Unindent();
                }
            }
        }

        private void DrawAutoAttack( string parentId ) {
            foreach( var autoAttack in Loaded.AutoAttack ) {
                var name = autoAttack.Key;
                var value = autoAttack.Value;

                if( ImGui.CollapsingHeader( $"{name}{parentId}" ) ) {
                    ImGui.Indent();

                    for( var i = 0; i < value.Count; i++ ) {
                        Dialog.DrawPath( $"Auto-Attack #{i+1}", value[i], $"{parentId}-{name}-{i}", SelectResultType.GameJob, $"{Selected.Name} Auto #{i+1} ({name})" );
                    }

                    ImGui.Unindent();
                }
            }
        }

        protected override string GetName( JobRow item ) => item.Name;
    }
}
