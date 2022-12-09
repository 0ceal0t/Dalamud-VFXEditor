using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Select.Rows {
    public class XivItemSelected {
        public XivItem Item;
        public bool VfxExists => VfxIds.Count > 0;
        public List<int> VfxIds;
        public string ImcPath;

        public XivItemSelected( Lumina.Data.Files.ImcFile file, XivItem item ) {
            Item = item;
            ImcPath = file.FilePath;

            VfxIds = file.GetParts().Select( x => x.Variants[item.Variant - 1] ).Where( x => x.VfxId != 0 ).Select( x => (int) x.VfxId ).ToList();
        }

        public List<string> GetVfxPaths() {
            if( !VfxExists ) return new List<string>();
            return VfxIds.Select( Item.GetVfxPath ).ToList();
        }
    }
}
