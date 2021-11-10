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
        public static TmbManager Manager { get; private set; }

        public ConcurrentDictionary<string, TmbReplace> PathToTmbReplace = new();

        public static void Initialize(Plugin plugin) {
            ResetInstance();
        }

        private static string LocalPath => Path.Combine(Configuration.Config.WriteLocation, "test.tmb" );

        public static bool GetReplacePath(string path, out FileInfo replacePath) {
            replacePath = null;
            if( Manager == null ) return false;
            if( Manager.TmbPath.Equals(path)) {
                replacePath = new FileInfo( LocalPath );
                return true;
            }
            return false;
        }

        public static void ResetInstance() {
            var oldInstance = Manager;
            Manager = new TmbManager();
            oldInstance?.DisposeInstance();
        }

        public static void Dispose() {
            Manager?.DisposeInstance();
            Manager = null;
        }

        public static void Show() {
            if (Manager == null) return;
            Manager.Visible = true;
        }

        // ==================

        public void DisposeInstance() {
            PathToTmbReplace.Clear();
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
                        for (int i = 0; i < Math.Min(output.Length, file.Data.Length); i++) {
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
