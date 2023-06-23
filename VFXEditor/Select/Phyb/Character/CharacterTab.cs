using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using OtterGui.Raii;
using System.Collections.Generic;
using VfxEditor.Select.Shared.Character;

namespace VfxEditor.Select.Phyb.Character {
    public class CharacterRowSelected {
        public Dictionary<string, string> FacePaths;
        public Dictionary<string, string> HairPaths;
    }

    public class CharacterPhybTab : SelectTab<CharacterRow, CharacterRowSelected> {
        public CharacterPhybTab( SelectDialog dialog, string name ) : base( dialog, name, "Character-Shared" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            foreach( var item in SelectUtils.RaceAnimationIds ) Items.Add( new( item.Key, item.Value ) );
        }

        public override void LoadSelection( CharacterRow item, out CharacterRowSelected loaded ) {
            // chara/human/c1201/skeleton/hair/h0101/phy_c1201h0101.phyb
            // chara/human/c1801/skeleton/face/f0005/phy_c1801f0005.phyb

            var facePaths = new Dictionary<string, string>();
            var hairPaths = new Dictionary<string, string>();

            for( var face = 1; face <= SelectUtils.MaxFaces; face++ ) {
                var faceString = $"f{face:D4}";
                facePaths[$"Face {face}"] = $"chara/human/{item.SkeletonId}/skeleton/face/{faceString}/phy_{item.SkeletonId}{faceString}.phyb";
            }

            var sheet = Plugin.DataManager.GetExcelSheet<CharaMakeCustomize>();
            for( var hair = item.HairOffset; hair < item.HairOffset + SelectUtils.HairEntries; hair++ ) {
                var hairRow = sheet.GetRow( ( uint )hair );
                var hairId = ( int )hairRow.FeatureID;
                if( hairId == 0 ) continue;

                var hairString = $"h{hairId:D4}";
                hairPaths[$"Hair {hairId}"] = $"chara/human/{item.SkeletonId}/skeleton/hair/{hairString}/phy_{item.SkeletonId}{hairString}.phyb";
            }

            loaded = new() {
                FacePaths = SelectUtils.FileExistsFilter( facePaths ),
                HairPaths = SelectUtils.FileExistsFilter( hairPaths )
            };
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
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
