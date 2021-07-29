using AVFXLib.Models;
using Dalamud.Plugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using VFXEditor.Data.Texture;

namespace VFXEditor.External {
    public static class Penumbra {
        private struct PenumbraMod {
            public string Name;
            public string Author;
            public string Description;
#nullable enable
            public string? Version;
            public string? Website;
#nullable disable
            public Dictionary<string, string> FileSwaps;
        }

        public static void Export( Plugin _plugin, string name, string author, string version, string modFolder, bool exportAll, bool exportTex ) {
            try {
                var mod = new PenumbraMod {
                    Name = name,
                    Author = author,
                    Description = "Exported from VFXEditor",
                    Version = version,
                    Website = null,
                    FileSwaps = new Dictionary<string, string>()
                };

                Directory.CreateDirectory( modFolder );
                var modConfig = Path.Combine( modFolder, "meta.json" );
                var configString = JsonConvert.SerializeObject( mod );
                File.WriteAllText( modConfig, configString );

                void AddMod( AVFXBase _avfx, string _path ) {
                    if( !string.IsNullOrEmpty( _path ) && _avfx != null ) {
                        var data = _avfx.ToAVFX().ToBytes();
                        var modFile = Path.Combine( modFolder, _path );
                        var modFileFolder = Path.GetDirectoryName( modFile );
                        Directory.CreateDirectory( modFileFolder );
                        File.WriteAllBytes( modFile, data );
                    }
                }

                void AddTex( string localPath, string _path ) {
                    if( !string.IsNullOrEmpty( localPath ) && !string.IsNullOrEmpty( _path ) ) {
                        var modFile = Path.Combine( modFolder, _path );
                        var modFileFolder = Path.GetDirectoryName( modFile );
                        Directory.CreateDirectory( modFileFolder );
                        File.Copy( localPath, modFile, true );
                    }
                }

                if( exportAll ) {
                    foreach( var doc in _plugin.DocManager.Docs ) {
                        AddMod( doc.Main?.AVFX, doc.Replace.Path );
                    }
                }
                else {
                    AddMod( _plugin.CurrentDocument.Main?.AVFX, _plugin.DocManager.ActiveDoc.Replace.Path );
                }

                if( exportTex ) {
                    foreach( var entry in TextureManager.Manager.PathToTextureReplace ) {
                        AddTex( entry.Value.localPath, entry.Key );
                    }
                }

                PluginLog.Log( "Exported To: " + modFolder );
            }
            catch( Exception e ) {
                PluginLog.LogError( e, "Could not export to Penumbra" );
            }
        }
    }
}
