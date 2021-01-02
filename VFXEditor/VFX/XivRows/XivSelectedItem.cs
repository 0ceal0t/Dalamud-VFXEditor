using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor
{
    public class XivSelectedItem
    {
        public XivItem Item;
        public int Count;

        public bool VfxExists;
        public int VfxId;
        public string ImcPath;

        public XivSelectedItem( Lumina.Data.Files.ImcFile file, XivItem item)
        {
            Item = item;
            Count = file.Count;
            var imcData = file.GetVariant( item.Variant - 1 );
            ImcPath = file.FilePath;
            VfxId = imcData.VfxId;
            VfxExists = !( VfxId == 0 );
        }

        public string GetVFXPath()
        {
            if( !VfxExists )
                return "--";
            return Item.GetVFXPath( VfxId );
        }
    }
}
