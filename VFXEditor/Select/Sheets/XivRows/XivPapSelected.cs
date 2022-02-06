using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXSelect.Select.Rows {
    public class XivPapSelected {
        public XivPap Pap;

        public readonly Dictionary<string, string> StartAnimations;
        public readonly Dictionary<string, string> EndAnimations;
        public readonly Dictionary<string, string> HitAnimations;

        public XivPapSelected( XivPap pap, Dictionary<string, string> startAnimations, Dictionary<string, string> endAnimations, Dictionary<string, string> hitAnimations ) {
            Pap = pap;
            StartAnimations = startAnimations;
            EndAnimations = endAnimations;
            HitAnimations = hitAnimations;
        }
    }
}
