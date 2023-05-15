using System.Collections.Generic;

namespace VfxEditor.Ui.Nodes {
    public class NodeGroupSet {
        protected readonly List<NodeGroup> AllGroups = new();

        public void Initialize() => AllGroups.ForEach( group => group.Initialize() );

        public Dictionary<string, string> GetRenamingMap() {
            Dictionary<string, string> ret = new();
            AllGroups.ForEach( group => group.GetRenamingMap( ret ) );
            return ret;
        }

        public void ReadRenamingMap( Dictionary<string, string> renamingMap ) {
            AllGroups.ForEach( group => group.ReadRenamingMap( renamingMap ) );
        }

        public void PreImport( bool hasDependencies ) => AllGroups.ForEach( group => group.PreImport( hasDependencies ) );

        public void PostImport() => AllGroups.ForEach( group => group.PostImport() );

        public void Dispose() => AllGroups.ForEach( group => group.Dispose() );
    }
}
