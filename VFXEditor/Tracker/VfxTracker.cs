using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface;
using ImGuiNET;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VfxEditor.Structs.Vfx;

namespace VfxEditor.Tracker {
    public unsafe class VfxTracker {
        private class TrackerData {
            public string Path;
            public VfxStruct* Vfx;
            public bool IsChecked = false;
            public int ActorId = -1;
            public bool Removed = false;
            public DateTime RemovedTime;

            public VfxDisplayItem ToDisplayItem() => new() {
                Path = Path,
                Removed = Removed
            };
        }

        private struct StaticVfxGroup {
            public VfxDisplayItem Path;
            public SharpDX.Vector3 Position;
        }

        private struct VfxDisplayItem {
            public string Path;
            public bool Removed;
        }

        private static readonly ClosenessComp CloseComp = new();

        public bool Enabled { get; private set; }
        private readonly ConcurrentDictionary<IntPtr, TrackerData> ActorVfxs = new();
        private readonly ConcurrentDictionary<IntPtr, TrackerData> StaticVfxs = new();

        public VfxTracker() {
        }

        public void Reset() {
            ActorVfxs.Clear();
            StaticVfxs.Clear();
        }

        public void Toggle() {
            Enabled = !Enabled;
            if( !Enabled ) Reset();
        }

        public void AddActor( VfxStruct* vfx, string path ) {
            if( !Enabled ) return;
            var data = new TrackerData() {
                Path = path,
                Vfx = vfx
            };
            ActorVfxs.TryAdd( new IntPtr( vfx ), data );
        }

        public void RemoveActor( VfxStruct* vfx ) {
            if( !Enabled ) return;
            if( ActorVfxs.TryGetValue( new IntPtr( vfx ), out var item ) ) {
                item.Removed = true;
                item.RemovedTime = DateTime.Now;
            }
        }

        public void AddStatic( VfxStruct* vfx, string path ) {
            if( !Enabled ) return;
            var data = new TrackerData() {
                Path = path,
                Vfx = vfx
            };
            StaticVfxs.TryAdd( new IntPtr( vfx ), data );
        }

        public void RemoveStatic( VfxStruct* vfx ) {
            if( !Enabled ) return;
            if( StaticVfxs.TryGetValue( new IntPtr( vfx ), out var item ) ) {
                item.Removed = true;
                item.RemovedTime = DateTime.Now;
            }
        }

