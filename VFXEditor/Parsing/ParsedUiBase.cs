using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.Parsing {
    public interface IParsedUiBase {
        public void Draw( string parentId, CommandManager manager );
    }
}
