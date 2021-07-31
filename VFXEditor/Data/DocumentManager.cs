using AVFXLib.Models;
using Dalamud.Plugin;
using System.Collections.Generic;
using System.IO;
using VFXEditor.Data.Texture;
using VFXEditor.UI.VFX;
using VFXSelect.UI;

namespace VFXEditor.Data {
    public class ReplaceDoc {
        public UIMain Main = null;
        public string WriteLocation;

        public VFXSelectResult Source;
        public VFXSelectResult Replace;

        public void SetAVFX(AVFXBase avfx) {
            Dispose();
            Main = new UIMain( avfx );
        }

        public void Dispose() {
            Main?.Dispose();
            Main = null;
            File.Delete( WriteLocation );
        }
    }

    public class DocumentManager {
        public static DocumentManager Manager => Instance;
        private static DocumentManager Instance = null;

        public static ReplaceDoc CurrentActiveDoc => Instance?.ActiveDoc;
        public static List<ReplaceDoc> CurrentDocs => Instance?.Docs;

        public static void Initialize() {
            ResetInstance();
        }

        public static void ResetInstance() {
            var oldInstance = Instance;
            Instance = new DocumentManager();
            oldInstance?.DisposeInstance();
        }

        public static void Dispose() {
            Instance?.DisposeInstance();
            Instance = null;
        }

        // ========= INSTANCE =========

        private ReplaceDoc ActiveDoc;
        private List<ReplaceDoc> Docs = new();

        private Dictionary<string, string> GamePathToLocalPath = new();
        private int DOC_ID = 0;

        private DocumentManager() {
            NewDoc();
        }

        public ReplaceDoc NewDoc() {
            var doc = new ReplaceDoc {
                WriteLocation = Path.Combine( Configuration.Config.WriteLocation, "VFXtemp" + ( DOC_ID++ ) + ".avfx" ),
                Source = VFXSelectResult.None(),
                Replace = VFXSelectResult.None()
            };

            Docs.Add( doc );
            ActiveDoc = doc;
            return doc;
        }

        public void ImportLocalDoc(string localPath, VFXSelectResult source, VFXSelectResult replace, Dictionary<string, string> renaming) {
            var doc = new ReplaceDoc {
                Source = source,
                Replace = replace,
                WriteLocation = Path.Combine( Configuration.Config.WriteLocation, "VFXtemp" + ( DOC_ID++ ) + ".avfx" )
            };

            if(localPath != "") {
                File.Copy( localPath, doc.WriteLocation, true );
                var localResult = DataManager.GetLocalFile( doc.WriteLocation, out var localAvfx );
                if( localResult ) {
                    doc.Main = new UIMain( localAvfx );
                    doc.Main.ReadRenamingMap( renaming );
                }
            }

            Docs.Add( doc );
            RebuildMap();
        }

        public void UpdateSource( VFXSelectResult select ) {
            ActiveDoc.Source = select;
        }

        public void UpdateReplace(VFXSelectResult select ) {
            ActiveDoc.Replace = select;
            RebuildMap();
        }

        public void SelectDoc(ReplaceDoc doc ) {
            ActiveDoc = doc;
        }

        public bool RemoveDoc(ReplaceDoc doc ) {
            var switchDoc = ( doc == ActiveDoc );
            Docs.Remove( doc );
            RebuildMap();

            if( switchDoc ) {
                ActiveDoc = Docs[0];
                doc.Dispose();
                return true;
            }

            doc.Dispose();
            return false;
        }

        public void Save() {
            DataManager.SaveLocalFile( ActiveDoc.WriteLocation, ActiveDoc.Main.AVFX );
            if(Configuration.Config.LogAllFiles) {
                PluginLog.Log( $"Saved VFX to {ActiveDoc.WriteLocation}" );
            }
        }

        public bool GetLocalPath(string gamePath, out FileInfo file ) {
            file = null;
            if( GamePathToLocalPath.ContainsKey( gamePath ) ) {
                file = new FileInfo( GamePathToLocalPath[gamePath] );
                return true;
            }
            return false;
        }

        public bool HasReplacePath(bool allDocuments ) {
            if( allDocuments ) {
                foreach(var doc in Docs ) {
                    if( doc.Replace.Path == "" )
                        return false;
                }
                return true;
            }
            return ActiveDoc.Replace.Path != "";
        }

        private void DisposeInstance() {
            foreach(var doc in Docs ) {
                doc.Dispose();
            }
            Docs = null;
            ActiveDoc = null;
            GamePathToLocalPath = null;
        }

        private void RebuildMap() {
            Dictionary<string, string> newMap = new();
            foreach( var doc in Docs ) {
                if( doc.Replace.Path != "" ) {
                    newMap[doc.Replace.Path] = doc.WriteLocation;
                }
            }
            GamePathToLocalPath = newMap;
        }
    }
}
