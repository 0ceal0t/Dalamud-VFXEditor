using Dalamud.Game.ClientState;
using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor {
    public class VfxTracker {
        public bool Enabled = false;

        public struct ActorData {
            public IntPtr actor;
            public string path;
        }
        public struct StaticData {
            public string path;
        }

        public Plugin _plugin;
        public ConcurrentDictionary<IntPtr, ActorData> ActorVfxs;
        public ConcurrentDictionary<IntPtr, StaticData> StaticVfxs;

        public VfxTracker(Plugin plugin ) {
            _plugin = plugin;
            Reset();
        }

        public void AddActor(IntPtr actor, IntPtr vfx, string path ) {
            if( !Enabled ) return;
            ActorData data = new ActorData() {
                actor = actor,
                path = path
            };
            ActorVfxs.TryAdd( vfx, data );
        }
        public void RemoveActor(IntPtr vfx) {
            if( !Enabled ) return;
            if( ActorVfxs.ContainsKey( vfx ) ) {
                ActorVfxs.TryRemove( vfx, out var value );
            }
        }


        public void AddStatic(IntPtr vfx, string path ) {
            if( !Enabled ) return;
            StaticData data = new StaticData() {
                path = path
            };
            StaticVfxs.TryAdd( vfx, data );
        }
        public void RemoveStatic(IntPtr vfx ) {
            if( !Enabled ) return;
            if( StaticVfxs.ContainsKey( vfx ) ) {
                StaticVfxs.TryRemove( vfx, out var value );
            }
        }

        public bool WatchingCutscene() {
            return ( _plugin.PluginInterface.ClientState != null && _plugin.PluginInterface.ClientState.Condition[ConditionFlag.OccupiedInCutSceneEvent] || _plugin.PluginInterface.ClientState.Condition[ConditionFlag.WatchingCutscene78] );
        }

        public struct StaticVfxGroup {
            public string path;
            public SharpDX.Vector3 position;
        }

        public class ClosenessComp : IEqualityComparer<SharpDX.Vector3> {
            public bool Equals( SharpDX.Vector3 x, SharpDX.Vector3 y ) {
                return ( x - y ).Length() < 2;
            }

            public int GetHashCode( SharpDX.Vector3 obj ) {
                return 0;
            }
        }
        public static ClosenessComp CloseComp = new ClosenessComp();

        public void Draw() {
            if( !Enabled ) return;

            var playPos = _plugin.PluginInterface.ClientState?.LocalPlayer?.Position;
            var screenPos = ImGui.GetMainViewport().Pos;
            var screenSize = ImGui.GetMainViewport().Size;

            // ====== STATIC ==========
            List<StaticVfxGroup> Groups = new List<StaticVfxGroup>();
            foreach( KeyValuePair<IntPtr, StaticData> entry in StaticVfxs ) {
                IntPtr addr = IntPtr.Add( entry.Key, 0x50 );

                byte[] x = new byte[4];
                byte[] y = new byte[4];
                byte[] z = new byte[4];
                Marshal.Copy( addr, x, 0, 4 );
                Marshal.Copy( addr + 0x4, y, 0, 4 );
                Marshal.Copy( addr + 0x8, z, 0, 4 );

                var pos = new SharpDX.Vector3
                {
                    X = BitConverter.ToSingle( x, 0 ),
                    Y = BitConverter.ToSingle( y, 0 ),
                    Z = BitConverter.ToSingle( z, 0 )
                };
                Groups.Add( new StaticVfxGroup
                {
                    path = entry.Value.path,
                    position = pos
                } );
            }

            int i = 0;
            foreach(var group in Groups.GroupBy( i => i.position, i => i.path, CloseComp ) ) {
                HashSet<string> paths = new HashSet<string>( group );
                // ==== CHECK WINDOW POSITION ======
                if( !playPos.HasValue || !_plugin.PluginInterface.Framework.Gui.WorldToScreen( group.Key, out var screenCoords ) ) continue;
                string longestPath = "";
                foreach( var item in paths ) {
                    if( item.Length > longestPath.Length ) {
                        longestPath = item;
                    }
                }
                var windowSize = GetWindowSize( longestPath );
                if( screenCoords.X + windowSize.X > screenPos.X + screenSize.X || screenCoords.Y + windowSize.Y > screenPos.Y + screenSize.Y ) continue;
                // ======== CHECK DISTANCE ============
                var d = Distance( playPos.Value, group.Key );
                if(d > 100f ) {
                    continue;
                }


                StartDraw( new Vector2( screenCoords.X, screenCoords.Y ), i );
                foreach( var item in paths ) {
                    Draw( item, i );
                    i++;
                }
                EndDraw();

                i++;
            }
            // ====== ACTOR =======
            var actorTable = _plugin.PluginInterface.ClientState.Actors;
            if( actorTable == null || ActorVfxs.Count == 0 ) {
                return;
            }
            Dictionary<IntPtr, List<string>> ActorToVfxs = new Dictionary<IntPtr, List<string>>();
            foreach( KeyValuePair<IntPtr, ActorData> entry in ActorVfxs ) {
                if( !ActorToVfxs.ContainsKey( entry.Value.actor ) ) {
                    ActorToVfxs[entry.Value.actor] = new List<string>();
                }
                ActorToVfxs[entry.Value.actor].Add( entry.Value.path );
            }
            foreach( var actor in actorTable ) {
                if( actor == null ) continue;
                if( _plugin.PluginInterface.ClientState.LocalPlayer == null ) continue;

                var result = ActorToVfxs.TryGetValue( actor.Address, out var paths );
                if( !result ) continue;

                var pos = new SharpDX.Vector3
                {
                    X = actor.Position.X,
                    Y = actor.Position.Z + 2,
                    Z = actor.Position.Y
                };

                // ===== CHECK WINDOW POSITION =========
                if( !playPos.HasValue || !_plugin.PluginInterface.Framework.Gui.WorldToScreen( pos, out var screenCoords ) ) continue;
                string longestPath = "";
                foreach(var path in paths ) {
                    if(path.Length > longestPath.Length ) {
                        longestPath = path;
                    }
                }
                var windowSize = GetWindowSize( longestPath );
                if( screenCoords.X + windowSize.X > screenPos.X + screenSize.X || screenCoords.Y + windowSize.Y > screenPos.Y + screenSize.Y ) continue; // (https://github.com/goatcorp/Dalamud/pull/277/files)
                // ======== CHECK DISTANCE ============
                var d = Distance( playPos.Value, pos );
                if( d > 100f ) {
                    continue;
                }

                StartDraw( new Vector2( screenCoords.X, screenCoords.Y ), i );
                foreach(string path in paths ) {
                    Draw( path, i );
                    i++;
                }
                EndDraw();
            }
        }

        public void StartDraw(Vector2 pos, int idx ) {
            ImGui.SetNextWindowPos( new Vector2( pos.X, pos.Y ) );
            ImGui.SetNextWindowBgAlpha( 0.5f );

            ImGui.Begin( $"vfx-{idx}",
                ImGuiWindowFlags.NoDecoration |
                ImGuiWindowFlags.AlwaysAutoResize |
                ImGuiWindowFlags.NoSavedSettings |
                ImGuiWindowFlags.NoMove |
                ImGuiWindowFlags.NoDocking |
                ImGuiWindowFlags.NoFocusOnAppearing |
                ImGuiWindowFlags.NoNav );
        }
        public void Draw(string path, int idx ) {
            ImGui.Text( $"{path}" );
            ImGui.SameLine();
            if( ImGui.Button( $"COPY##vfx-{idx}" ) ) {
                ImGui.SetClipboardText( path );
            }
        }

        public Vector2 GetWindowSize(string text) {
            return ImGui.CalcTextSize( text ) + ImGui.GetStyle().WindowPadding + new Vector2( 150, 10 ); // to account of "COPY" button, plus some extra padding
        }

        public void EndDraw() {
            ImGui.End();
        }

        public void Reset() {
            ActorVfxs = new ConcurrentDictionary<IntPtr, ActorData>();
            StaticVfxs = new ConcurrentDictionary<IntPtr, StaticData>();
        }

        public static float Distance(Vector3 p1, SharpDX.Vector3 p2 ) {
            return (float) Math.Sqrt(Math.Pow( p1.X - p2.X, 2 ) + Math.Pow( p1.Y - p2.Z, 2 ) + Math.Pow( p1.Z - p2.Y, 2 ));
        }
    }
}
