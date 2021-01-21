using AVFXLib.Models;
using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VFXEditor.UI.VFX
{
    public abstract class UIDropdownView<T> : UIBase where T : UINode {
        public string Id;
        public string defaultText;
        public AVFXBase AVFX;
        public UINodeGroup<T> Group;
        public T Selected = null;

        public bool AllowNew;
        public bool AllowDelete;

        public UIDropdownView( AVFXBase avfx, string _id, string _defaultText, bool allowNew = true, bool allowDelete = true )
        {
            AVFX = avfx;
            Id = _id;
            defaultText = _defaultText;
            AllowNew = allowNew;
            AllowDelete = allowDelete;
        }

        public abstract T OnNew();
        public abstract void OnDelete( T item );
        public abstract byte[] OnExport( T item );
        public virtual void OnSelect( T item ) { }
        public abstract T OnImport( AVFXLib.AVFX.AVFXNode node );

        public override void Draw( string parentId = "" )
        {
            ViewSelect();
            if( AllowNew )
            {
                ImGui.SameLine();
                if( ImGui.SmallButton( "+ New" + Id ) )
                {
                    ImGui.OpenPopup( "New_Popup" + Id );
                }
                if( ImGui.BeginPopup( "New_Popup" + Id ) )
                {
                    if( ImGui.Selectable( "Create" + Id ) )
                    {
                        Group.Add( OnNew() );
                    }
                    if( ImGui.Selectable("Load" + Id ) )
                    {
                        Load();
                    }
                    ImGui.EndPopup();
                }
            }
            if( Selected != null && AllowDelete )
            {
                ImGui.SameLine();
                if( ImGui.SmallButton( "Save" + Id ) )
                {
                    Save( Selected );
                }
                ImGui.SameLine();
                if( UIUtils.RemoveButton( "Delete" + Id, small:true) )
                {
                    Group.Remove( Selected );
                    Selected.DeleteNode();
                    OnDelete( Selected );
                    Selected = null;
                }
            }
            ImGui.Separator();
            // ====================
            if( Selected != null )
            {
                Selected.DrawBody( Id );
            }
        }
        // ========================
        public void ViewSelect() {
            var selectedString = (Selected != null) ? Selected.GetText() : defaultText;
            if( ImGui.BeginCombo( "Select" + Id, selectedString ) ) {
                for( int idx = 0; idx < Group.Items.Count; idx++ ) {
                    var item = Group.Items[idx];
                    if( ImGui.Selectable( item.GetText() + Id, Selected == item ) ) {
                        Selected = item;
                        OnSelect( item );
                    }
                }
                ImGui.EndCombo();
            }
        }
        // ========= DIALOGS ==================
        public void Load()
        {
            Task.Run( async () => {
                var picker = new OpenFileDialog
                {
                    Filter = "Partial AVFX (*.vfxedit)|*.vfxedit*|All files (*.*)|*.*",
                    Title = "Select a File Location.",
                    CheckFileExists = true
                };
                var result = await picker.ShowDialogAsync();
                if( result == DialogResult.OK ) {
                    try {
                        var node = AVFXLib.Main.Reader.readAVFX( picker.FileName, out List<string> messages );
                        foreach( string message in messages ) {
                            PluginLog.Log( message );
                        }
                        Group.Add( OnImport( node ));
                    }
                    catch( Exception ex ) {
                        PluginLog.LogError( ex, "Could not select a file" );
                    }
                }
            } );
        }

        public void Save( T item)
        {
            Task.Run( async () => {
                var picker = new SaveFileDialog
                {
                    Filter = "Partial AVFX (*.vfxedit)|*.vfxedit*|All files (*.*)|*.*",
                    Title = "Select a Save Location.",
                    DefaultExt = "vfxedit",
                    AddExtension = true
                };
                var result = await picker.ShowDialogAsync();
                if( result == DialogResult.OK ) {
                    try {
                        var data = OnExport( item );
                        File.WriteAllBytes( picker.FileName, data );
                    }
                    catch( Exception ex ) {
                        PluginLog.LogError( ex, "Could not select a file" );
                    }
                }
            } );
        }
    }
}
