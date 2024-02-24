using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Select.Tabs.Character {
    public class SelectedMtrl {
        public Dictionary<string, Dictionary<string, string>> Faces;
        public Dictionary<string, Dictionary<string, string>> Bodies;
        public Dictionary<(string, uint), Dictionary<string, string>> Hairs;
        public Dictionary<string, Dictionary<string, string>> Ears;
        public Dictionary<string, Dictionary<string, string>> Tails;
    }

    public class CharacterTabMtrl : SelectTab<CharacterRow, SelectedMtrl> {
        public CharacterTabMtrl( SelectDialog dialog, string name ) : base( dialog, name, "Character", SelectResultType.GameCharacter ) { }

        // ===== LOADING =====

        public override void LoadData() => CharacterTab.Load( Items );

        public override void LoadSelection( CharacterRow item, out SelectedMtrl loaded ) {
            loaded = new() {
                Faces = GetPart( "Face", CharacterPart.Face, item, item.Data.FaceOptions, new List<string>() { "fac_a", "etc_a", "iri_a" } ),
                Bodies = GetPart( "Body", CharacterPart.Body, item, item.Data.BodyOptions, new List<string>() { "a" } ),
                Hairs = GetPart( "Hair", CharacterPart.Hair, item, item.Data.HairOptions, new List<string>() { "hir_a", "acc_b" }, item.Data.HairToIcon ),
                Ears = GetPart( "Ear", CharacterPart.Ear, item, item.Data.EarOptions, new List<string>() { "fac_a", "a" } ),
                Tails = GetPart( "Tail", CharacterPart.Tail, item, item.Data.TailOptions, new List<string>() { "a" } )
            };
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            using var tabBar = ImRaii.TabBar( "Tabs" );
            if( !tabBar ) return;

            if( ImGui.BeginTabItem( "Faces" ) ) {
                DrawPaths( Loaded.Faces, Selected.Name );
                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "Bodies" ) ) {
                DrawPaths( Loaded.Bodies, Selected.Name );
                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "Hairs" ) ) {
                DrawPaths( Loaded.Hairs, Selected.Name );
                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "Ears" ) ) {
                DrawPaths( Loaded.Ears, Selected.Name );
                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "Tails" ) ) {
                DrawPaths( Loaded.Tails, Selected.Name );
                ImGui.EndTabItem();
            }
        }

        protected override string GetName( CharacterRow item ) => item.Name;

        private static IEnumerable<(int, Dictionary<string, string>)> IdsToPaths( IEnumerable<int> ids, IEnumerable<string> suffixes, CharacterRow item, CharacterPart part ) =>
            ids.Select( id =>
                (id, suffixes
                .Select( suffix => item.GetMtrl( part, id, suffix ) )
                .Where( path => Dalamud.DataManager.FileExists( path ) )
                .WithIndex()
                .ToDictionary(
                    path => $"Material {path.Index}",
                    path => path.Value
                ))
            )
            .Where( x => x.Item2.Count > 0 );

        private static Dictionary<string, Dictionary<string, string>> GetPart( string name, CharacterPart part, CharacterRow item, IEnumerable<int> ids, IEnumerable<string> suffixes ) =>
            IdsToPaths( ids, suffixes, item, part )
            .ToDictionary( x => $"{name} {x.Item1}", x => x.Item2 );

        private static Dictionary<(string, uint), Dictionary<string, string>> GetPart( string name, CharacterPart part, CharacterRow item, IEnumerable<int> ids, IEnumerable<string> suffixes, Dictionary<int, uint> iconMap ) =>
            IdsToPaths( ids, suffixes, item, part )
            .ToDictionary( x => ($"{name} {x.Item1}", iconMap.TryGetValue( x.Item1, out var icon ) ? icon : 0), x => x.Item2 );
    }
}
