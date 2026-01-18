using SharpDX.Direct3D11;
using System;
using Device = SharpDX.Direct3D11.Device;

namespace VfxEditor.DirectX {
    public abstract class RenderInstance : IDisposable {
        public bool NeedsRender { get; set; } = false;

        protected readonly Device Device;
        protected readonly DeviceContext Ctx;

        private static int _Id = 0;
        public static int NewId => _Id++;
        public int CurrentRenderId { get; set; } = -1;

        public RenderInstance() {
            Device = Plugin.DirectXManager.Device;
            Ctx = Plugin.DirectXManager.Ctx;
            Plugin.DirectXManager.Instances.Add( this );
        }

        protected abstract void ResizeResources();

        public virtual void Dispose() {
            Plugin.DirectXManager.Instances.Remove(this);
        }
    }
}
