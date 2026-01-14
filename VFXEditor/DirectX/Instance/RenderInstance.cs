using SharpDX.Direct3D11;
using System;
using VfxEditor;
using Device = SharpDX.Direct3D11.Device;

namespace VFXEditor.DirectX.Instance {
    public abstract class RenderInstance : IDisposable {
        protected bool NeedsRedraw = false;

        protected readonly Device Device;
        protected readonly DeviceContext Ctx;

        private static int _Id = 0;
        public static int NewId => _Id++;
        public int CurrentRenderId { get; protected set; } = -1;

        public RenderInstance() {
            Device = Plugin.DirectXManager.Device;
            Ctx = Plugin.DirectXManager.Ctx;
        }

        public void SetCurrentRenderId( int renderId ) {
            CurrentRenderId = renderId;
        }

        public void IsUpdated() {
            NeedsRedraw = true;
        }

        public void Updated() {
            NeedsRedraw = false; 
        }

        protected abstract void ResizeResources();

        public abstract void Dispose();
    }
}
