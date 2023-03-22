using System;
using System.Collections.Generic;

namespace VfxEditor.Select.Pap.Action {
    public class ActionRowSelected {
        public readonly Dictionary<string, string> StartAnimations;
        public readonly Dictionary<string, string> EndAnimations;
        public readonly Dictionary<string, string> HitAnimations;

        public ActionRowSelected( Dictionary<string, string> startAnimations, Dictionary<string, string> endAnimations, Dictionary<string, string> hitAnimations ) {
            StartAnimations = startAnimations;
            EndAnimations = endAnimations;
            HitAnimations = hitAnimations;
        }
    }
}
