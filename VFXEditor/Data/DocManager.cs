using AVFXLib.Models;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.UI;

namespace VFXEditor {
    public class ReplaceDoc {
        public AVFXBase AVFX = null;
        public bool Written = false;
        public string WriteLocation;

        public VFXSelectResult Source;
        public VFXSelectResult Replace;
    }

    public class DocManager {
        public ReplaceDoc ActiveDoc = null;
        public List<ReplaceDoc> Docs = new List<ReplaceDoc>();
        public Dictionary<string, string> GamePathToLocalPath = new Dictionary<string, string>();

        public Plugin _plugin;
        public string WriteLocation;
        public int DOC_ID = 0;

        public DocManager( Plugin plugin ) {
            _plugin = plugin;
            WriteLocation = Path.Combine( plugin.WriteLocation, "documents" );
            Directory.CreateDirectory( WriteLocation );
            NewDoc();
        }

        public ReplaceDoc NewDoc() {
            ReplaceDoc doc = new ReplaceDoc();
            doc.WriteLocation = Path.Combine( WriteLocation, "VFXtemp" + ( DOC_ID++ ) + ".avfx" );
            doc.Source = VFXSelectResult.None();
            doc.Replace = VFXSelectResult.None();

            Docs.Add( doc );
            ActiveDoc = doc;
            return doc;
        }

        public void UpdateSource( VFXSelectResult select ) {
            ActiveDoc.Source = select;
        }
        public void UpdateReplace(VFXSelectResult select ) {
            if( GamePathToLocalPath.ContainsKey( ActiveDoc.Replace.Path ) ) {
                GamePathToLocalPath.Remove( ActiveDoc.Replace.Path );
            }
            ActiveDoc.Replace = select;
            if(ActiveDoc.Replace.Path != "")
                GamePathToLocalPath[ActiveDoc.Replace.Path] = ActiveDoc.WriteLocation;
        }
        public void SelectDoc(ReplaceDoc doc ) {
            ActiveDoc = doc;
        }
        public bool RemoveDoc(ReplaceDoc doc ) {
            bool switchDoc = ( doc == ActiveDoc );
            if( GamePathToLocalPath.ContainsKey( doc.Replace.Path ) )
                GamePathToLocalPath.Remove( doc.Replace.Path );
            Docs.Remove( doc );
            if( switchDoc ) {
                ActiveDoc = Docs[0];
                return true;
            }
            return false;
        }
        public bool HasVFX() {
            return ActiveDoc.AVFX != null;
        }

        public void Save() {
            _plugin.Manager.SaveLocalFile( ActiveDoc.WriteLocation, ActiveDoc.AVFX );
        }

        public string GetLocalPath(string gamePath ) {
            if( GamePathToLocalPath.ContainsKey( gamePath ) ) {
                foreach(var doc in Docs) {
                    if(doc.WriteLocation == GamePathToLocalPath[gamePath] && doc.Source.DisplayString == "[NONE]" ) { // if it's not being replaced by anything, don't bother
                        return "";
                    }
                }
                return GamePathToLocalPath[gamePath];
            }
            return "";
        }

        public void Cleanup() {
            foreach(var doc in Docs ) {
                File.Delete( doc.WriteLocation );
            }
        }
    }
}
