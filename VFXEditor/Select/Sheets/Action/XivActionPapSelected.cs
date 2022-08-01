using System.Collections.Generic;

namespace VFXEditor.Select.Rows {
    public class XivActionPapSelected {
        public XivActionPap ActionPap;

        public readonly Dictionary<string, string> StartAnimations;
        public readonly Dictionary<string, string> EndAnimations;
        public readonly Dictionary<string, string> HitAnimations;

        public XivActionPapSelected( XivActionPap actionPap, Dictionary<string, string> startAnimations, Dictionary<string, string> endAnimations, Dictionary<string, string> hitAnimations ) {
            ActionPap = actionPap;
            StartAnimations = startAnimations;
            EndAnimations = endAnimations;
            HitAnimations = hitAnimations;
        }
    }
}
