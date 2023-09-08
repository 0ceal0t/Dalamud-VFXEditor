using OtterGui.Raii;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.ScdFormat {
    public enum TrackCmd {
        End,
        Volume,
        Pitch,
        Interval,
        Modulation,
        ReleaseRate,
        Panning,
        KeyOn,
        RandomVolume,
        RandomPitch,
        RandomPan,
        KeyOff = 0xC,
        LoopStart,
        LoopEnd,
        ExternalAudio,
        EndForLoop,
        AddInterval,
        Expression,
        Velocity,
        MidiVolume,
        MidiAddVolume,
        MidiPan,
        MidiAddPan,
        ModulationType,
        ModulationDepth,
        ModulationAddDepth,
        ModulationSpeed,
        ModulationAddSpeed,
        ModulationOff,
        PitchBend,
        Transpose,
        AddTranspose,
        FrPanning,
        RandomWait,
        Adsr,
        CutOff,
        Jump,
        PlayContinueLoop,
        Sweep,
        MidiKeyOnOld,
        SlurOn,
        SlurOff,
        AutoAdsrEnvelope,
        MidiExternalAudio,
        Marker,
        InitParams,
        Version,
        ReverbOn,
        ReverbOff,
        MidiKeyOn,
        PortamentoOn,
        PortamentoOff,
        MidiEnd,
        ClearKeyInfo,
        ModulationDepthFade,
        ModulationSpeedFade,
        AnalysisFlag,
        Config,
        Filter,
        PlayInnerSound,
        VolumeZeroOne,
        ZeroOneJump,
        ChannelVolumeZeroOne,
        Unknown64
    }

    public class ScdTrackItem : IUiItem {
        public readonly ParsedEnum<TrackCmd> Type = new( "Type", size: 2 );
        public ScdTrackData Data = null;

        public ScdTrackItem() {
            Type.ExtraCommandGenerator = () => {
                return new ScdTrackDataCommand( this );
            };
        }

        public void Read( BinaryReader reader ) {
            Type.Read( reader );

            UpdateData();
            Data?.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            Type.Write( writer );
            Data?.Write( writer );
        }

        public void UpdateData() {
            Data = Type.Value switch {
                TrackCmd.Volume => new TrackParamData(),
                TrackCmd.Pitch => new TrackParamData(),
                TrackCmd.Interval => new TrackIntData(),
                TrackCmd.Modulation => new TrackModulationData(),
                TrackCmd.ReleaseRate => new TrackIntData(),
                TrackCmd.Panning => new TrackParamData(),
                TrackCmd.RandomVolume => new TrackRandomData(),
                TrackCmd.RandomPitch => new TrackRandomData(),
                TrackCmd.RandomPan => new TrackRandomData(),
                TrackCmd.LoopStart => new TrackInt2Data(),
                TrackCmd.ExternalAudio => new TrackExternalAudioData(),
                TrackCmd.AddInterval => new TrackFloatData(),
                TrackCmd.Expression => new TrackFloat2Data(),
                TrackCmd.Velocity => new TrackFloatData(),
                TrackCmd.MidiVolume => new TrackFloat2Data(),
                TrackCmd.MidiAddVolume => new TrackFloatData(),
                TrackCmd.MidiPan => new TrackFloat2Data(),
                TrackCmd.MidiAddPan => new TrackFloatData(),
                TrackCmd.ModulationType => new TrackModulationTypeData(),
                TrackCmd.ModulationDepth => new TrackModulationDepthData(),
                TrackCmd.ModulationAddDepth => new TrackModulationDepthData(),
                TrackCmd.ModulationSpeed => new TrackModulationSpeedData(),
                TrackCmd.ModulationAddSpeed => new TrackModulationSpeedData(),
                TrackCmd.ModulationOff => new TrackModulationOffData(),
                TrackCmd.PitchBend => new TrackFloat2Data(),
                TrackCmd.Transpose => new TrackFloat2Data(),
                TrackCmd.AddTranspose => new TrackFloatData(),
                TrackCmd.FrPanning => new TrackParamData(),
                TrackCmd.RandomWait => new TrackInt2Data(),
                TrackCmd.Adsr => new TrackParamData(),
                TrackCmd.Jump => new TrackJumpData(),
                TrackCmd.Sweep => new TrackSweepData(),
                TrackCmd.AutoAdsrEnvelope => new TrackAutoAdsrEnvelopeData(),
                TrackCmd.MidiExternalAudio => new TrackExternalAudioData(),
                TrackCmd.Version => new TrackShortData(),
                TrackCmd.ReverbOn => new TrackFloatData(),
                TrackCmd.MidiKeyOn => new TrackFloat2Data(),
                TrackCmd.PortamentoOn => new TrackPortamentoData(),
                TrackCmd.ModulationDepthFade => new TrackModulationDepthFadeData(),
                TrackCmd.ModulationSpeedFade => new TrackModulationSpeedFadeData(),
                TrackCmd.AnalysisFlag => new TrackAnalysisFlagData(),
                TrackCmd.Config => new TrackConfigData(),
                TrackCmd.Filter => new TrackFilterData(),
                TrackCmd.PlayInnerSound => new TrackPlayInnerSoundData(),
                TrackCmd.VolumeZeroOne => new TrackVolumeZeroOneData(),
                TrackCmd.ZeroOneJump => new TrackIntData(),
                TrackCmd.ChannelVolumeZeroOne => new TrackChannelVolumeZeroOneData(),
                TrackCmd.Unknown64 => new TrackUnknown64Data(),
                _ => null
            };
        }

        public void Draw() {
            Type.Draw( CommandManager.Scd );
            using var _ = ImRaii.PushId( "Data" );
            Data?.Draw();
        }
    }
}
