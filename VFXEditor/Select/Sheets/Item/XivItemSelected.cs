using System.Collections.Generic;
using System.Linq;

namespace VFXEditor.Select.Rows {
    public class XivItemSelected {
        public XivItem Item;
        public bool VfxExists;
        public List<int> VfxIds;
        public string ImcPath;

        public XivItemSelected( Lumina.Data.Files.ImcFile file, XivItem item ) {
            Item = item;
            ImcPath = file.FilePath;

            VfxIds = file.GetParts().Select( x => x.Variants[item.Variant - 1] ).Where( x => x.VfxId != 0 ).Select( x => (int) x.VfxId ).ToList();
            VfxExists = VfxIds.Count > 0;
        }

        public List<string> GetVfxPath() {
            if( !VfxExists ) return new List<string>();
            return VfxIds.Select( Item.GetVfxPath ).ToList();
        }
    }
}
