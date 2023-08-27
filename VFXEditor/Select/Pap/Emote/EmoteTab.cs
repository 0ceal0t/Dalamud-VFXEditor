using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace VfxEditor.Select.Pap.Emote {
    public class EmoteRowSelected {
        public Dictionary<string, Dictionary<string, Dictionary<string, string>>> AllPaps;
    }

    public class EmoteTab : SelectTab<EmoteRow, EmoteRowSelected> {
        public EmoteTab( SelectDialog dialog, string name ) : base( dialog, name, "Pap-Emote", SelectResultType.GameEmote ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Emote>().Where( x => !string.IsNullOrEmpty( x.Name ) );
            foreach( var item in sheet ) Items.Add( new EmoteRow( item ) );
        }

        public override void LoadSelection( EmoteRow item, out EmoteRowSelected loaded ) {
            var allPaps = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

            foreach( var papFile in item.PapFiles ) {
                var key = papFile.Key;
                var papDict = new Dictionary<string, Dictionary<string, string>>();

                if( papFile.Type == EmoteRowType.Normal ) {
                    // bt_common, per race (chara/human/{RACE}/animation/a0001/bt_common/emote/add_yes.pap)
                    papDict.Add( "Action", SelectDataUtils.FileExistsFilter( SelectDataUtils.GetAllSkeletonPaths( $"bt_common/{key}.pap" ) ) ); // just a dummy node

                }
                else if( papFile.Type == EmoteRowType.PerJob ) {
                    // chara/human/c0101/animation/a0001/bt_swd_sld/emote/battle01.pap
                    papDict = SelectDataUtils.GetAllJobPaps( key );
                }
                else if( papFile.Type == EmoteRowType.Facial ) {
                    // chara/human/c0101/animation/f0003/resident/face.pap
                    // chara/human/c0101/animation/f0003/resident/smile.pap
                    // chara/human/c0101/animation/f0003/nonresident/angry_cl.pap
                    papDict = SelectDataUtils.GetAllFacePaps( key );
                }

                allPaps.Add( key, papDict );
            }

            loaded = new EmoteRowSelected {
                AllPaps = allPaps
            };
        }

        // ===== DRAWING ======

        protected override void OnSelect() => LoadIcon( Selected.Icon );

        protected override void DrawSelected() {
            SelectUiUtils.DrawIcon( Icon );
            ImGui.TextDisabled( Selected.Command );

            using var tabBar = ImRaii.TabBar( "Tabs" );
            if( !tabBar ) return;

            foreach( var (subAction, subActionPaths) in Loaded.AllPaps ) {
                using var tabItem = ImRaii.TabItem( subAction );
                if( !tabItem ) continue;

                using var _ = ImRaii.PushId( subAction );
                using var child = ImRaii.Child( "Child", new Vector2( -1 ), false );

                if( subActionPaths.TryGetValue( "Action", out var actionPaths ) ) {
                    DrawPaps( actionPaths, $"{Selected.Name} {subAction}" );
                }
                else {
                    DrawPapsWithHeader( subActionPaths, $"{Selected.Name} {subAction}" );
                }
            }
        }

        protected override string GetName( EmoteRow item ) => item.Name;

        protected override bool CheckMatch( EmoteRow item, string searchInput ) => base.CheckMatch( item, SearchInput ) || SelectUiUtils.Matches( item.Command, searchInput );
    }
}
