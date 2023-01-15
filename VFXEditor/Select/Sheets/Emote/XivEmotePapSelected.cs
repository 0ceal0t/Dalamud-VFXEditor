using System.Collections.Generic;

namespace VfxEditor.Select.Rows {
    public class XivEmotePapSelected {
        public readonly XivEmotePap EmotePap;

        public readonly Dictionary<string, Dictionary<string, Dictionary<string, string>>> AllPaps;

        public XivEmotePapSelected( XivEmotePap emotePap, Dictionary<string, Dictionary<string, Dictionary<string, string>>> allPaps ) {
            EmotePap = emotePap;
            AllPaps = allPaps;
        }
    }
}
