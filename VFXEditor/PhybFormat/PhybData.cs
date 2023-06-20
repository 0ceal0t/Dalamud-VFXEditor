using System.Collections.Generic;
using VfxEditor.Parsing;

namespace VfxEditor.PhybFormat {
    public abstract class PhybData { // : IUiItem
        private readonly PhybFile File;
        private readonly List<ParsedBase> Parsed;

        public PhybData( PhybFile file ) {
            File = file;
            Parsed = GetParsed();
        }

        protected abstract List<ParsedBase> GetParsed();
    }
}
