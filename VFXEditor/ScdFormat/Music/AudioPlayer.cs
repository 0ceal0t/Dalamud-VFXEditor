using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiFileDialog;
using ImGuiNET;
using NAudio.Wave;
using System;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using VfxEditor.ScdFormat.Music.Data;
using VfxEditor.Utils;

namespace VfxEditor.ScdFormat
{
    public class AudioPlayer {
        private readonly ScdAudioEntry Entry;
        private PlaybackState State => CurrentOutput == null ? PlaybackState.Stopped : CurrentOutput.PlaybackState;
        private PlaybackState PrevState = PlaybackState.Stopped;

        private WaveStream LeftStream;
        private WaveStream RightStream;
        private IWaveProvider Volume;
        private MultiplexingWaveProvider LeftRightCombined;
        private WasapiOut CurrentOutput;

        private double TotalTime => LeftStream?.TotalTime == null ? 0 : LeftStream.TotalTime.TotalSeconds - 0.01f;
        private double CurrentTime => LeftStream?.CurrentTime == null ? 0 : LeftStream.CurrentTime.TotalSeconds;

        private bool IsVorbis => Entry.Format == SscfWaveFormat.Vorbis;

        private int ConverterSamplesOut = 0;
        private int ConverterSecondsOut = 0;
        private int ConverterSamples = 0;
        private float ConverterSeconds = 0f;

        private bool LoopTimeInitialized = false;
        private bool LoopTimeRefreshing = false;
        private double LoopStartTime = 0;
        private double LoopEndTime = 0;

        private double QueueSeek = -1;

        private bool ShowChannelSelect => Entry.NumChannels > 2;
        private int Channel1 = 0;
        private int Channel2 = 1;

        public AudioPlayer( ScdAudioEntry entry ) {
            Entry = entry;
        }

        public void Draw( string id) {
            if( ImGui.BeginTabBar( $"{id}/Tabs" ) ) {
                if( ImGui.BeginTabItem( $"Music{id}" ) ) {
                    DrawPlayer( id );
                    ImGui.EndTabItem();
                }
                if( ShowChannelSelect && ImGui.BeginTabItem( $"Channels{id}" ) ) {
                    DrawChannels( id );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( $"Converter{id}" ) ) {
                    DrawConverter( id );
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }

            var currentState = State;
            var justQueued = false;

            if( currentState == PlaybackState.Stopped && PrevState == PlaybackState.Playing &&
                ( ( IsVorbis && Plugin.Configuration.LoopMusic ) || ( !IsVorbis && Plugin.Configuration.LoopSoundEffects ) ) ) {
                PluginLog.Log( "Looping..." );
                Play();
                if( !Entry.NoLoop && Plugin.Configuration.SimulateScdLoop && LoopTimeInitialized && LoopStartTime > 0 ) {
                    if( QueueSeek == -1 ) {
                        QueueSeek = LoopStartTime;
                        justQueued = true;
                    }
                }
            }
            else if( currentState == PlaybackState.Playing && !Entry.NoLoop && Plugin.Configuration.SimulateScdLoop && LoopTimeInitialized && Math.Abs( LoopEndTime - CurrentTime ) < 0.03f ) {
                if( QueueSeek == -1 ) {
                    QueueSeek = LoopStartTime;
                    justQueued = true;
                }
            }

            if( currentState == PlaybackState.Playing && QueueSeek != -1 && !justQueued ) {
                LeftStream.CurrentTime = TimeSpan.FromSeconds( QueueSeek );
                RightStream.CurrentTime = TimeSpan.FromSeconds( QueueSeek );
                QueueSeek = -1;
            }

            PrevState = currentState;
        }

        private void DrawPlayer( string id ) {
            // Controls
            ImGui.PushFont( UiBuilder.IconFont );
            if( State == PlaybackState.Stopped ) {
                if( ImGui.Button( $"{( char )FontAwesomeIcon.Play}" + id ) ) Play();
            }
            else if( State == PlaybackState.Playing ) {
                if( ImGui.Button( $"{( char )FontAwesomeIcon.Pause}" + id ) ) CurrentOutput.Pause();
            }
            else if( State == PlaybackState.Paused ) {
                if( ImGui.Button( $"{( char )FontAwesomeIcon.Play}" + id ) ) CurrentOutput.Play();
            }

            ImGui.PopFont();

            if( State == PlaybackState.Stopped ) ImGui.PushStyleVar( ImGuiStyleVar.Alpha, 0.5f );
            var selectedTime = ( float )CurrentTime;
            ImGui.SameLine( 25f );
            ImGui.SetNextItemWidth( 221f );
            var drawPos = ImGui.GetCursorScreenPos();
            if( ImGui.SliderFloat( $"{id}-Drag", ref selectedTime, 0, ( float )TotalTime ) ) {
                if( State != PlaybackState.Stopped && selectedTime > 0 && selectedTime < TotalTime ) {
                    CurrentOutput.Pause();
                    LeftStream.CurrentTime = TimeSpan.FromSeconds( selectedTime );
                    RightStream.CurrentTime = TimeSpan.FromSeconds( selectedTime );
                }
            }
            if( State == PlaybackState.Stopped ) ImGui.PopStyleVar();

            if( State != PlaybackState.Stopped && !Entry.NoLoop && LoopTimeInitialized && Plugin.Configuration.SimulateScdLoop ) {
                var startX = 221f * ( LoopStartTime / TotalTime );
                var endX = 221f * ( LoopEndTime / TotalTime );

                var startPos = drawPos + new Vector2( ( float )startX - 2, 0 );
                var endPos = drawPos + new Vector2( ( float )endX - 2, 0 );

                var height = ImGui.GetFrameHeight();

                var drawList = ImGui.GetWindowDrawList();
                drawList.AddRectFilled( startPos, startPos + new Vector2( 4, height ), 0xFFFF0000, 1 );
                drawList.AddRectFilled( endPos, endPos + new Vector2( 4, height ), 0xFFFF0000, 1 );
            }

            // Save
            ImGui.SameLine();
            ImGui.PushFont( UiBuilder.IconFont );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Download}" + id ) ) {
                if( IsVorbis ) ImGui.OpenPopup( "SavePopup" + id );
                else SaveWaveDialog();
            }
            ImGui.PopFont();
            UiUtils.Tooltip( "Export sound file to .wav or .ogg" );

