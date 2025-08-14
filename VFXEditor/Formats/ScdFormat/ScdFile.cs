using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VfxEditor.FileManager;
using VfxEditor.Formats.ScdFormat.Utils;
using VfxEditor.ScdFormat.Music;
using VfxEditor.ScdFormat.Music.Data;
using VfxEditor.Ui.Components.SplitViews;
using VfxEditor.Utils;

namespace VfxEditor.ScdFormat {
    public class ScdFile : FileManagerFile {
        private readonly ScdHeader Header;

        public readonly List<ScdAudioEntry> Audio = [];
        public readonly List<ScdSoundEntry> Sounds = [];
        private List<ScdLayoutEntry> Layouts => Sounds.Select( x => x.Layout ).ToList();
        public readonly List<ScdTrackEntry> Tracks = [];
        public readonly List<ScdAttributeEntry> Attributes = [];

        public readonly ScdAudioEntrySplitView AudioSplitView;
        public readonly CommandSplitView<ScdSoundEntry> SoundView;
        public readonly CommandSplitView<ScdTrackEntry> TrackView;
        public readonly UiSplitView<ScdAttributeEntry> AttributeView;

        private readonly short UnknownOffset;
        private readonly int EofPaddingSize;

        public ScdFile( BinaryReader reader, bool verify ) : base() {
            Header = new( reader );
            var offsets = new ScdReader( reader );
            UnknownOffset = offsets.UnknownOffset;
            EofPaddingSize = offsets.EofPaddingSize;

            // The acutal sound effect/music data
            foreach( var offset in offsets.AudioOffsets.Where( x => x != 0 ) ) {
                var newAudio = new ScdAudioEntry( this );
                newAudio.Read( reader, offset );
                Audio.Add( newAudio );
            }

            var layouts = new List<ScdLayoutEntry>();
            foreach( var offset in offsets.LayoutOffsets.Where( x => x != 0 ) ) {
                var newLayout = new ScdLayoutEntry();
                newLayout.Read( reader, offset );
                layouts.Add( newLayout );
            }

            foreach( var offset in offsets.TrackOffsets.Where( x => x != 0 ) ) {
                var newTrack = new ScdTrackEntry();
                newTrack.Read( reader, offset );
                Tracks.Add( newTrack );
            }

            foreach( var offset in offsets.AttributeOffsets.Where( x => x != 0 ) ) {
                var newAttribute = new ScdAttributeEntry();
                newAttribute.Read( reader, offset );
                Attributes.Add( newAttribute );
            }

            foreach( var (offset, index) in offsets.SoundOffsets.Where( x => x != 0 ).WithIndex() ) {
                var newSound = new ScdSoundEntry( layouts[index] );
                newSound.Read( reader, offset );
                Sounds.Add( newSound );
            }

            if( verify ) Verified = FileUtils.Verify( reader, ToBytes() );
            if( offsets.Modded || Audio.Any( x => x.Data is ScdVorbis vorbis && vorbis.LegacyImported ) ) {
                Verified = VerifiedStatus.UNSUPPORTED;
            }

            AudioSplitView = new( Audio );
            SoundView = new( "Sound", Sounds, true, null, () => new ScdSoundEntry() );
            TrackView = new( "Track", Tracks, false, null, () => new ScdTrackEntry() );
            AttributeView = new( "Attribute", Attributes, false );
        }

        public override void Draw() {
            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            DrawAudio();
            DrawSounds();
            DrawTracks();
            DrawAttributes();
        }

        private void DrawAudio() {
            using var tabItem = ImRaii.TabItem( "Audio" );
            if( !tabItem ) return;

            using var _ = ImRaii.PushId( "Audio" );

            if( ImGui.CollapsingHeader( "Settings" ) ) {
                using var indent = ImRaii.PushIndent( 10f );

                ImGui.TextDisabled( "Audio player settings. These do not have any effect on the .scd file" );
                if( ImGui.Checkbox( "Loop Music", ref Plugin.Configuration.LoopMusic ) ) Plugin.Configuration.Save();
                if( ImGui.Checkbox( "Loop Sound Effects", ref Plugin.Configuration.LoopSoundEffects ) ) Plugin.Configuration.Save();
                if( ImGui.Checkbox( "Simulate Loop Start/End", ref Plugin.Configuration.SimulateScdLoop ) ) Plugin.Configuration.Save();
                ImGui.SetNextItemWidth( 50 );
                if( ImGui.InputFloat( "Volume", ref Plugin.Configuration.ScdVolume ) ) {
                    Plugin.Configuration.Save();
                    Audio.ForEach( x => x.Player.UpdateVolume() );
                }
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 1 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 1 );

            AudioSplitView.Draw();
        }

        private void DrawSounds() {
            using var tabItem = ImRaii.TabItem( "Sounds" );
            if( !tabItem ) return;

            using var _ = ImRaii.PushId( "Sounds" );
            SoundView.Draw();
        }

        private void DrawTracks() {
            using var tabItem = ImRaii.TabItem( "Tracks" );
            if( !tabItem ) return;

            using var _ = ImRaii.PushId( "Tracks" );
            TrackView.Draw();
        }

