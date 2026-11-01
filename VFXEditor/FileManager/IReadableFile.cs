using System.IO;

namespace VfxEditor.FileManager {
    // A file that can be read without any extra state from its document, so it can use FormatDocument
    public interface IReadableFile<F> where F : FileManagerFile {
        public static abstract F Read( BinaryReader reader, string sourcePath, bool verify );
    }
}