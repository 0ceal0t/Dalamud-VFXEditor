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


        public static bool ClearSelect {
            get { return Instance.DoClearSelect; }
            set { Instance.DoClearSelect = value; }
        }

        public static bool Copy {
            get { return Instance.DoCopy; }
            set { Instance.DoCopy = value; }
        }

        public static bool Paste {
            get { return Instance.DoPaste; }
            set { Instance.DoPaste = value; }
        }

        public static void Initialize() {
            Instance = new CopyManager();
        }

        public static void Dispose() {
        }

        public static void ResetEvents() {
            ClearSelect = false;
            Copy = false;
            Paste = false;
        }

        public bool DoClearSelect = false; // when an item is selected, clear the selection of the other ones
        public bool DoCopy = false;
        public bool DoPaste = false;

        public CopyManager() {
        }
    }
}
