using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using ImGuiNET;
using OtterGui.Raii;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using Vec2 = System.Numerics.Vector2;
using Vec3 = System.Numerics.Vector3;

namespace VfxEditor.Tracker {
    public struct TrackerItem {
        public string Path;
        public bool Removed;
    }

    public struct TrackerItemWithPosition {
        public TrackerItem Item;
        public Vector3 Position;
    }

    public unsafe class TrackerManager {
        private static bool WatchingCutscene => Dalamud.ClientState != null && Dalamud.Condition[ConditionFlag.OccupiedInCutSceneEvent] || Dalamud.Condition[ConditionFlag.WatchingCutscene78];

        private static readonly ClosenessComparator Comparator = new();

        public bool Enabled { get; private set; }
        public bool AnyEnabled => EnabledTrackers.Any();

        public readonly VfxTracker Vfx = new();
        public readonly TmbTracker Tmb = new();
        public readonly SklbTracker Sklb = new();

        private List<Tracker> Trackers => new( new Tracker[] {
            Vfx,
            Tmb,
            Sklb,
        } );

        private IEnumerable<Tracker> EnabledTrackers => Trackers.Where( x => x.Enabled );

        public TrackerManager() { }

        public void Draw() {
            if( !AnyEnabled ) return;
            using var _ = ImRaii.PushId( "Tracker" );

            var playerPosition = Plugin.PlayerObject?.Position;
            if( !playerPosition.HasValue ) return;
            var windowPosition = ImGuiHelpers.MainViewport.Pos;
            var removeTime = DateTime.Now.AddSeconds( -1 * Plugin.Configuration.OverlayRemoveDelay );

            foreach( var tracker in EnabledTrackers ) tracker.RemoveStale( removeTime );

            if( WatchingCutscene ) {
                var paths = new HashSet<TrackerItem>();
                foreach( var tracker in EnabledTrackers ) tracker.PopulateAll( paths );

                var pos = windowPosition + new Vec2( 15, 15 );
                DrawOverlayItems( pos, paths, 0 );
                return;
            }

            var floatingItems = new List<TrackerItemWithPosition>();
            var actorIdToItems = new Dictionary<int, HashSet<TrackerItem>>();
            var addressToItems = new Dictionary<IntPtr, HashSet<TrackerItem>>();

            foreach( var tracker in EnabledTrackers ) {
                tracker.Populate( floatingItems, actorIdToItems, addressToItems );
            }

            var camera = CameraManager.Instance->GetActiveCamera();
            if( camera == null ) return;
            var sceneCamera = camera->CameraBase.SceneCamera;

            var idx = 0;

            foreach( var group in floatingItems.GroupBy( item => item.Position, item => item.Item, Comparator ) ) {
                var paths = new HashSet<TrackerItem>( group );

                if( !sceneCamera.WorldToScreen( new() {
                    X = group.Key.X,
                    Y = group.Key.Y,
                    Z = group.Key.Z,
                }, out var screenPos ) ) continue;

                if( Distance( playerPosition.Value, group.Key ) > 100f && Plugin.Configuration.OverlayLimit ) continue;

                DrawOverlayItems( new Vec2( screenPos.X, screenPos.Y ), paths, idx );

                idx++;
            }

            var actorTable = Dalamud.Objects;
            if( actorTable == null ) return;
            foreach( var actor in actorTable ) {
                if( actor == null ) continue;
                if( Plugin.PlayerObject == null ) continue;

                var paths = new HashSet<TrackerItem>();

                if( actorIdToItems.TryGetValue( ( int )actor.ObjectId, out var _p1 ) ) {
                    foreach( var p in _p1 ) paths.Add( p );
                }

                if( addressToItems.TryGetValue( actor.Address, out var _p2 ) ) {
                    foreach( var p in _p2 ) paths.Add( p );
                }

                if( paths.Count == 0 ) continue;

                var pos = new Vector3 {
                    X = actor.Position.X,
                    Y = actor.Position.Y,
                    Z = actor.Position.Z
                };

                if( !sceneCamera.WorldToScreen( new() {
                    X = actor.Position.X,
                    Y = actor.Position.Y,
                    Z = actor.Position.Z,
                }, out var screenPos ) ) continue;

                if( Distance( playerPosition.Value, pos ) > 100f && Plugin.Configuration.OverlayLimit ) continue;

                DrawOverlayItems( new Vec2( screenPos.X, screenPos.Y ), paths, idx );

                idx++;
            }
        }

