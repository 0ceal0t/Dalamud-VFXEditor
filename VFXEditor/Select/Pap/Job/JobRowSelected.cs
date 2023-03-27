using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.Select.Pap.Job {
    public class JobRowSelected {
        public readonly Dictionary<string, GeneralData> General; // per race
        public readonly Dictionary<string, PoseData> Pose;
        public readonly Dictionary<string, List<string>> AutoAttack;

        public JobRowSelected( Dictionary<string, GeneralData> general, Dictionary<string, PoseData> pose, Dictionary<string, List<string>> autoAttack) {
            General = general;
            Pose = pose;
            AutoAttack = autoAttack;
        }
    }
}
