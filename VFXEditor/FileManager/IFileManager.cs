namespace VfxEditor.FileManager {
    public interface IFileManager {
        public bool DoDebug( string path );

        public bool GetReplacePath( string gamePath, out string replacePath );

        public void Draw();
    }
}
