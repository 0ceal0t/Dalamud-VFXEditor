using System;
using System.Collections.Generic;

namespace VfxEditor.Select2.Pap.Emote {
    public class EmoteRowSelected {
        public readonly Dictionary<string, Dictionary<string, Dictionary<string, string>>> AllPaps;

        public EmoteRowSelected( Dictionary<string, Dictionary<string, Dictionary<string, string>>> allPaps ) {
            AllPaps = allPaps;
        }
    }
}
