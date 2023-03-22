using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace VfxEditor.Select.Pap.Emote {
    public class EmoteTab : SelectTab<EmoteRow, EmoteRowSelected> {
        public EmoteTab( SelectDialog dialog, string name ) : base( dialog, name, "Pap-Emote" ) { }

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
                    papDict.Add( "Action", SelectUtils.FileExistsFilter( SelectUtils.GetAllSkeletonPaths( $"bt_common/{key}.pap" ) ) ); // just a dummy node

                }
                else if( papFile.Type == EmoteRowType.PerJob ) {
                    // chara/human/c0101/animation/a0001/bt_swd_sld/emote/battle01.pap
                    papDict = SelectUtils.GetAllJobPaths( key );
                }
                else if( papFile.Type == EmoteRowType.Facial ) {
                    // chara/human/c0101/animation/f0003/resident/face.pap
                    // chara/human/c0101/animation/f0003/resident/smile.pap
                    // chara/human/c0101/animation/f0003/nonresident/angry_cl.pap
                    papDict = SelectUtils.GetAllFacePaths( key );
                }

                allPaps.Add( key, papDict );
            }

            loaded = new EmoteRowSelected( allPaps );
        }

        // ===== DRAWING ======

        protected override void OnSelect() => LoadIcon( Selected.Icon );

        protected override void DrawSelected( string parentId ) {
            SelectTabUtils.DrawIcon( Icon );

            ImGui.BeginTabBar( "EmotePapTabs" );
            foreach( var action in Loaded.AllPaps ) {
                if( !ImGui.BeginTabItem( $"{action.Key}{parentId}/Emote" ) ) continue;
                ImGui.BeginChild( "EmotePapChild", new Vector2( -1 ), false );

                if( action.Value.TryGetValue( "Action", out var actionDict ) ) Dialog.DrawPapDict( actionDict, "", $"{Selected.Name} {action.Key}", parentId );
                else {
                    foreach( var subItem in action.Value ) {
                        if( subItem.Value.Count == 0 ) continue;
                        if( ImGui.CollapsingHeader( subItem.Key ) ) {
                            ImGui.Indent();
                            Dialog.DrawPapDict( subItem.Value, "", $"{Selected.Name} {action.Key} {subItem.Key}", parentId );
                            ImGui.Unindent();
                        }
                    }
                }

                ImGui.EndChild();
                ImGui.EndTabItem();
            }
            ImGui.EndTabBar();
        }

        protected override string GetName( EmoteRow item ) => item.Name;
    }
}
