using System;
using System.IO;
using Dalamud.Plugin;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;
using System.Reflection;
using System.Threading.Tasks;

namespace VFXEditor.Data.DirectX {
    public class DirectXManager {
        private static DirectXManager Instance = null;

        public static ModelPreview ModelView => Instance?._ModelView;
        public static Gradient GradientView => Instance?._GradientView;

        private Device Device;
        private DeviceContext Ctx;

        private ModelPreview _ModelView;
        private Gradient _GradientView;

        public static void Initialize( Plugin plugin ) {
            Instance?.DoDispose();
            Instance = new DirectXManager( plugin );
        }
        private DirectXManager(Plugin plugin) {
            var shaderPath = Path.Combine( Plugin.TemplateLocation, "Shaders" );
            Device = plugin.PluginInterface.UiBuilder.Device;
            Ctx = Device.ImmediateContext;
            _ModelView = new ModelPreview( Device, Ctx, shaderPath );
            _GradientView = new Gradient( Device, Ctx, shaderPath );
        }

        public static void Dispose() {
            Instance?.DoDispose();
            Instance = null;
        }
        private void DoDispose() {
            _ModelView.Dispose();
            _GradientView.Dispose();
            Device = null;
            Ctx = null;
        }
    }
}
