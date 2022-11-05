using SharpDX.Direct3D11;
using System.IO;
using Device = SharpDX.Direct3D11.Device;

namespace VfxEditor.DirectX {
    public class DirectXManager {
        private Device Device;
        private DeviceContext Ctx;

        public readonly ModelPreview ModelPreview;
        public readonly GradientView GradientView;
        public readonly AnimationPreview AnimationPreview;

        public DirectXManager() {
            var shaderPath = Path.Combine( Plugin.RootLocation, "Shaders" );
            Device = Plugin.PluginInterface.UiBuilder.Device;
            Ctx = Device.ImmediateContext;
            ModelPreview = new ModelPreview( Device, Ctx, shaderPath );
            GradientView = new GradientView( Device, Ctx, shaderPath );
            AnimationPreview = new AnimationPreview( Device, Ctx, shaderPath );
        }

        public void Dispose() {
            ModelPreview.Dispose();
            GradientView.Dispose();
            AnimationPreview.Dispose();
            Device = null;
            Ctx = null;
        }
    }
}
