using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public abstract class AvfxDrawable : AvfxBase, IUiBase {
        public AvfxDrawable( string avfxName ) : base( avfxName ) { }

        public abstract void Draw( string parentId );
    }
}
