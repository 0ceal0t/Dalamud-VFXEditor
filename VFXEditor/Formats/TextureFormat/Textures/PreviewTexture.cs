using ImGuiNET;
using System.Numerics;

namespace VfxEditor.TextureFormat.Textures {
    public class PreviewTexture {
        public readonly string GamePath;
        public readonly ushort Height;
        public readonly ushort Width;
        public readonly ushort MipLevels;
        public readonly ushort Depth;
        public readonly bool IsReplaced;
        public readonly TextureFormat Format;
        public readonly ImGuiScene.TextureWrap Wrap;

        public PreviewTexture( TextureFile file, string gamePath, bool loadImage ) {
            Format = file.Header.Format;
            MipLevels = file.Header.MipLevels;
            Width = file.Header.Width;
            Height = file.Header.Height;
            Depth = file.Header.Depth;
            IsReplaced = file.Local;
            GamePath = gamePath;
            if( loadImage ) {
                var texBind = Plugin.PluginInterface.UiBuilder.LoadImageRaw( file.ImageData, file.Header.Width, file.Header.Height, 4 );
                Wrap = texBind;
            }
        }

        public void Draw() {
            ImGui.Image( Wrap.ImGuiHandle, new Vector2( Width, Height ) );
        }
    }
}
