using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace VfxEditor.Select.Tabs.Emotes {
    public class SelectedPapEntry {
        public EmoteRowType Type;
        public Dictionary<string, string> ActionData;
        public Dictionary<string, Dictionary<string, string>> JobData;
        public Dictionary<string, List<(string, uint, string)>> FaceData;
    }

    public class EmoteTabPap : EmoteTab<Dictionary<string, SelectedPapEntry>> {
        public EmoteTabPap( SelectDialog dialog, string name ) : base( dialog, name ) { }

        public override void LoadSelection( EmoteRow item, out Dictionary<string, SelectedPapEntry> loaded ) {
            loaded = [];

            foreach( var (key, type) in item.Items ) {
                if( string.IsNullOrEmpty( key ) ) continue;

                var entry = new SelectedPapEntry() {
                    Type = type
                };

                if( type == EmoteRowType.Normal ) {
                    // bt_common, per race (chara/human/{RACE}/animation/a0001/bt_common/emote/add_yes.pap)
                    entry.ActionData = SelectDataUtils.FileExistsFilter( SelectDataUtils.GetAllSkeletonPaths( $"bt_common/{key}.pap" ) );

                }
                else if( type == EmoteRowType.PerJob ) {
                    // chara/human/c0101/animation/a0001/bt_swd_sld/emote/battle01.pap
                    entry.JobData = SelectDataUtils.JobAnimationIds.ToDictionary(
                        x => x.Key,
                        x => SelectDataUtils.FileExistsFilter( SelectDataUtils.GetAllSkeletonPaths( $"{x.Value}/{key}.pap" ) )
                    );
                }
                else if( type == EmoteRowType.Facial ) {
                    // chara/human/c0101/animation/f0003/resident/face.pap
                    // chara/human/c0101/animation/f0003/resident/smile.pap
                    // chara/human/c0101/animation/f0003/nonresident/angry_cl.pap
                    entry.FaceData = SelectDataUtils.CharacterRaces.ToDictionary( race => race.Name, race =>
                        race.FaceOptions
                            .Select( id => (id, $"chara/human/{race.Id}/animation/f{id:D4}/nonresident/{key}.pap") )
                            .Where( x => Dalamud.DataManager.FileExists( x.Item2 ) )
                            .Select( x => ($"Face {x.id}", race.FaceToIcon.TryGetValue( x.id, out var icon ) ? icon : 0, x.Item2) )
                            .ToList()
                    );
                }

                loaded.Add( key, entry );
            }
        }

        protected override void DrawSelected() {
            ImGui.TextDisabled( Selected.Command );

            using var tabBar = ImRaii.TabBar( "Tabs" );
            if( !tabBar ) return;

            foreach( var (key, paths) in Loaded ) {
                using var tabItem = ImRaii.TabItem( key );
                if( !tabItem ) continue;

                using var _ = ImRaii.PushId( key );
                using var child = ImRaii.Child( "Child", new Vector2( -1 ), false );

                if( paths.Type == EmoteRowType.Normal ) Dialog.DrawPaths( paths.ActionData, $"{Selected.Name} {key}", SelectResultType.GameEmote );
                else if( paths.Type == EmoteRowType.PerJob ) Dialog.DrawPaths( paths.JobData, $"{Selected.Name} {key}", SelectResultType.GameEmote );
                else if( paths.Type == EmoteRowType.Facial ) Dialog.DrawPaths( paths.FaceData, $"{Selected.Name} {key}", SelectResultType.GameEmote );
            }
        }
    }
}