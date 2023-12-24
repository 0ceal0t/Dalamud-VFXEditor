using ImGuiNET;
using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;

namespace VfxEditor.Select.Tabs.Character {
    public class SelectedSkeleton {
        public string BodyPath;
        public Dictionary<string, string> FacePaths;
        public Dictionary<string, string> HairPaths;
    }

    public class CharacterTabSkeleton : SelectTab<CharacterRow, SelectedSkeleton> {
        private readonly string Prefix;
        private readonly string Extension;
        private readonly bool HairFace;

        public CharacterTabSkeleton( SelectDialog dialog, string name, string prefix, string extension, bool hairFace ) : base( dialog, name, "Character", SelectResultType.GameCharacter ) {
            Prefix = prefix;
            Extension = extension;
            HairFace = hairFace;
        }

        // ===== LOADING =====

        public override void LoadData() => CharacterTab.Load( Items );

        public override void LoadSelection( CharacterRow item, out SelectedSkeleton loaded ) {
            var bodyPath = $"chara/human/{item.SkeletonId}/skeleton/base/b0001/{Prefix}_{item.SkeletonId}b0001.{Extension}";

            loaded = new() {
                BodyPath = bodyPath
            };

            if( !HairFace ) return;

            var facePaths = new Dictionary<string, string>();
            var hairPaths = new Dictionary<string, string>();

            foreach( var face in item.GetOptions().Face ) {
                facePaths[$"Face {face}"] = $"chara/human/{item.SkeletonId}/skeleton/face/f{face:D4}/{Prefix}_{item.SkeletonId}f{face:D4}.{Extension}";
            }

            foreach( var hair in item.GetOptions().Hair ) {
                hairPaths[$"Hair {hair}"] = $"chara/human/{item.SkeletonId}/skeleton/hair/h{hair:D4}/{Prefix}_{item.SkeletonId}h{hair:D4}.{Extension}";
            }

            loaded.FacePaths = SelectDataUtils.FileExistsFilter( facePaths );
            loaded.HairPaths = SelectDataUtils.FileExistsFilter( hairPaths );
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            DrawPath( "Body", Loaded.BodyPath, Selected.Name );

            if( !HairFace ) return;

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