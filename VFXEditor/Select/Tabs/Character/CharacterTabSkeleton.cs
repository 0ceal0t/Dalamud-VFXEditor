using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Select.Tabs.Character {
    public class SelectedSkeleton {
        public string BodyPath;
        public Dictionary<string, string> FacePaths;
        public Dictionary<(string, uint), string> HairPaths;
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
            loaded = new() {
                BodyPath = $"chara/human/{item.SkeletonId}/skeleton/base/b0001/{Prefix}_{item.SkeletonId}b0001.{Extension}"
            };

            if( !HairFace ) return;

            loaded.FacePaths = item.Data.FaceOptions
                .Select( face => (face, $"chara/human/{item.SkeletonId}/skeleton/face/f{face:D4}/{Prefix}_{item.SkeletonId}f{face:D4}.{Extension}") )
                .Where( x => Dalamud.DataManager.FileExists( x.Item2 ) )
                .ToDictionary( x => $"Face {x.Item1}", x => x.Item2 );

            loaded.HairPaths = item.Data.HairOptions
                .Select( hair => (hair, $"chara/human/{item.SkeletonId}/skeleton/hair/h{hair:D4}/{Prefix}_{item.SkeletonId}h{hair:D4}.{Extension}") )
                .Where( x => Dalamud.DataManager.FileExists( x.Item2 ) )
                .ToDictionary( x => ($"Hair {x.Item1}", item.Data.HairToIcon.TryGetValue( x.Item1, out var icon ) ? icon : 0), x => x.Item2 );
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