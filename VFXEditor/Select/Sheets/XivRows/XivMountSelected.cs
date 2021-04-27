using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VFXSelect.Data.Rows {
    public class XivMountSelected {
        public XivMount Mount;

        public int Count;
        public bool VfxExists;
        public int VfxId;
        public string ImcPath;


        public XivMountSelected( Lumina.Data.Files.ImcFile file, XivMount mount) {
            Mount = mount;

            Count = file.Count;
            var imcData = file.GetVariant( mount.Variant - 1 );
            ImcPath = file.FilePath;
            VfxId = imcData.VfxId;
            VfxExists = !( VfxId == 0 );
        }

        public string GetVFXPath() {
            if( !VfxExists )
                return "--";
            return Mount.GetVfxPath( VfxId );
        }
    }
}
