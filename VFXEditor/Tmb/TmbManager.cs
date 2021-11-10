using Dalamud.Logging;
using ImGuiNET;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.Tmb {
    public struct TmbReplace {
        public string LocalPath;
        public TmbFile File;
    }

    public class TmbManager {
        private static string LocalPath => Path.Combine(Plugin.Configuration.WriteLocation, "test.tmb" );

        public bool GetReplacePath(string path, out string replacePath) {
            replacePath = null;
            if( TmbPath.Equals(path)) {
                replacePath = LocalPath;
                return true;
            }
            return false;
        }

        public void Dispose() {
        }

        public void Show() {
            Visible = true;
        }

        private string TmbPath = "";
        private bool Visible = false;
        private TmbFile CurrentFile;

        public void Draw() {
            if( !Visible ) return;
            ImGui.SetNextWindowSize( new Vector2( 600, 400 ), ImGuiCond.FirstUseEver );

            if( ImGui.Begin( "Tmb Tester", ref Visible ) ) {
                ImGui.InputText( "Path##Tmb", ref TmbPath, 255 );
                ImGui.SameLine();
                if( ImGui.Button( "Load##Tmb" ) ) {
                    var result = Plugin.DataManager.FileExists( TmbPath );
                    if( result ) {
                        var file = Plugin.DataManager.GetFile( TmbPath );
                        using var ms = new MemoryStream( file.Data );
                        using var br = new BinaryReader( ms );

                        CurrentFile = new TmbFile( br );
                        Update();

                        var output = CurrentFile.ToBytes();
                        for (var i = 0; i < Math.Min(output.Length, file.Data.Length); i++) {
                            if (output[i] != file.Data[i]) {
                                PluginLog.Log( $"FILES DO NOT MATCH: {i}" );
                                break;
                            }
                        }
                    }
                }
                ImGui.SameLine();
                if( ImGui.Button( "Update" ) ) Update();

                ImGui.SameLine();
                if( ImGui.Button( "Reload" ) ) Reload();
                ImGui.SameLine();
                Plugin.HelpMarker( "Manually reload the resource" );

                if( CurrentFile != null ) CurrentFile.Draw("##Tmb");
            }
            ImGui.End();
        }

        private void Update() {
            if( CurrentFile == null ) return;
            File.WriteAllBytes( LocalPath, CurrentFile.ToBytes() );
        }

        private void Reload() {
            if( CurrentFile == null ) return;
            Plugin.ResourceLoader.ReloadPath( TmbPath, false );
        }
    }
}
