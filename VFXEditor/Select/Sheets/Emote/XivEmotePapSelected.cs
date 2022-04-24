using System.Collections.Generic;

namespace VFXSelect.Select.Rows {
    public class XivEmotePapSelected {
        public XivEmotePap EmotePap;

        public readonly Dictionary<string, Dictionary<string, Dictionary<string, string>>> AllPaps;

        public XivEmotePapSelected( XivEmotePap emotePap, Dictionary<string, Dictionary<string, Dictionary<string, string>>> allPaps ) {
            EmotePap = emotePap;
            AllPaps = allPaps;
        }
    }
}
