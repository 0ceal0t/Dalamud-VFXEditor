using SharpDX.Direct3D11;
using System.IO;
using Device = SharpDX.Direct3D11.Device;

namespace VfxEditor.DirectX {
    public class DirectXManager {
        private Device Device;
        private DeviceContext Ctx;

        public readonly ModelPreview ModelView;
        public readonly GradientView GradientView;

        public DirectXManager() {
            var shaderPath = Path.Combine( VfxEditor.RootLocation, "Shaders" );
            Device = VfxEditor.PluginInterface.UiBuilder.Device;
            Ctx = Device.ImmediateContext;
            ModelView = new ModelPreview( Device, Ctx, shaderPath );
            GradientView = new GradientView( Device, Ctx, shaderPath );
        }

        public void Dispose() {
            ModelView.Dispose();
            GradientView.Dispose();
            Device = null;
            Ctx = null;
        }
    }
}