            if( ImGui.BeginPopup( "SavePopup" + id ) ) {
                if( ImGui.Selectable( ".wav" ) ) SaveWaveDialog();
                if( ImGui.Selectable( ".ogg" ) ) SaveOggDialog();
                ImGui.EndPopup();
            }

            // Import
            ImGui.PushStyleVar( ImGuiStyleVar.ItemSpacing, new Vector2( 3, 4 ) );
            ImGui.SameLine();
            ImGui.PushFont( UiBuilder.IconFont );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Upload}" + id ) ) ImportDialog();
            ImGui.PopFont();
            ImGui.PopStyleVar( 1 );
            UiUtils.Tooltip( "Replace sound file" );

            var loopStartEnd = new int[2] { Entry.LoopStart, Entry.LoopEnd };
            ImGui.SetNextItemWidth( 246f );
            if( ImGui.InputInt2( $"{id}/LoopStartEnd", ref loopStartEnd[0] ) ) {
                Entry.LoopStart = loopStartEnd[0];
                Entry.LoopEnd = loopStartEnd[1];
            }

            ImGui.SameLine();
            if( UiUtils.DisabledButton( $"Update{id}", Plugin.Configuration.SimulateScdLoop ) ) {
                RefreshLoopStartEndTime();
            }
            ImGui.SameLine();
            ImGui.Text( "Loop Start/End (Bytes)" );

