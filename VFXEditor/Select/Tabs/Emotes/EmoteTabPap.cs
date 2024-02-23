using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using System.Numerics;

namespace VfxEditor.Select.Tabs.Emotes {
    public class EmoteTabPap : EmoteTab<Dictionary<string, Dictionary<string, Dictionary<string, string>>>> {
        public EmoteTabPap( SelectDialog dialog, string name ) : base( dialog, name ) { }

        public override void LoadSelection( EmoteRow item, out Dictionary<string, Dictionary<string, Dictionary<string, string>>> loaded ) {
            loaded = [];

            foreach( var papFile in item.Items ) {
                var key = papFile.Item1;
                var papDict = new Dictionary<string, Dictionary<string, string>>();

                if( papFile.Item2 == EmoteRowType.Normal ) {
                    // bt_common, per race (chara/human/{RACE}/animation/a0001/bt_common/emote/add_yes.pap)
                    papDict.Add( "Action", SelectDataUtils.FileExistsFilter( SelectDataUtils.GetAllSkeletonPaths( $"bt_common/{key}.pap" ) ) ); // just a dummy node

                }
                else if( papFile.Item2 == EmoteRowType.PerJob ) {
                    // chara/human/c0101/animation/a0001/bt_swd_sld/emote/battle01.pap
                    papDict = SelectDataUtils.GetAllJobPaps( key );
                }
                else if( papFile.Item2 == EmoteRowType.Facial ) {
                    // chara/human/c0101/animation/f0003/resident/face.pap
                    // chara/human/c0101/animation/f0003/resident/smile.pap
                    // chara/human/c0101/animation/f0003/nonresident/angry_cl.pap
                    papDict = SelectDataUtils.GetAllFacePaps( key );
                }

                loaded.Add( key, papDict );
            }
        }

        protected override void DrawSelected() {
            DrawIcon( Selected.Icon );
            ImGui.TextDisabled( Selected.Command );

            using var tabBar = ImRaii.TabBar( "Tabs" );
            if( !tabBar ) return;

            foreach( var (subAction, subActionPaths) in Loaded ) {
                using var tabItem = ImRaii.TabItem( subAction );
                if( !tabItem ) continue;

                using var _ = ImRaii.PushId( subAction );
                using var child = ImRaii.Child( "Child", new Vector2( -1 ), false );

                if( subActionPaths.TryGetValue( "Action", out var actionPaths ) ) {
                    DrawPaths( actionPaths, $"{Selected.Name} {subAction}" );
                }
                else {
                    DrawPaths( subActionPaths, $"{Selected.Name} {subAction}" );
                }
            }
        }
    }
}