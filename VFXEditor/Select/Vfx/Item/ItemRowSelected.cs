using System;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Select.Shared.Item;

namespace VfxEditor.Select.Vfx.Item {
    public class ItemRowSelected {
        public readonly string ImcPath;
        public readonly List<string> VfxPaths;

        public ItemRowSelected( Lumina.Data.Files.ImcFile file, ItemRow item ) {
            ImcPath = file.FilePath;
            var vfxIds = file.GetParts().Select( x => x.Variants[item.GetVariant() - 1] ).Where( x => x.VfxId != 0 ).Select( x => ( int )x.VfxId ).ToList();
            VfxPaths = vfxIds.Select( item.GetVfxPath ).ToList();
        }
    }
}