        private static void DrawOverlayItems( Vec2 pos, HashSet<TrackerItem> items, int idx ) {
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

            ImGui.SetNextWindowPos( pos, ImGuiCond.Always );
            ImGui.SetNextWindowBgAlpha( 0.5f );
            ImGuiHelpers.ForceNextWindowMainViewport();

            if( ImGui.Begin( $"TrackerWindow{idx}",
                ImGuiWindowFlags.NoDecoration |
                ImGuiWindowFlags.AlwaysAutoResize |
                ImGuiWindowFlags.NoSavedSettings |
                ImGuiWindowFlags.NoMove |
                ImGuiWindowFlags.NoDocking |
                ImGuiWindowFlags.NoFocusOnAppearing |
                ImGuiWindowFlags.NoNav ) ) {

                using var _ = ImRaii.PushId( $"Tracker{idx}" );

                var i = 0;
                foreach( var item in items ) {
                    if( i >= maxDisplay ) break;

                    using var __ = ImRaii.PushId( i );

                    if( item.Removed ) ImGui.TextDisabled( item.Path );
                    else ImGui.Text( item.Path );

                    ImGui.SameLine( largestSize.X + 20 );
                    if( ImGui.Button( "COPY" ) ) ImGui.SetClipboardText( item.Path );
                    i++;
                }
            }
            ImGui.End();
        }

        private class ClosenessComparator : IEqualityComparer<Vector3> {
            public bool Equals( Vector3 x, Vector3 y ) => ( x - y ).Length() < 2;
            public int GetHashCode( Vector3 obj ) => 0;
        }

        private static float Distance( Vec3 p1, Vector3 p2 ) => ( float )Math.Sqrt( Math.Pow( p1.X - p2.X, 2 ) + Math.Pow( p1.Y - p2.Y, 2 ) + Math.Pow( p1.Z - p2.Z, 2 ) );

        private static Matrix GetMatrix( out float width, out float height ) {
            // Setup the matrix
            var matrixSingleton = Plugin.ResourceLoader.GetMatrixSingleton();
            var viewProjectionMatrix = new Matrix();
            unsafe {
                var rawMatrix = ( float* )( matrixSingleton + 0x1b4 + ( 0x13c * 0 ) ).ToPointer(); // 0 = projection idx
                for( var i = 0; i < 16; i++, rawMatrix++ )
                    viewProjectionMatrix[i] = *rawMatrix;
                width = *rawMatrix;
                height = *( rawMatrix + 1 );
            }
            return viewProjectionMatrix;
        }

        private static bool WorldToScreen( float height, float width, ref Matrix viewProjectionMatrix, Vec2 windowPos, Vector3 worldPos, out Vector2 screenPos ) {
            Vector3.Transform( ref worldPos, ref viewProjectionMatrix, out SharpDX.Vector3 pCoords );
            screenPos = new Vector2( pCoords.X / pCoords.Z, pCoords.Y / pCoords.Z );
            screenPos.X = 0.5f * width * ( screenPos.X + 1f ) + windowPos.X;
            screenPos.Y = 0.5f * height * ( 1f - screenPos.Y ) + windowPos.Y;
            return pCoords.Z > 0 && screenPos.X > windowPos.X && screenPos.X < windowPos.X + width && screenPos.Y > windowPos.Y && screenPos.Y < windowPos.Y + height;
        }
    }
}
