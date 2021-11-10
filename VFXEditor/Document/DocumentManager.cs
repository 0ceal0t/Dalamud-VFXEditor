using Dalamud.Logging;
using System.Collections.Generic;
using System.IO;

using AVFXLib.Models;

using VFXEditor.Data;
using VFXEditor.UI;
using VFXEditor.UI.Vfx;

using VFXSelect.UI;

namespace VFXEditor.Document {
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

    public partial class DocumentManager : GenericDialog {

        public ReplaceDoc ActiveDocument { get; private set; }
        public List<ReplaceDoc> Documents { get; private set; } = new();

        private Dictionary<string, string> GamePathToLocalPath = new();
        private int DOC_ID = 0;

        public DocumentManager() : base("Documents") {
            CreateNewDocument();
        }

        public ReplaceDoc CreateNewDocument() {
            var doc = new ReplaceDoc {
                WriteLocation = Path.Combine( Plugin.Configuration.WriteLocation, "VFXtemp" + ( DOC_ID++ ) + ".avfx" ),
                Source = VFXSelectResult.None(),
                Replace = VFXSelectResult.None()
            };

            Documents.Add( doc );
            ActiveDocument = doc;
            return doc;
        }

        public void ImportLocalDocument(string localPath, VFXSelectResult source, VFXSelectResult replace, Dictionary<string, string> renaming) {
            var doc = new ReplaceDoc {
                Source = source,
                Replace = replace,
                WriteLocation = Path.Combine( Plugin.Configuration.WriteLocation, "VFXtemp" + ( DOC_ID++ ) + ".avfx" )
            };

            if(localPath != "") {
                File.Copy( localPath, doc.WriteLocation, true );
                var localResult = AvfxHelper.GetLocalFile( doc.WriteLocation, out var localAvfx );
                if( localResult ) {
                    doc.Main = new UIMain( localAvfx );
                    doc.Main.ReadRenamingMap( renaming );
                }
            }

            Documents.Add( doc );
            RebuildMap();
        }

        public void UpdateSource( VFXSelectResult select ) {
            ActiveDocument.Source = select;
        }

        public void UpdateReplace(VFXSelectResult select ) {
            ActiveDocument.Replace = select;
            RebuildMap();
        }

        public void SelectDocument(ReplaceDoc doc ) {
            ActiveDocument = doc;
        }

        public bool RemoveDocument(ReplaceDoc doc ) {
            var switchDoc = ( doc == ActiveDocument );
            Documents.Remove( doc );
            RebuildMap();

            if( switchDoc ) {
                ActiveDocument = Documents[0];
                doc.Dispose();
                return true;
            }

            doc.Dispose();
            return false;
        }

        public void Save() {
            AvfxHelper.SaveLocalFile( ActiveDocument.WriteLocation, ActiveDocument.Main.AVFX );
            if( Plugin.Configuration.LogAllFiles) {
                PluginLog.Log( $"Saved VFX to {ActiveDocument.WriteLocation}" );
            }
        }

        public bool GetReplacePath(string gamePath, out string file ) {
            file = GamePathToLocalPath.TryGetValue( gamePath, out var path ) ? path : null;
            return !string.IsNullOrEmpty( file );
        }

        public bool HasReplacePath(bool allDocuments ) {
            if( allDocuments ) {
                foreach(var doc in Documents ) {
                    if( doc.Replace.Path == "" )
                        return false;
                }
                return true;
            }
            return ActiveDocument.Replace.Path != "";
        }

        private void RebuildMap() {
            Dictionary<string, string> newMap = new();
            foreach( var doc in Documents ) {
                if( doc.Replace.Path != "" ) {
                    newMap[doc.Replace.Path] = doc.WriteLocation;
                }
            }
            GamePathToLocalPath = newMap;
        }

        public void Dispose() {
            foreach( var doc in Documents ) {
                doc.Dispose();
            }
            Documents = null;
            ActiveDocument = null;
            GamePathToLocalPath = null;
        }
    }
}
