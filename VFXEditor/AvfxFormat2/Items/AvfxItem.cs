using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public abstract class AvfxItem : AvfxDrawable, IUiItem {
        public AvfxItem( string avfxName ) : base( avfxName ) { }

        public abstract string GetDefaultText();
        public virtual string GetText() => GetDefaultText();
    }
}
