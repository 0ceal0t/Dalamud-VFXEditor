using ImGuiNET;
using Lumina.Data.Files;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Select.Tabs.Items {
    public class SelectedMtrl {
        public bool IsWeapon;
        public Dictionary<string, string> WeaponPaths;
        public Dictionary<string, Dictionary<string, string>> ArmorPaths;
    }

    public class ItemTabMtrl : ItemTab<SelectedMtrl> {
        public ItemTabMtrl( SelectDialog dialog, string name ) : base( dialog, name, "Item-Mtrl", ItemTabFilter.Weapon | ItemTabFilter.SubWeapon | ItemTabFilter.Armor | ItemTabFilter.Acc ) { }

        // ===== LOADING =====

        public override void LoadSelection( ItemRow item, out SelectedMtrl loaded ) {
            loaded = new() {
                IsWeapon = false,
                WeaponPaths = new(),
                ArmorPaths = new(),
            };

            var imcPath = item.ImcPath;
            if( !Dalamud.DataManager.FileExists( imcPath ) ) return;
            try {
                var file = Dalamud.DataManager.GetFile<ImcFile>( imcPath );

                var ids = file.GetParts().Select( x => x.Variants[item.Variant - 1] ).Where( x => x.MaterialId != 0 ).Select( x => ( int )x.MaterialId ).ToList();
                if( ids.Count == 0 ) return;
                var id = ids[0];

                if( item is ItemRowWeapon weapon ) {
                    loaded = new() {
                        IsWeapon = true,
                        WeaponPaths = GetPaths(
                            weapon.GetMtrlPath( id, "a" ),
                            weapon.GetMtrlPath( id, "b" ),
                            weapon.GetMtrlPath( id, "c" ) )
                    };
                }
                else if( item is ItemRowArmor armor ) {
                    var paths = new Dictionary<string, Dictionary<string, string>>();

                    foreach( var race in SelectDataUtils.CharacterRaces ) {
                        paths[race.Name] = GetPaths(
                            armor.GetMtrlPath( id, race.Id, "a" ),
                            armor.GetMtrlPath( id, race.Id, "b" ),
                            armor.GetMtrlPath( id, race.Id, "c" ) );
                    }

                    loaded = new() {
                        IsWeapon = false,
                        ArmorPaths = paths
                    };
                }
            }
            catch( Exception e ) {
                Dalamud.Error( e, "Error loading IMC file " + imcPath );
            }
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            DrawIcon( Selected.Icon );
            ImGui.Text( $"Variant: {Selected.Variant}" );

            if( string.IsNullOrEmpty( Selected.ImcPath ) ) return;

            ImGui.Text( "IMC: " );
            ImGui.SameLine();
            SelectUiUtils.DisplayPath( Selected.ImcPath );

            if( Loaded.IsWeapon ) {
                DrawPaths( Loaded.WeaponPaths, Selected.Name );
            }
            else {
                DrawPaths( Loaded.ArmorPaths, Selected.Name );
            }
        }

        private static Dictionary<string, string> GetPaths( string pathA, string pathB, string pathC ) {
            var paths = new Dictionary<string, string>();
            if( Dalamud.GameFileExists( pathA ) ) paths["A"] = pathA;
            if( Dalamud.GameFileExists( pathB ) ) paths["B"] = pathB;
            if( Dalamud.GameFileExists( pathC ) ) paths["C"] = pathC;
            return paths;
        }
    }
}
