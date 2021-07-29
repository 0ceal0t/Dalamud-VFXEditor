using System;
using System.IO;
using SharpDX.Direct3D11;
using Device = SharpDX.Direct3D11.Device;
namespace VFXEditor.DirectX {
    public class DirectXManager {
        private static DirectXManager Instance = null;

        public static ModelPreview ModelView => Instance?._ModelView;
        public static GradientView GradientView => Instance?._GradientView;

        public static void Initialize( Plugin plugin ) {
            Instance?.DisposeInstance();
            Instance = new DirectXManager( plugin );
        }

        public static void Dispose() {
            Instance?.DisposeInstance();
            Instance = null;
        }

        // ====== INSTANCE ========

        private Device Device;
        private DeviceContext Ctx;

        private readonly ModelPreview _ModelView;
        private readonly GradientView _GradientView;

        private DirectXManager( Plugin plugin ) {
            var shaderPath = Path.Combine( Plugin.TemplateLocation, "Shaders" );
            Device = plugin.PluginInterface.UiBuilder.Device;
            Ctx = Device.ImmediateContext;
            _ModelView = new ModelPreview( Device, Ctx, shaderPath );
            _GradientView = new GradientView( Device, Ctx, shaderPath );
        }

        private void DisposeInstance() {
            _ModelView.Dispose();
            _GradientView.Dispose();
            Device = null;
            Ctx = null;
        }
    }
}
