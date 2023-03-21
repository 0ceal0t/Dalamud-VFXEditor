using System;
using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Select2.Vfx.Item {
    public class ItemRowSelected {
        public readonly ItemRow Item;
        public string ImcPath;
        public List<int> VfxIds;
        public bool VfxExists => VfxIds.Count > 0;

        public ItemRowSelected( Lumina.Data.Files.ImcFile file, ItemRow item ) {
            Item = item;
            ImcPath = file.FilePath;

            VfxIds = file.GetParts().Select( x => x.Variants[item.Variant - 1] ).Where( x => x.VfxId != 0 ).Select( x => ( int )x.VfxId ).ToList();
        }

        public List<string> GetVfxPaths() {
            if( !VfxExists ) return new List<string>();
            return VfxIds.Select( Item.GetVfxPath ).ToList();
        }
    }
}
