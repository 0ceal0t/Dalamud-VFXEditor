using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using VfxEditor.Select.Shared.Character;

namespace VfxEditor.Select.Shared.Skeleton {
    public class CharacterRowSelected {
        public string BodyPath;
        public Dictionary<string, string> FacePaths;
        public Dictionary<string, string> HairPaths;
    }

    public class SkeletonCharacterTab : SelectTab<CharacterRow, CharacterRowSelected> {
        private readonly string Prefix;
        private readonly string Extension;

        public SkeletonCharacterTab( SelectDialog dialog, string name, string prefix, string extension ) : base( dialog, name, "Character-Shared", SelectResultType.GameCharacter ) {
            Prefix = prefix;
            Extension = extension;
        }

        // ===== LOADING =====

        public override void LoadData() {
            foreach( var item in SelectDataUtils.RaceAnimationIds ) Items.Add( new( item.Key, item.Value ) );
        }

        public override void LoadSelection( CharacterRow item, out CharacterRowSelected loaded ) {
            var bodyPath = $"chara/human/{item.SkeletonId}/skeleton/base/b0001/{Prefix}_{item.SkeletonId}b0001.{Extension}";

            var facePaths = new Dictionary<string, string>();
            var hairPaths = new Dictionary<string, string>();

            foreach( var face in item.GetFaceIds() ) {
                facePaths[$"Face {face}"] = $"chara/human/{item.SkeletonId}/skeleton/face/f{face:D4}/{Prefix}_{item.SkeletonId}f{face:D4}.{Extension}";
            }

            foreach( var hairId in item.GetHairIds() ) {
                hairPaths[$"Hair {hairId}"] = $"chara/human/{item.SkeletonId}/skeleton/hair/h{hairId:D4}/{Prefix}_{item.SkeletonId}h{hairId:D4}.{Extension}";
            }

            loaded = new() {
                BodyPath = bodyPath,
                FacePaths = SelectDataUtils.FileExistsFilter( facePaths ),
                HairPaths = SelectDataUtils.FileExistsFilter( hairPaths )
            };
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            DrawPath( "Body", Loaded.BodyPath, Selected.Name );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );

            using var tabBar = ImRaii.TabBar( "Tabs" );
            if( !tabBar ) return;

            if( ImGui.BeginTabItem( "Hair" ) ) {
                DrawPaths( Loaded.HairPaths, Selected.Name );
                ImGui.EndTabItem();
            }

            if( ImGui.BeginTabItem( "Face" ) ) {
                DrawPaths( Loaded.FacePaths, Selected.Name );
                ImGui.EndTabItem();
            }
        }

        protected override string GetName( CharacterRow item ) => item.Name;
    }
}
