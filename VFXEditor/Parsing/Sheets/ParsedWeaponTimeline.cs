using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Data;

namespace VfxEditor.Parsing.Sheets {
    public class ParsedWeaponTimeline : ParsedSimpleBase<ushort, int> {
        private static bool IsInitialized = false;
        private static readonly Dictionary<ushort, string> Timelines = new();

        private static void Init() {
            if( IsInitialized ) return;
            IsInitialized = true;

            foreach( var item in Plugin.DataManager.GetExcelSheet<WeaponTimeline>() ) {
                Timelines[( ushort )item.RowId] = item.File.ToString();
            }
        }

        // =================

        private string CurrentTimeline => Timelines.TryGetValue( Value, out var timeline ) ? timeline : "";

        public ParsedWeaponTimeline( string name ) : base( name ) {
            Init();
        }

        public override void Read( BinaryReader reader, int size ) => Read( reader, 0 );

        public override void Read( BinaryReader reader ) {
            Value = reader.ReadUInt16();
        }

        public override void Write( BinaryWriter writer ) => writer.Write( Value );

        public override void Draw( CommandManager manager ) {
            Copy( manager );

            DrawCombo( manager );

            ImGui.SameLine();
            ImGui.Text( Name );
        }

        private void DrawCombo( CommandManager manager ) {
            using var combo = ImRaii.Combo( "##Combo", $"[{Value}] {CurrentTimeline}" );
            if( !combo ) return;

            foreach( var option in Timelines ) {
                var selected = option.Key == Value;

                if( ImGui.Selectable( $"[{option.Key}] {option.Value}", selected ) ) {
                    manager.Add( new ParsedSimpleCommand<ushort>( this, option.Key ) );
                }

                if( selected ) ImGui.SetItemDefaultFocus();
            }
        }

        protected override Dictionary<string, int> GetCopyMap( CopyManager manager ) => manager.Ints;

        protected override int ToCopy() => Value;

        protected override ushort FromCopy( int val ) => ( ushort )val;
    }
}
