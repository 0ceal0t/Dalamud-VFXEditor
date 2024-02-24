using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Select.Tabs.Character {
    public class SelectedMdl {
        public Dictionary<string, string> Faces;
        public Dictionary<string, string> Bodies;
        public Dictionary<(string, uint), string> Hairs;
        public Dictionary<string, string> Ears;
        public Dictionary<string, string> Tails;
    }

    public class CharacterTabMdl : SelectTab<CharacterRow, SelectedMdl> {
        public CharacterTabMdl( SelectDialog dialog, string name ) : base( dialog, name, "Character", SelectResultType.GameCharacter ) { }

        // ===== LOADING =====

        public override void LoadData() => CharacterTab.Load( Items );

        public override void LoadSelection( CharacterRow item, out SelectedMdl loaded ) {
            loaded = new() {
                Faces = GetPart( "Face", CharacterPart.Face, item, item.Data.FaceOptions ),
                Bodies = GetPart( "Body", CharacterPart.Body, item, item.Data.BodyOptions ),
                Hairs = GetPart( "Hair", CharacterPart.Hair, item, item.Data.HairOptions, item.Data.HairToIcon ),
                Ears = GetPart( "Ear", CharacterPart.Ear, item, item.Data.EarOptions ),
                Tails = GetPart( "Tail", CharacterPart.Tail, item, item.Data.TailOptions )
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

        private static Dictionary<string, string> GetPart( string name, CharacterPart part, CharacterRow item, IEnumerable<int> ids ) =>
            ids
            .Select( id => (id, item.GetMdl( part, id )) )
            .Where( x => Dalamud.DataManager.FileExists( x.Item2 ) )
            .ToDictionary( x => $"{name} {x.id}", x => x.Item2 );

        private static Dictionary<(string, uint), string> GetPart( string name, CharacterPart part, CharacterRow item, IEnumerable<int> ids, Dictionary<int, uint> iconMap ) =>
            ids
            .Select( id => (id, item.GetMdl( part, id )) )
            .Where( x => Dalamud.DataManager.FileExists( x.Item2 ) )
            .ToDictionary( x => ($"{name} {x.id}", iconMap.TryGetValue( x.id, out var icon ) ? icon : 0), x => x.Item2 );
    }
}
