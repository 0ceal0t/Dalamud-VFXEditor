using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.Structs.Vfx {
    public abstract class BaseVfx {
        public Plugin _Plugin;
        public IntPtr Vfx;
        public string Path; // the ".avfx"
        public string RedirectPath;

        public BaseVfx( Plugin plugin, string path) {
            _Plugin = plugin;
            Path = path;
        }

        public abstract void Remove();
    }
}