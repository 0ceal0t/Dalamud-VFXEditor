using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Select.Tabs.Character {
    public class SelectedSkeleton {
        public string BodyPath;
        public List<(string, uint, string)> FacePaths;
        public List<(string, uint, string)> HairPaths;
    }

    public class CharacterTabSkeleton : SelectTab<CharacterRow, SelectedSkeleton> {
        private readonly string Prefix;
        private readonly string Extension;
        private readonly bool HairFace;

        public CharacterTabSkeleton( SelectDialog dialog, string name, string prefix, string extension, bool hairFace ) : base( dialog, name, "Character" ) {
            Prefix = prefix;
            Extension = extension;
            HairFace = hairFace;
        }

        // ===== LOADING =====

        public override void LoadData() => CharacterTab.Load( Items );

        public override void LoadSelection( CharacterRow item, out SelectedSkeleton loaded ) {
            loaded = new() {
                BodyPath = $"chara/human/{item.SkeletonId}/skeleton/base/b0001/{Prefix}_{item.SkeletonId}b0001.{Extension}"
            };

            if( !HairFace ) return;

            loaded.FacePaths = GetPaths( item, item.Data.FaceOptions, "face", "Face", item.Data.FaceToIcon );
            loaded.HairPaths = GetPaths( item, item.Data.HairOptions, "hair", "Hair", item.Data.HairToIcon );
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            if( Dalamud.DataManager.FileExists( Loaded.BodyPath ) )
                Dialog.DrawPaths( Loaded.BodyPath, $"{Selected.Name} Body", SelectResultType.GameCharacter );

            if( !HairFace ) return;

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );

            using var tabBar = ImRaii.TabBar( "Tabs" );
            if( !tabBar ) return;

            if( ImGui.BeginTabItem( "Hair" ) ) {
                Dialog.DrawPaths( Loaded.HairPaths, Selected.Name, SelectResultType.GameCharacter );
                ImGui.EndTabItem();
            }

            if( ImGui.BeginTabItem( "Face" ) ) {
                Dialog.DrawPaths( Loaded.FacePaths, Selected.Name, SelectResultType.GameCharacter );
                ImGui.EndTabItem();
            }
        }

        private List<(string, uint, string)> GetPaths( CharacterRow item, IEnumerable<int> ids, string part, string name, Dictionary<int, uint> iconMap ) {
            return ids
                .Select( id => (id, $"chara/human/{item.SkeletonId}/skeleton/{part}/{part[0]}{id:D4}/{Prefix}_{item.SkeletonId}{part[0]}{id:D4}.{Extension}") )
                .Where( x => Dalamud.DataManager.FileExists( x.Item2 ) )
                .Select( x => ($"{name} {x.id}", iconMap.TryGetValue( x.id, out var icon ) ? icon : 0, x.Item2) )
                .ToList();
        }
    }
}