            ImGui.TextDisabled( $"{Entry.Format} / {Entry.NumChannels} Ch / {Entry.SampleRate}Hz / 0x{Entry.DataLength:X8} bytes" );
        }

        private void DrawChannels( string id ) {
            ImGui.TextDisabled( "Which channels to play when previewing the audio file. Does not affect the .scd file" );
            
            if( ImGui.BeginCombo( $"Preview Channel 1{id}", $"Channel #{Channel1}" ) ) {
                for( var i = 0; i < Entry.NumChannels; i++ ) {
                    if( ImGui.Selectable( $"Channel #{i}{id}", Channel1 == i ) ) Channel1 = i;
                }
                ImGui.EndCombo();
            }

            if( ImGui.BeginCombo( $"Preview Channel 2{id}", $"Channel #{Channel2}" ) ) {
                for( var i = 0; i < Entry.NumChannels; i++ ) {
                    if( ImGui.Selectable( $"Channel #{i}{id}", Channel2 == i ) ) Channel2 = i;
                }
                ImGui.EndCombo();
            }

            if( ImGui.Button( $"Update{id}" ) ) {
                Reset();
            }
        }

        private void DrawConverter( string id ) {
            ImGui.TextDisabled( "Utilities to generate byte values which can be used for loop start/end" );

            // Bytes
            ImGui.SetNextItemWidth( 100 ); ImGui.InputInt( $"{id}/SamplesIn", ref ConverterSamples, 0, 0 );
            ImGui.SameLine();
            ImGui.PushFont( UiBuilder.IconFont ); ImGui.Text( $"{( char )FontAwesomeIcon.ArrowRight}" ); ImGui.PopFont();
            ImGui.SameLine();
            ImGui.SetNextItemWidth( 100 ); ImGui.InputInt( $"{id}/SamplesOut", ref ConverterSamplesOut, 0, 0, ImGuiInputTextFlags.ReadOnly );
            ImGui.SameLine();
            if( ImGui.Button( $"Samples to Bytes{id}" ) ) {
                ConverterSamplesOut = Entry.Data.SamplesToBytes( ConverterSamples );
            }

            // Time
            ImGui.SetNextItemWidth( 100 ); ImGui.InputFloat( $"{id}/SecondsIn", ref ConverterSeconds, 0, 0 );
            ImGui.SameLine();
            ImGui.PushFont( UiBuilder.IconFont ); ImGui.Text( $"{( char )FontAwesomeIcon.ArrowRight}" ); ImGui.PopFont();
            ImGui.SameLine();
            ImGui.SetNextItemWidth( 100 ); ImGui.InputInt( $"{id}/SecondsOut", ref ConverterSecondsOut, 0, 0, ImGuiInputTextFlags.ReadOnly );
            ImGui.SameLine();
            if( ImGui.Button( $"Seconds to Bytes{id}" ) ) {
                ConverterSecondsOut = Entry.Data.TimeToBytes( ConverterSeconds );
            }
        }

        private void Play() {
            Reset();
            try {
                if( !LoopTimeInitialized ) RefreshLoopStartEndTime();

                var stream = Entry.Data.GetStream();
                var format = stream.WaveFormat;
                LeftStream = ConvertStream( stream );
                RightStream = ConvertStream( Entry.Data.GetStream() );

                var firstChannel = ShowChannelSelect ? Channel1 : 0;
                var secondChannel = ShowChannelSelect ? Channel2 : ( format.Channels > 1 ? 1 : 0 );

                var leftStreamIsolated = new MultiplexingWaveProvider( new IWaveProvider[] { LeftStream }, 1 );
                leftStreamIsolated.ConnectInputToOutput( firstChannel, 0 );

                var rightStreamIsolated = new MultiplexingWaveProvider( new IWaveProvider[] { RightStream }, 1 );
                rightStreamIsolated.ConnectInputToOutput( secondChannel, 0 );

                LeftRightCombined = new MultiplexingWaveProvider( new[] { leftStreamIsolated, rightStreamIsolated }, 2 );
                LeftRightCombined.ConnectInputToOutput( 0, 0 );
                LeftRightCombined.ConnectInputToOutput( 1, 1 );

                if( format.Encoding == WaveFormatEncoding.IeeeFloat ) {
                    var floatVolume = new WaveFloatTo16Provider( LeftRightCombined ) {
                        Volume = Plugin.Configuration.ScdVolume
                    };
                    Volume = floatVolume;
                }
                else {
                    var pcmVolume = new VolumeWaveProvider16( LeftRightCombined ) {
                        Volume = Plugin.Configuration.ScdVolume
                    };
                    Volume = pcmVolume;
                }

                CurrentOutput = new WasapiOut();
                CurrentOutput.Init( Volume );
                CurrentOutput.Play();
            }
            catch( Exception e ) {
                PluginLog.LogError( e, "Error playing sound" );
            }
        }

        public void UpdateVolume() {
            if( Volume == null ) return;
            if( Volume is  WaveFloatTo16Provider floatVolume ) {
                floatVolume.Volume = Plugin.Configuration.ScdVolume;
            }
            else if( Volume is VolumeWaveProvider16 pcmVolume ) {
                pcmVolume.Volume = Plugin.Configuration.ScdVolume;
            }
        }

        private void ImportDialog() {
            var text = IsVorbis ? "Audio files{.ogg,.wav},.*" : "Audio files{.wav},.*";
            FileDialogManager.OpenFileDialog( "Import File", text, ( bool ok, string res ) => {
                if( ok ) ScdFile.Import( res, Entry );
            } );
        }

        private void SaveWaveDialog() {
            FileDialogManager.SaveFileDialog( "Select a Save Location", ".wav", "ExportedSound", "wav", ( bool ok, string res ) => {
                if( ok ) {
                    using var stream = Entry.Data.GetStream();
                    WaveFileWriter.CreateWaveFile( res, stream );
                }
            } );
        }

        private void SaveOggDialog() {
            FileDialogManager.SaveFileDialog( "Select a Save Location", ".ogg", "ExportedSound", "ogg", ( bool ok, string res ) => {
                if( ok ) {
                    var data = ( ScdVorbis )Entry.Data;
                    File.WriteAllBytes( res, data.DecodedData );
                }
            } );
        }

        private static WaveStream ConvertStream( WaveStream stream ) => stream.WaveFormat.Encoding switch {
            WaveFormatEncoding.Pcm => WaveFormatConversionStream.CreatePcmStream( stream ),
            WaveFormatEncoding.Adpcm => WaveFormatConversionStream.CreatePcmStream( stream ),
            _ => stream
        };

        private async void RefreshLoopStartEndTime() {
            if( LoopTimeRefreshing ) return;
            LoopTimeRefreshing = true;
            await Task.Run( () => {
                Entry.Data.BytesToLoopStartEnd( Entry.LoopStart, Entry.LoopEnd, out LoopStartTime, out LoopEndTime );
                LoopTimeInitialized = true;
                LoopTimeRefreshing = false;
            } );
        }

        public void Reset() {
            CurrentOutput?.Stop();
            CurrentOutput?.Dispose();
            LeftStream?.Dispose();
            RightStream?.Dispose();
            LeftStream = null;
            RightStream = null;
            CurrentOutput = null;
        }

        public void Dispose() {
            CurrentOutput?.Stop();
            Reset();
        }
    }
}
