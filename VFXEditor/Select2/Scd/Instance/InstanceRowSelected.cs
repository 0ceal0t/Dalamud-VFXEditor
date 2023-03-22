using Lumina.Excel.GeneratedSheets;
using VfxEditor.Select2.Scd.BgmQuest;

namespace VfxEditor.Select2.Scd.Instance {
    public class InstanceRowSelected {
        public readonly BgmSituationStruct Situation;

        public InstanceRowSelected( InstanceRow content ) {
            var instance = Plugin.DataManager.GetExcelSheet<InstanceContent>().GetRow( content.ContentRowId );
            Situation = BgmQuestRowSelected.GetBgmSituation( ( ushort )instance.BGM.Row );
        }
    }
}