        public void Draw() {
            if( !Enabled ) return;
            var playPos = Plugin.ClientState?.LocalPlayer?.Position;
            if( !playPos.HasValue ) return;
            var windowPos = ImGuiHelpers.MainViewport.Pos;

            var removeTime = DateTime.Now.AddSeconds( -1 * Plugin.Configuration.OverlayRemoveDelay );
            foreach( var item in StaticVfxs.Where( x => x.Value.Removed && x.Value.RemovedTime < removeTime ).ToList() ) StaticVfxs.Remove( item.Key, out var _ );
            foreach( var item in ActorVfxs.Where( x => x.Value.Removed && x.Value.RemovedTime < removeTime ).ToList() ) ActorVfxs.Remove( item.Key, out var _ );

            // Give up in positions in cutscene
            if( WatchingCutscene ) {
                var paths = new HashSet<VfxDisplayItem>();
                foreach( var item in ActorVfxs ) item.Value.ToDisplayItem();
                foreach( var item in StaticVfxs ) item.Value.ToDisplayItem();

                var pos = windowPos + new Vector2( 15, 15 );
                DrawOverlayItems( pos, paths, 0 );
                return;
            }

            // Setup the matrix
            var matrix = GetMatrix(out var width, out var height);

            var vfxsWithoutActor = new List<StaticVfxGroup>(); // static vfxs without an actor
            var actorIdToVfxPath = new Dictionary<int, HashSet<VfxDisplayItem>>(); // either one with an actor

            // Static
            foreach( var item in StaticVfxs ) {
                if( item.Key == IntPtr.Zero ) continue;

                var vfx = item.Value;
                if( !vfx.IsChecked ) { // try to identify actor
                    var actorId = ChooseId( vfx.Vfx->StaticCaster, vfx.Vfx->StaticTarget );

                    vfx.IsChecked = true;
                    vfx.ActorId = actorId;
                }

                if( vfx.ActorId > 0 ) {
                    if( !actorIdToVfxPath.ContainsKey( vfx.ActorId ) ) actorIdToVfxPath[vfx.ActorId] = new HashSet<VfxDisplayItem>();
                    actorIdToVfxPath[vfx.ActorId].Add( vfx.ToDisplayItem() );
                }
                else { // add to groups
                    var pos = vfx.Vfx->Position;
                    vfxsWithoutActor.Add( new StaticVfxGroup {
                        Path = vfx.ToDisplayItem(),
                        Position = new SharpDX.Vector3( pos.X, pos.Y, pos.Z )
                    } );
                }
            }

            // Actors
            foreach( var item in ActorVfxs ) {
                if( item.Key == IntPtr.Zero ) continue;

                var vfx = item.Value;
                if( !vfx.IsChecked ) {
                    var actorId = ChooseId( vfx.Vfx->ActorCaster, vfx.Vfx->ActorTarget );

                    vfx.IsChecked = true;
                    vfx.ActorId = actorId;
                }

                if( vfx.ActorId > 0 ) { // add to actor to vfxs
                    if( !actorIdToVfxPath.ContainsKey( vfx.ActorId ) ) actorIdToVfxPath[vfx.ActorId] = new HashSet<VfxDisplayItem>();
                    actorIdToVfxPath[vfx.ActorId].Add( vfx.ToDisplayItem() );
                }
            }

            // Draw groups
            var idx = 0;
            foreach( var group in vfxsWithoutActor.GroupBy( item => item.Position, item => item.Path, CloseComp ) ) {
                var paths = new HashSet<VfxDisplayItem>( group );

                if( !WorldToScreen( height, width, ref matrix, windowPos, group.Key, out var screenCoords ) ) continue;
                if( Distance( playPos.Value, group.Key ) > 100f && Plugin.Configuration.OverlayLimit ) continue;
                DrawOverlayItems( new Vector2( screenCoords.X, screenCoords.Y ), paths, idx );
                idx++;
            }

            // Draw Actors
            var actorTable = Plugin.Objects;
            if( actorTable == null ) return;
            foreach( var actor in actorTable ) {
                if( actor == null ) continue;
                if( Plugin.ClientState.LocalPlayer == null ) continue;

                var result = actorIdToVfxPath.TryGetValue( ( int )actor.ObjectId, out var paths );
                if( !result ) continue;

                var pos = new SharpDX.Vector3 {
                    X = actor.Position.X,
                    Y = actor.Position.Y,
                    Z = actor.Position.Z
                };

                if( !WorldToScreen( height, width, ref matrix, windowPos, pos, out var screenCoords ) ) continue;
                if( Distance( playPos.Value, pos ) > 100f && Plugin.Configuration.OverlayLimit ) continue;
                DrawOverlayItems( new Vector2( screenCoords.X, screenCoords.Y ), paths, idx );
                idx++;
            }
        }

