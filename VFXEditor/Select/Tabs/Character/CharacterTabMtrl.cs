using ImGuiNET;
using Dalamud.Interface.Utility.Raii;
using System;
using System.Collections.Generic;

namespace VfxEditor.Select.Tabs.Character {
    public class SelectedMtrl {
        public Dictionary<string, Dictionary<string, string>> Faces;
        public Dictionary<string, Dictionary<string, string>> Bodies;
        public Dictionary<string, Dictionary<string, string>> Hairs;
        public Dictionary<string, Dictionary<string, string>> Ears;
        public Dictionary<string, Dictionary<string, string>> Tails;
    }

    public class CharacterTabMtrl : SelectTab<CharacterRow, SelectedMtrl> {
        public CharacterTabMtrl( SelectDialog dialog, string name ) : base( dialog, name, "Character", SelectResultType.GameCharacter ) { }

        // ===== LOADING =====

        public override void LoadData() => CharacterTab.Load( Items );

        public override void LoadSelection( CharacterRow item, out SelectedMtrl loaded ) {
            var options = item.GetOptions();
            loaded = new() {
                Faces = GetDictionary( "Face", item.GetFaceMaterial, options.Face, new[] { "fac_a", "etc_a", "iri_a" } ),
                Bodies = GetDictionary( "Body", item.GetBodyMaterial, options.Body, new[] { "a" } ),
                Hairs = GetDictionary( "Hair", item.GetHairMaterial, options.Hair, new[] { "hir_a", "acc_b" } ),
                Ears = GetDictionary( "Ear", item.GetEarMaterial, options.Ear, new[] { "fac_a", "a" } ),
                Tails = GetDictionary( "Tail", item.GetTailMaterial, options.Tail, new[] { "a" } )
            };
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            using var tabBar = ImRaii.TabBar( "Tabs" );
            if( !tabBar ) return;

            if( ImGui.BeginTabItem( "Faces" ) ) {
                DrawWithHeader( Loaded.Faces, Selected.Name );
                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "Bodies" ) ) {
                DrawWithHeader( Loaded.Bodies, Selected.Name );
                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "Hairs" ) ) {
                DrawWithHeader( Loaded.Hairs, Selected.Name );
                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "Ears" ) ) {
                DrawWithHeader( Loaded.Ears, Selected.Name );
                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "Tails" ) ) {
                DrawWithHeader( Loaded.Tails, Selected.Name );
                ImGui.EndTabItem();
            }
        }

        protected override string GetName( CharacterRow item ) => item.Name;

        private static Dictionary<string, Dictionary<string, string>> GetDictionary( string name, Func<int, string, string> getMaterial, IEnumerable<int> ids, IEnumerable<string> suffixes ) {
            var ret = new Dictionary<string, Dictionary<string, string>>();
            foreach( var id in ids ) {
                var dict = new Dictionary<string, string>();

                foreach( var suffix in suffixes ) {
                    var path = getMaterial( id, suffix );
                    if( !Dalamud.DataManager.FileExists( path ) ) continue;
                    dict[$"Material {dict.Count}"] = path;
                }

                if( dict.Count > 0 ) ret[$"{name} {id}"] = dict;
            }
            return ret;
        }
    }
}
