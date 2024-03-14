using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Select.Tabs.Character {
    public class SelectedMdl {
        public List<(string, uint, string)> Faces;
        public List<(string, string)> Bodies;
        public List<(string, uint, string)> Hairs;
        public List<(string, uint, string)> Ears;
        public List<(string, uint, string)> Tails;
    }

    public class CharacterTabMdl : SelectTab<CharacterRow, SelectedMdl> {
        public CharacterTabMdl( SelectDialog dialog, string name ) : base( dialog, name, "Character", SelectResultType.GameCharacter ) { }

        // ===== LOADING =====

        public override void LoadData() => CharacterTab.Load( Items );

        public override void LoadSelection( CharacterRow item, out SelectedMdl loaded ) {
            loaded = new() {
                Faces = GetPart( "Face", CharacterPart.Face, item, item.Data.FaceOptions, item.Data.FaceToIcon ),
                Bodies = GetPart( "Body", CharacterPart.Body, item, item.Data.BodyOptions ),
                Hairs = GetPart( "Hair", CharacterPart.Hair, item, item.Data.HairOptions, item.Data.HairToIcon ),
                Ears = GetPart( "Ear", CharacterPart.Ear, item, item.Data.EarOptions, item.Data.FeatureToIcon ),
                Tails = GetPart( "Tail", CharacterPart.Tail, item, item.Data.TailOptions, item.Data.FeatureToIcon )
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

        private static List<(string, string)> GetPart( string name, CharacterPart part, CharacterRow item, IEnumerable<int> ids ) =>
            ids
            .Select( id => (id, item.GetMdl( part, id )) )
            .Where( x => Dalamud.DataManager.FileExists( x.Item2 ) )
            .Select( x => ($"{name} {x.id}", x.Item2) ).ToList();

        private static List<(string, uint, string)> GetPart( string name, CharacterPart part, CharacterRow item, IEnumerable<int> ids, Dictionary<int, uint> iconMap ) =>
            ids
            .Select( id => (id, item.GetMdl( part, id )) )
            .Where( x => Dalamud.DataManager.FileExists( x.Item2 ) )
            .Select( x => ($"{name} {x.id}", iconMap.TryGetValue( x.id, out var icon ) ? icon : 0, x.Item2) )
            .ToList();
    }
}
