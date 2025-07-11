using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Select.Tabs.Character {
    public class SelectedPap {
        // Idle, MoveA, MoveB
        public Dictionary<string, string> General;

        // Pose # -> Start, Loop
        public Dictionary<string, Dictionary<string, string>> Poses;

        public List<(string, uint, string)> FacePaths;

        public string GroundStart;
        public string Jmn;
        public string ChairStart;
        public string Sit;
        public Dictionary<string, Dictionary<string, string>> GroundSitPoses;
        public Dictionary<string, Dictionary<string, string>> ChairSitPoses;
        public Dictionary<string, Dictionary<string, string>> OrnamentPoses;
    }

    public class CharacterTabPap : SelectTab<CharacterRow, SelectedPap> {
        public CharacterTabPap( SelectDialog dialog, string name ) : base( dialog, name, "Character" ) { }

        // ===== LOADING =====

        public override void LoadData() => CharacterTab.Load( Items );

        public override void LoadSelection( CharacterRow item, out SelectedPap loaded ) {
            var general = new Dictionary<string, string>();
            var idlePath = item.GetPap( "resident/idle" );
            var movePathA = item.GetPap( "resident/move_a" );
            var movePathB = item.GetPap( "resident/move_b" );
            if( Dalamud.DataManager.FileExists( idlePath ) ) general.Add( "Idle", idlePath );
            if( Dalamud.DataManager.FileExists( movePathA ) ) general.Add( "Move A", movePathA );
            if( Dalamud.DataManager.FileExists( movePathB ) ) general.Add( "Move B", movePathB );

            var poses = new Dictionary<string, Dictionary<string, string>>();
            for( var i = 1; i <= SelectDataUtils.MaxChangePoses; i++ ) {
                var start = item.GetStartPap( i, "" );
                var loop = item.GetLoopPap( i, "" );
                if( Dalamud.DataManager.FileExists( start ) && Dalamud.DataManager.FileExists( loop ) ) {
                    poses.Add( $"Pose {i}", new Dictionary<string, string>() {
                        { "Start", start },
                        { "Loop", loop }
                    } );
                }
            }

            var groundSitPoses = new Dictionary<string, Dictionary<string, string>>();
            for( var i = 1; i <= SelectDataUtils.MaxChangePoses; i++ ) {
                var start = item.GetStartPap( i, "j_" );
                var loop = item.GetLoopPap( i, "j_" );
                if( Dalamud.DataManager.FileExists( start ) && Dalamud.DataManager.FileExists( loop ) ) {
                    groundSitPoses.Add( $"Ground Sit Pose {i}", new Dictionary<string, string>() {
                        { "Start", start },
                        { "Loop", loop }
                    } );
                }
            }

            var chairSitPoses = new Dictionary<string, Dictionary<string, string>>();
            for( var i = 1; i <= SelectDataUtils.MaxChangePoses; i++ ) {
                var start = item.GetStartPap( i, "s_" );
                var loop = item.GetLoopPap( i, "s_" );
                if( Dalamud.DataManager.FileExists( start ) && Dalamud.DataManager.FileExists( loop ) ) {
                    chairSitPoses.Add( $"Chair Sit Pose {i}", new Dictionary<string, string>() {
                        { "Start", start },
                        { "Loop", loop }
                    } );
                }
            }

            var ornamentPoses = new Dictionary<string, Dictionary<string, string>>();
            for( var i = 1; i <= SelectDataUtils.MaxChangePoses; i++ ) {
                var start = item.GetOrnamentStartPap( i, "onm_" );
                var loop = item.GetOrnamentLoopPap( i, "onm_" );
                if( Dalamud.DataManager.FileExists( start ) && Dalamud.DataManager.FileExists( loop ) ) {
                    ornamentPoses.Add( $"Fashion Accessory Pose {i}", new Dictionary<string, string>() {
                        { "Start", start },
                        { "Loop", loop }
                    } );
                }
            }

            var jmn = item.GetPap( "emote/jmn" );
            var groundStart = item.GetPap( "event_base/event_base_ground_start" );
            var sit = item.GetPap( "emote/sit" );
            var chairStart = item.GetPap( "event_base/event_base_chair_start" );

            var facePaths = item.Data.FaceOptions
                .Select( id => (id, $"chara/human/{item.SkeletonId}/animation/f{id:D4}/resident/face.pap") )
                .Where( x => Dalamud.DataManager.FileExists( x.Item2 ) )
                .Select( x => ($"Face {x.id}", item.Data.FaceToIcon.TryGetValue( x.id, out var icon ) ? icon : 0, x.Item2) )
                .ToList();

            loaded = new SelectedPap {
                General = general,
                Poses = poses,
                GroundSitPoses = groundSitPoses,
                ChairSitPoses = chairSitPoses,
                OrnamentPoses = ornamentPoses,
                FacePaths = facePaths,
                Jmn = Dalamud.DataManager.FileExists( jmn ) ? jmn : null,
                GroundStart = Dalamud.DataManager.FileExists( groundStart ) ? groundStart : null,
                Sit = Dalamud.DataManager.FileExists( sit ) ? sit : null,
                ChairStart = Dalamud.DataManager.FileExists( chairStart ) ? chairStart : null,
            };
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            using var tabBar = ImRaii.TabBar( "Tabs" );
            if( !tabBar ) return;

            if( ImGui.BeginTabItem( "General" ) ) {
                Dialog.DrawPaths( Loaded.General, Selected.Name, SelectResultType.GameCharacter );
                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "Poses" ) ) {
                Dialog.DrawPaths( Loaded.Poses, Selected.Name, SelectResultType.GameCharacter );
                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "Ground Sit" ) ) {
                Dialog.DrawPaths( new Dictionary<string, string>() {
                    { "Ground Start", Loaded.GroundStart },
                    { "Ground Sit Default", Loaded.Jmn },
                }, Selected.Name, SelectResultType.GameCharacter );

                ImGui.Separator();
                Dialog.DrawPaths( Loaded.GroundSitPoses, Selected.Name, SelectResultType.GameCharacter );

                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "Chair Sit" ) ) {
                Dialog.DrawPaths( new Dictionary<string, string>() {
                    { "Chair Start", Loaded.ChairStart },
                    { "Chair Sit Default", Loaded.Sit },
                }, Selected.Name, SelectResultType.GameCharacter );

                ImGui.Separator();
                Dialog.DrawPaths( Loaded.ChairSitPoses, Selected.Name, SelectResultType.GameCharacter );

                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "Fashion Accessory" ) )
            {
                Dialog.DrawPaths( Loaded.OrnamentPoses, Selected.Name, SelectResultType.GameCharacter );

                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "Faces" ) ) {
                Dialog.DrawPaths( Loaded.FacePaths, Selected.Name, SelectResultType.GameCharacter );
                ImGui.EndTabItem();
            }
        }
    }
}