using AVFXLib.Models;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXSelect.UI;

namespace VFXEditor.Data {
    public class ReplaceDoc {
        public AVFXBase AVFX = null;
        public string WriteLocation;

        public VFXSelectResult Source;
        public VFXSelectResult Replace;
    }

    public class DocManager {
        public ReplaceDoc ActiveDoc;
        public List<ReplaceDoc> Docs = new();
        public Dictionary<string, string> GamePathToLocalPath = new();

        public Plugin _plugin;
        public int DOC_ID = 0;

        public DocManager( Plugin plugin ) {
            _plugin = plugin;
            NewDoc();
        }

        public ReplaceDoc NewDoc() {
            ReplaceDoc doc = new ReplaceDoc();
            doc.WriteLocation = Path.Combine( _plugin.WriteLocation, "VFXtemp" + ( DOC_ID++ ) + ".avfx" );
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
            if(ActiveDoc.Replace.Path != "") {
                GamePathToLocalPath[ActiveDoc.Replace.Path] = ActiveDoc.WriteLocation;
            }
        }
        public void ResetDoc() {
            File.Delete( ActiveDoc.WriteLocation );
        }
        public void SelectDoc(ReplaceDoc doc ) {
            ActiveDoc = doc;
        }
        public bool RemoveDoc(ReplaceDoc doc ) {
            bool switchDoc = ( doc == ActiveDoc );
            if( GamePathToLocalPath.ContainsKey( doc.Replace.Path ) ) {
                GamePathToLocalPath.Remove( doc.Replace.Path );
            }
            Docs.Remove( doc );
            File.Delete( doc.WriteLocation );
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
                foreach(var _doc in Docs ) {
                    if( _doc.Replace.Path == "" )
                        return false;
                }
                return true;
            }
            return ActiveDoc.Replace.Path != "";
        }

        public void Dispose() {
            foreach(var doc in Docs ) {
                File.Delete( doc.WriteLocation );
            }
        }
    }
}
