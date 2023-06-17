using VfxEditor.FileManager;

namespace VfxEditor.TextureFormat.Textures {
    public class TextureReplace : IFileDocument {
        public readonly string LocalPath;
        public readonly string ReplacePath;
        public readonly int Height;
        public readonly int Width;
        public readonly int Depth;
        public readonly int MipLevels;
        public readonly TextureFormat Format;

        public TextureReplace( string localPath, string replacePath, int height, int width, int depth, int mips, TextureFormat format ) {
            LocalPath = localPath;
            ReplacePath = replacePath;
            Height = height;
            Width = width;
            Depth = depth;
            MipLevels = mips;
            Format = format;
        }

        public string GetDisplayName() => ReplacePath;
    }
}
