using Dalamud.Bindings.ImGui;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.Utils;

namespace VfxEditor.Select.Penumbra {
    public class SelectPenumbraTab : SelectTab<SelectPenumbraTabItem, PenumbraMod> {
        public SelectPenumbraTab( SelectDialog dialog ) : base( dialog, "Penumbra", "Penumbra-Shared" ) { }

        protected override bool IsDisabled() => !Plugin.PenumbraIpc.PenumbraEnabled;

        // Don't need to worry about doing this async
        // Also fine to keep getting the mods every frame, since it could change
        public override void Load() {
            LoadData();
            State.WaitingForItems = false;
            State.ItemsLoaded = true;
        }

        public override void LoadData() {
            Items.Clear();
            Items.AddRange( Plugin.PenumbraIpc.GetMods().Select( x => new SelectPenumbraTabItem( x ) ) );
        }

        // $"{group} {option}" -> (gamePath, localPath)
        public override void LoadSelection( SelectPenumbraTabItem item, out PenumbraMod loaded ) {
            loaded = new();
            PenumbraUtils.LoadFromName( item.Name, Dialog.Extensions, out loaded );
        }

        protected override void DrawSelected() {
            if( Loaded.Meta != null ) {
                ImGui.TextDisabled( $"by {Loaded.Meta.Author}" );
            }

            var files = Dialog.ShowLocal ? Loaded.SourceFiles : Loaded.ReplaceFiles;
            if( files != null ) {
                Dialog.DrawPaths( files, Selected.Name, SelectResultType.Local );
            }
        }
    }
}
