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

        public SkeletonCharacterTab( SelectDialog dialog, string name, string prefix, string extension ) : base( dialog, name, "Character-Shared" ) {
            Prefix = prefix;
            Extension = extension;
        }

        // ===== LOADING =====

        public override void LoadData() {
            foreach( var item in SelectUtils.RaceAnimationIds ) Items.Add( new( item.Key, item.Value ) );
        }

        public override void LoadSelection( CharacterRow item, out CharacterRowSelected loaded ) {
            var bodyPath = $"chara/human/{item.SkeletonId}/skeleton/base/b0001/{Prefix}_{item.SkeletonId}b0001.{Extension}";

            var facePaths = new Dictionary<string, string>();
            var hairPaths = new Dictionary<string, string>();

            for( var face = 1; face <= SelectUtils.MaxFaces; face++ ) {
                var faceString = $"f{face:D4}";
                facePaths[$"Face {face}"] = $"chara/human/{item.SkeletonId}/skeleton/face/{faceString}/{Prefix}_{item.SkeletonId}{faceString}.{Extension}";
            }

            foreach( var hairId in item.GetHairIds() ) {
                var hairString = $"h{hairId:D4}";
                hairPaths[$"Hair {hairId}"] = $"chara/human/{item.SkeletonId}/skeleton/hair/{hairString}/{Prefix}_{item.SkeletonId}{hairString}.{Extension}";
            }

            loaded = new() {
                BodyPath = bodyPath,
                FacePaths = SelectUtils.FileExistsFilter( facePaths ),
                HairPaths = SelectUtils.FileExistsFilter( hairPaths )
            };
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            Dialog.DrawPath( "Body", Loaded.BodyPath, SelectResultType.GameCharacter, Selected.Name );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );

            using var tabBar = ImRaii.TabBar( "Tabs" );
            if( !tabBar ) return;

            if( ImGui.BeginTabItem( "Hair" ) ) {
                Dialog.DrawPaths( Loaded.HairPaths, SelectResultType.GameCharacter, Selected.Name );
                ImGui.EndTabItem();
            }

            if( ImGui.BeginTabItem( "Faces" ) ) {
                Dialog.DrawPaths( Loaded.FacePaths, SelectResultType.GameCharacter, Selected.Name );
                ImGui.EndTabItem();
            }
        }

        protected override string GetName( CharacterRow item ) => item.Name;
    }
}
