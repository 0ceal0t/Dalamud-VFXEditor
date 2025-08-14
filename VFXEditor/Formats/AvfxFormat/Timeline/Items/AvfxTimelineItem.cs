using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.AvfxFormat.Assign;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.AvfxFormat {
    public class AvfxTimelineItem : GenericWorkspaceItem {
        public readonly AvfxTimeline Timeline;

        public readonly AvfxBool Enabled = new( "Enabled", "bEna" );
        public readonly AvfxInt StartTime = new( "Start Time", "StTm" );
        public readonly AvfxInt EndTime = new( "End Time", "EdTm" );
        public readonly AvfxInt BinderIdx = new( "Binder Index", "BdNo", value: -1 );
        public readonly AvfxInt EffectorIdx = new( "Effector Index", "EfNo", value: -1 );
        public readonly AvfxInt EmitterIdx = new( "Emitter Index", "EmNo", value: -1 );
        public readonly AvfxInt Platform = new( "Platform", "Plfm" );
        public readonly AvfxInt ClipIdx = new( "Clip Index", "ClNo" );

        private readonly List<AvfxBase> Parsed;

        public AvfxNodeSelect<AvfxBinder> BinderSelect;
        public AvfxNodeSelect<AvfxEmitter> EmitterSelect;
        public AvfxNodeSelect<AvfxEffector> EffectorSelect;

        private readonly List<IUiItem> Display;

        public AvfxTimelineItem( AvfxTimeline timeline, bool initNodeSelects ) {
            Timeline = timeline;

            Parsed = [
                Enabled,
                StartTime,
                EndTime,
                BinderIdx,
                EffectorIdx,
                EmitterIdx,
                Platform,
                ClipIdx
            ];
            foreach( var item in Parsed ) item.SetAssigned( false );

            if( initNodeSelects ) InitializeNodeSelects();

            Display = [
                Enabled,
                StartTime,
                EndTime,
                Platform
            ];
        }

        public AvfxTimelineItem( AvfxTimeline timeline, bool initNodeSelects, byte[] data ) : this( timeline, initNodeSelects ) {
            using var buffer = new MemoryStream( data );
            using var reader = new BinaryReader( buffer );
            AvfxBase.ReadNested( reader, Parsed, data.Length );
        }

        public void InitializeNodeSelects() {
            BinderSelect = new AvfxNodeSelect<AvfxBinder>( Timeline, "Target Binder", Timeline.NodeGroups.Binders, BinderIdx );
            EmitterSelect = new AvfxNodeSelect<AvfxEmitter>( Timeline, "Target Emitter", Timeline.NodeGroups.Emitters, EmitterIdx );
            EffectorSelect = new AvfxNodeSelect<AvfxEffector>( Timeline, "Target Effector", Timeline.NodeGroups.Effectors, EffectorIdx );
        }

        public void Write( BinaryWriter writer ) => AvfxBase.WriteNested( writer, Parsed );

        public override void Draw() {
            using var _ = ImRaii.PushId( "Item" );
            DrawRename();

            BinderSelect.Draw();
            EmitterSelect.Draw();
            EffectorSelect.Draw();
            AvfxBase.DrawItems( Display );

            var assigned = ClipIdx.IsAssigned();
            if( ImGui.Checkbox( "Clip Enabled", ref assigned ) ) {
                CommandManager.Add( new AvfxAssignCommand( ClipIdx, assigned ) );
            }
            ClipIdx.Draw();
        }

        public override string GetDefaultText() {
            if( EmitterIdx.Value != -1 ) return EmitterSelect.GetText();

            if( ClipIdx.IsAssigned() && ClipIdx.Value != -1 ) {
                if( ClipIdx.Value < Timeline.Clips.Count ) return Timeline.Clips[ClipIdx.Value].GetText();
                return $"Clip {ClipIdx.Value}";
            }

            if( EffectorIdx.IsAssigned() && EffectorSelect.Selected != null ) {
                return EffectorSelect.GetText();
            }

            return "[NONE]";
        }

        public override string GetWorkspaceId() => $"{Timeline.GetWorkspaceId()}/Item{GetIdx()}";

        public AvfxEmitter Emitter => EmitterSelect.Selected;

        public bool HasValue => EmitterIdx.Value >= 0 || ( ClipIdx.IsAssigned() && ClipIdx.Value >= 0 ) || ( EffectorIdx.IsAssigned() && EffectorSelect.Selected != null );
    }
}
