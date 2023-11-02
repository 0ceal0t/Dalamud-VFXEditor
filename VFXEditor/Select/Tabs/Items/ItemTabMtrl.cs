using ImGuiNET;
using Lumina.Data.Files;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Select.Tabs.Items {
    public class ItemSelectedMtrl {
        public bool IsWeapon;
        public string ImcPath;
        public Dictionary<string, string> WeaponPaths;
        public Dictionary<string, Dictionary<string, string>> ArmorPaths;
    }

    public class ItemTabMtrl : ItemTab<ItemSelectedMtrl> {
        public ItemTabMtrl( SelectDialog dialog, string name ) : base( dialog, name, "Item-Mtrl", ItemTabFilter.Weapon | ItemTabFilter.SubWeapon | ItemTabFilter.Armor | ItemTabFilter.Acc ) { }

        // ===== LOADING =====

        public override void LoadSelection( ItemRow item, out ItemSelectedMtrl loaded ) {
            loaded = new() {
                ImcPath = null,
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
                        ImcPath = imcPath,
                        IsWeapon = true,
                        WeaponPaths = GetPaths( weapon.GetMtrlPath( id, "a" ), weapon.GetMtrlPath( id, "b" ), weapon.GetMtrlPath( id, "c" ) )
                    };
                }
                else if( item is ItemRowArmor armor ) {
                    var paths = new Dictionary<string, Dictionary<string, string>>();

                    foreach( var race in SelectDataUtils.RaceAnimationIds ) {
                        var skeleton = race.Value.SkeletonId;
                        paths[race.Key] = GetPaths( armor.GetMtrlPath( id, skeleton, "a" ), armor.GetMtrlPath( id, skeleton, "b" ), armor.GetMtrlPath( id, skeleton, "c" ) );
                    }

                    loaded = new() {
                        ImcPath = imcPath,
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

            if( string.IsNullOrEmpty( Loaded.ImcPath ) ) return;

            ImGui.Text( "IMC: " );
            ImGui.SameLine();
            SelectUiUtils.DisplayPath( Loaded.ImcPath );

            if( Loaded.IsWeapon ) {
                DrawPaths( Loaded.WeaponPaths, Selected.Name );
            }
            else {
                DrawWithHeader( Loaded.ArmorPaths, Selected.Name );
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
