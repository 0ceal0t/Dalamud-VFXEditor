using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.AvfxFormat {
    public class AvfxTimelineItem : GenericWorkspaceItem {
        public readonly AvfxTimeline Timeline;

        public readonly AvfxBool Enabled = new( "Enabled", "bEna" );
        public readonly AvfxInt StartTime = new( "Start Time", "StTm" );
        public readonly AvfxInt EndTime = new( "End Time", "EdTm" );
        public readonly AvfxInt BinderIdx = new( "Binder Index", "BdNo", defaultValue: -1 );
        public readonly AvfxInt EffectorIdx = new( "Effector Index", "EfNo", defaultValue: -1 );
        public readonly AvfxInt EmitterIdx = new( "Emitter Index", "EmNo", defaultValue: -1 );
        public readonly AvfxInt Platform = new( "Platform", "Plfm" );
        public readonly AvfxInt ClipIdx = new( "Clip Index", "ClNo" );

        private readonly List<AvfxBase> Parsed;

        public UiNodeSelect<AvfxBinder> BinderSelect;
        public UiNodeSelect<AvfxEmitter> EmitterSelect;
        public UiNodeSelect<AvfxEffector> EffectorSelect;

        private readonly List<IUiItem> Display;

        public AvfxTimelineItem( AvfxTimeline timeline, bool initNodeSelects ) {
            Timeline = timeline;

            Parsed = new List<AvfxBase> {
                Enabled,
                StartTime,
                EndTime,
                BinderIdx,
                EffectorIdx,
                EmitterIdx,
                Platform,
                ClipIdx
            };
            AvfxBase.RecurseAssigned( Parsed, false );

            if( initNodeSelects ) InitializeNodeSelects();

            Display = new() {
                Enabled,
                StartTime,
                EndTime,
                Platform
            };
        }

        public AvfxTimelineItem( AvfxTimeline timeline, bool initNodeSelects, byte[] data ) : this( timeline, initNodeSelects ) {
            using var buffer = new MemoryStream( data );
            using var reader = new BinaryReader( buffer );
            AvfxBase.ReadNested( reader, Parsed, data.Length );
        }

        public void InitializeNodeSelects() {
            BinderSelect = new UiNodeSelect<AvfxBinder>( Timeline, "Target Binder", Timeline.NodeGroups.Binders, BinderIdx );
            EmitterSelect = new UiNodeSelect<AvfxEmitter>( Timeline, "Target Emitter", Timeline.NodeGroups.Emitters, EmitterIdx );
            EffectorSelect = new UiNodeSelect<AvfxEffector>( Timeline, "Target Effector", Timeline.NodeGroups.Effectors, EffectorIdx );
        }

        public void Write( BinaryWriter writer ) => AvfxBase.WriteNested( writer, Parsed );

        public override void Draw() {
            using var _ = ImRaii.PushId( "Item" );
            DrawRename();

            BinderSelect.Draw();
            EmitterSelect.Draw();
            EffectorSelect.Draw();
            AvfxBase.DrawItems( Display );

            var clipAssigned = ClipIdx.IsAssigned();
            if( ImGui.Checkbox( "Clip Enabled", ref clipAssigned ) ) CommandManager.Avfx.Add( new AvfxAssignCommand( ClipIdx, clipAssigned ) );
            ClipIdx.Draw();
        }

        public override string GetDefaultText() {
            if( EmitterIdx.GetValue() != -1 ) return EmitterSelect.GetText();

            if( ClipIdx.IsAssigned() && ClipIdx.GetValue() != -1 ) {
                if( ClipIdx.GetValue() < Timeline.Clips.Count ) return Timeline.Clips[ClipIdx.GetValue()].GetText();
                return $"Clip {ClipIdx.GetValue()}";
            }

            return "[NONE]";
        }

        public override string GetWorkspaceId() => $"{Timeline.GetWorkspaceId()}/Item{GetIdx()}";

        public AvfxEmitter Emitter => EmitterSelect.Selected;

        public bool HasSound => EmitterSelect.Selected != null && EmitterSelect.Selected.HasSound;
    }
}