        private void DrawAttributes() {
            using var tabItem = ImRaii.TabItem( "Attributes" );
            if( !tabItem ) return;

            using var _ = ImRaii.PushId( "Attributes" );
            AttributeView.Draw();
        }

        public override void Write( BinaryWriter writer ) {
            Header.Write( writer );

            writer.Write( ( short )Sounds.Count );
            writer.Write( ( short )Tracks.Count );
            writer.Write( ( short )Audio.Count );
            writer.Write( UnknownOffset );

            var placeholders = writer.BaseStream.Position;
            writer.Write( 0 ); // track
            writer.Write( 0 ); // audio
            writer.Write( 0 ); // layout
            writer.Write( 0 ); // routing
            writer.Write( 0 ); // attribute
            writer.Write( EofPaddingSize );

            var soundOffset = PopulateOffsetPlaceholders( writer, Sounds, false );
            var trackOffset = PopulateOffsetPlaceholders( writer, Tracks, false );
            var audioOffset = PopulateOffsetPlaceholders( writer, Audio, false );
            var layoutOffset = PopulateOffsetPlaceholders( writer, Layouts, false );
            var attributeOffset = PopulateOffsetPlaceholders( writer, Attributes, true );

            // Update placeholders
            var savePos = writer.BaseStream.Position;
            writer.BaseStream.Position = placeholders;
            writer.Write( trackOffset );
            writer.Write( audioOffset );
            writer.Write( layoutOffset );
            writer.Write( 0 ); // routing
            writer.Write( attributeOffset );
            writer.BaseStream.Position = savePos;

            UpdateOffsets( writer, Sounds.Select( x => x.Layout ).ToList(), layoutOffset, ( BinaryWriter bw, ScdLayoutEntry item ) => {
                item.Write( writer );
            } );
            FileUtils.PadTo( writer, 16 );

            UpdateOffsets( writer, Sounds, soundOffset, ( BinaryWriter bw, ScdSoundEntry item ) => {
                item.Write( writer );
            } );
            FileUtils.PadTo( writer, 16 );

            UpdateOffsets( writer, Tracks, trackOffset, ( BinaryWriter bw, ScdTrackEntry item ) => {
                item.Write( writer );
            } );
            FileUtils.PadTo( writer, 16 );

            UpdateOffsets( writer, Attributes, attributeOffset, ( BinaryWriter bw, ScdAttributeEntry item ) => {
                item.Write( writer );
            } );
            FileUtils.PadTo( writer, 16 );

            // Sounds
            long paddingSubtract = 0;
            UpdateOffsets( writer, Audio, audioOffset, ( BinaryWriter bw, ScdAudioEntry music ) => {
                music.Write( writer, out var padding );
                paddingSubtract += padding;
            } );
            if( ( paddingSubtract % 16 ) > 0 ) paddingSubtract -= paddingSubtract % 16;

            ScdHeader.UpdateFileSize( writer, paddingSubtract ); // end with this
        }

        public void Replace( ScdAudioEntry entry, ScdAudioEntry newEntry ) {
            var index = Audio.IndexOf( entry );
            if( index == -1 || entry == newEntry || entry == null || newEntry == null ) return;
            Audio.Remove( entry );
            Audio.Insert( index, newEntry );
            entry.Player.Dispose();
        }

        public override void Dispose() => Audio.ForEach( x => x.Dispose() );

        public async void Import( string path, ScdAudioEntry entry ) {
            await Task.Run( () => {
                var newEntry = entry.Data switch {
                    ScdAdpcm _ => ( Path.GetExtension( path ) == ".wav" ) ? ScdAdpcm.ImportWav( path, entry ) : ScdVorbis.ImportOgg( path, entry ),
                    ScdVorbis _ => ( Path.GetExtension( path ) == ".wav" ) ? ScdVorbis.ImportWav( path, entry ) : ScdVorbis.ImportOgg( path, entry ),
                    _ => null
                };

                if( newEntry != null ) Replace( entry, newEntry );
            } );
        }

        private static int PopulateOffsetPlaceholders<T>( BinaryWriter writer, List<T> items, bool defaultZero ) {
            if( items.Count == 0 ) return defaultZero ? 0 : ( int )writer.BaseStream.Position;
            var offset = writer.BaseStream.Position;
            foreach( var _ in items ) writer.Write( 0 );
            FileUtils.PadTo( writer, 16 );
            return ( int )offset;
        }

        private static void UpdateOffsets<T>( BinaryWriter writer, List<T> items, int offsetLocation, Action<BinaryWriter, T> action ) where T : ScdEntry {
            List<int> positions = [];
            foreach( var item in items ) {
                positions.Add( ( int )writer.BaseStream.Position );
                action.Invoke( writer, item );
            }
            var savePos = writer.BaseStream.Position;

            writer.BaseStream.Position = offsetLocation;
            foreach( var position in positions ) {
                writer.Write( position );
            }

            writer.BaseStream.Position = savePos;
        }
    }
}
