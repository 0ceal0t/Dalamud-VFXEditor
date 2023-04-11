using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.Parsing {
    public abstract class ParsedSimpleBase<T> : ParsedBase {
        public T Value = default;
    }
}
