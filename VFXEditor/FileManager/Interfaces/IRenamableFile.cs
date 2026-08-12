using System.Collections.Generic;

namespace VfxEditor.FileManager.Interfaces {
    public interface IRenamableFile {
        public Dictionary<string, string> GetRenamingMap();

        public void ReadRenamingMap( Dictionary<string, string> renamingMap );
    }
}