        private static void DrawOverlayItems( Vector2 pos, HashSet<VfxDisplayItem> items, int idx ) {
            var longestString = "";
            foreach( var item in items ) {
                if( item.Path.Length > longestString.Length ) longestString = item.Path;
            }
            var screenPos = ImGui.GetMainViewport().Pos;
            var screenSize = ImGui.GetMainViewport().Size;
            var windowSize = ImGui.CalcTextSize( longestString );
            var largestSize = windowSize;
            windowSize.X += ImGui.GetStyle().WindowPadding.X + 100; // account for "COPY" button
            windowSize.Y += ImGui.GetStyle().WindowPadding.Y + 10;
            if( pos.X + windowSize.X > screenPos.X + screenSize.X || pos.Y + windowSize.Y > screenPos.Y + screenSize.Y ) return;

            var maxDisplay = Math.Floor( ( screenPos.Y + screenSize.Y - pos.Y ) / windowSize.Y ); // how many can we fit vertically?

            ImGui.SetNextWindowPos( new Vector2( pos.X, pos.Y ) );
            ImGui.SetNextWindowBgAlpha( 0.5f );
            ImGuiHelpers.ForceNextWindowMainViewport();

            if( ImGui.Begin( $"vfx-{idx}",
                ImGuiWindowFlags.NoDecoration |
                ImGuiWindowFlags.AlwaysAutoResize |
                ImGuiWindowFlags.NoSavedSettings |
                ImGuiWindowFlags.NoMove |
                ImGuiWindowFlags.NoDocking |
                ImGuiWindowFlags.NoFocusOnAppearing |
                ImGuiWindowFlags.NoNav ) ) {
                var i = 0;
                foreach( var item in items ) {
                    if( i >= maxDisplay ) break;

                    if( item.Removed ) ImGui.TextDisabled( item.Path );
                    else ImGui.Text( item.Path );

                    ImGui.SameLine( largestSize.X + 20 );
                    if( ImGui.Button( $"COPY##vfx-{idx}-{i}" ) ) ImGui.SetClipboardText( item.Path );
                    i++;
                }
            }
            ImGui.End();
        }

        private static bool WatchingCutscene => Plugin.ClientState != null && Plugin.Condition[ConditionFlag.OccupiedInCutSceneEvent] || Plugin.Condition[ConditionFlag.WatchingCutscene78];

        private class ClosenessComp : IEqualityComparer<SharpDX.Vector3> {
            public bool Equals( SharpDX.Vector3 x, SharpDX.Vector3 y ) => ( x - y ).Length() < 2;
            public int GetHashCode( SharpDX.Vector3 obj ) => 0;
        }

        private static int ChooseId( int caster, int target ) => target > 0 ? target : ( caster > 0 ? caster : -1 );

        private static float Distance( Vector3 p1, SharpDX.Vector3 p2 ) => ( float )Math.Sqrt( Math.Pow( p1.X - p2.X, 2 ) + Math.Pow( p1.Y - p2.Y, 2 ) + Math.Pow( p1.Z - p2.Z, 2 ) );

        private static SharpDX.Matrix GetMatrix(out float width, out float height) {
            // Setup the matrix
            var matrixSingleton = Plugin.ResourceLoader.GetMatrixSingleton();
            var viewProjectionMatrix = new SharpDX.Matrix();
            unsafe {
                var rawMatrix = ( float* )( matrixSingleton + 0x1b4 + ( 0x13c * 0 ) ).ToPointer(); // 0 = projection idx
                for( var i = 0; i < 16; i++, rawMatrix++ )
                    viewProjectionMatrix[i] = *rawMatrix;
                width = *rawMatrix;
                height = *( rawMatrix + 1 );
            }
            return viewProjectionMatrix;
        }

        private static bool WorldToScreen( float height, float width, ref SharpDX.Matrix viewProjectionMatrix, Vector2 windowPos, SharpDX.Vector3 worldPos, out SharpDX.Vector2 screenPos ) {
            SharpDX.Vector3.Transform( ref worldPos, ref viewProjectionMatrix, out SharpDX.Vector3 pCoords );
            screenPos = new SharpDX.Vector2( pCoords.X / pCoords.Z, pCoords.Y / pCoords.Z );
            screenPos.X = 0.5f * width * ( screenPos.X + 1f ) + windowPos.X;
            screenPos.Y = 0.5f * height * ( 1f - screenPos.Y ) + windowPos.Y;
            return pCoords.Z > 0 && screenPos.X > windowPos.X && screenPos.X < windowPos.X + width && screenPos.Y > windowPos.Y && screenPos.Y < windowPos.Y + height;
        }
    }
}
