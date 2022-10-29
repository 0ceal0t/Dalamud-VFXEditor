using System.Collections.Generic;
using System.Linq;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.Sheets {
    public class EmotePapSheetLoader : SheetLoader<XivEmotePap, XivEmotePapSelected> {
        public override void OnLoad() {
            var sheet = VfxEditor.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Emote>().Where( x => !string.IsNullOrEmpty( x.Name ) );
            foreach( var item in sheet ) {
                Items.Add( new XivEmotePap( item ) );
            }
        }

        public override bool SelectItem( XivEmotePap item, out XivEmotePapSelected selectedItem ) {
            var allPaps = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

            foreach( var papFile in item.PapFiles ) {
                var key = papFile.Key;
                var papDict = new Dictionary<string, Dictionary<string, string>>();

                if( papFile.Type == XivEmotePap.XivEmotePapType.Normal ) {
                    // bt_common, per race (chara/human/{RACE}/animation/a0001/bt_common/emote/add_yes.pap)
                    papDict.Add( "Action", SheetManager.FileExistsFilter( SheetManager.GetAllSkeletonPaths( $"bt_common/{key}.pap" ) ) ); // just a dummy node

                }
                else if( papFile.Type == XivEmotePap.XivEmotePapType.PerJob ) {
                    // chara/human/c0101/animation/a0001/bt_swd_sld/emote/battle01.pap
                    papDict = SheetManager.GetAllJobPaths( key );
                }
                else if( papFile.Type == XivEmotePap.XivEmotePapType.Facial ) {
                    // chara/human/c0101/animation/f0003/resident/face.pap
                    // chara/human/c0101/animation/f0003/resident/smile.pap
                    // chara/human/c0101/animation/f0003/nonresident/angry_cl.pap
                    papDict = SheetManager.GetAllFacePaths( key );
                }

                allPaps.Add( key, papDict );
            }

            selectedItem = new XivEmotePapSelected( item, allPaps );
            return true;
        }
    }
}
