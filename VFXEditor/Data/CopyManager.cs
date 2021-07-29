using AVFXLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.UI.VFX;

namespace VFXEditor.Data {
    public class CopyManager {
        private static CopyManager Instance;

        public static void Initialize() {
            Instance = new CopyManager();
        }

        public static void Dispose() {
        }

        public CopyManager() {
        }
    }
}
