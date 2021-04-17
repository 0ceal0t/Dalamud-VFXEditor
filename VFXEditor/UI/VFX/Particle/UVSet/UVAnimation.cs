using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.Data.DirectX;
using VFXEditor.Data.Texture;

namespace VFXEditor.UI.VFX.Particle.UVSet {
    public class UVAnimation : UIItem {
        UIParticleUVSet UVSet;

        float CurrentRotation = 0;
        Vector2 CurrentScale = new Vector2( 1, 1 );
        Vector2 CurrentScroll = new Vector2( 0, 0 );

        Stopwatch timer;

        UVPreview _UVPreview;

        public UVAnimation(UIParticleUVSet uvSet ) {
            UVSet = uvSet;
            Assigned = true;
                        _UVPreview = DirectXManager.Manager._UVPreview;
            SetFrame( 0 );
            timer = new Stopwatch();
            timer.Stop();
        }

        public int Frame;
        public UITexture SelectedTexture = null;
        public UIModel SelectedModel = null;

        public override void DrawBody( string parentId ) {
            if( _UVPreview._CurrentUV != this ) {
                UpdatePreview();
                _UVPreview._CurrentUV = this;
            }

            if( SelectedModel != null && SelectedModel.IsDeleted ) SelectedModel = null;
            if( SelectedTexture != null && SelectedTexture.IsDeleted ) SelectedTexture = null;

            // ======== MODEL SELECT ========
            var modelText = SelectedModel == null ? "[NONE]" : SelectedModel.GetText();
            if(ImGui.BeginCombo("Model" + parentId, modelText ) ) {
                var idx = 0;
                foreach(var item in UINode._Models.Items ) {
                    if( ImGui.Selectable(item.GetText() + parentId + idx, item == SelectedModel) ) {
                        SelectedModel = item;
                        if(SelectedTexture != null ) {
                            UpdatePreview();
                        }
                    }
                    idx++;
                }
                ImGui.EndCombo();
            }
            // ======= TEXTURE SELECT ==========
            var texText = SelectedTexture == null ? "[NONE]" : SelectedTexture.GetText();
            if( ImGui.BeginCombo( "Texture" + parentId, texText ) ) {
                var idx = 0;
                foreach( var item in UINode._Textures.Items ) {
                    if( ImGui.Selectable( item.GetText() + parentId + idx, item == SelectedTexture ) ) {
                        SelectedTexture = item;
                        UpdateBitmap();
                        if( SelectedModel != null ) {
                            UpdatePreview();
                        }
                    }
                    idx++;
                }
                ImGui.EndCombo();
            }

            if(ImGui.InputInt("Frame" + parentId, ref Frame ) ) {
                if( Frame < 0 ) Frame = 0;
                SetFrame( Frame );
            }

            if(ImGui.Button("Play" + parentId ) ) {
                timer.Start();
            }
            ImGui.SameLine();
            if( ImGui.Button( "Stop" + parentId ) ) {
                timer.Stop();
            }
            ImGui.SameLine();
            if( ImGui.Button( "Reset" + parentId ) ) {
                timer.Reset();
                SetFrame( 0 );
            }

            if( timer.IsRunning ) {
                int _frame = ( int )( 15.0f * timer.ElapsedMilliseconds / 1000.0f );
                if( _frame != Frame ) {
                    Frame = _frame;
                    SetFrame( Frame );
                }
            }

            ImGui.BeginChild( "UVViewChild" );
            var space = ImGui.GetContentRegionAvail();
            _UVPreview.Resize( space );
            ImGui.ImageButton( _UVPreview.RenderShad.NativePointer, space, new Vector2( 0, 0 ), new Vector2( 1, 1 ), 0 );
            if( ImGui.IsItemActive() && ImGui.IsMouseDragging( ImGuiMouseButton.Left ) ) {
                var delta = ImGui.GetMouseDragDelta();
                _UVPreview.Drag( delta );
            }
            else {
                _UVPreview.IsDragging = false;
            }
            if( ImGui.IsItemHovered() ) {
                _UVPreview.Zoom( ImGui.GetIO().MouseWheel );
            }
            ImGui.EndChild();
        }

        Bitmap _bitmap;
        public void UpdateBitmap() {
            if( SelectedTexture == null ) return;
            var tex = SelectedTexture.Manager.GetTexture( SelectedTexture.Path.Literal.Value.Trim( '\0' ) );
            _bitmap = new Bitmap( tex.Header.Width, tex.Header.Height );
            for( int i = 0; i < tex.Header.Height; i++ ) {
                for( int j = 0; j < tex.Header.Width; j++ ) {
                    int _idx = ( i * tex.Header.Width + j ) * 4;
                    int r = tex.ImageData[_idx];
                    int g = tex.ImageData[_idx + 1];
                    int b = tex.ImageData[_idx + 2];
                    int a = tex.ImageData[_idx + 3];
                    _bitmap.SetPixel( j, i, Color.FromArgb( a, r, g, b ) );
                }
            }
        }

        public void UpdatePreview() {
            if( SelectedModel != null && SelectedTexture != null ) {
                _UVPreview.LoadModel( SelectedModel.Model );
                _UVPreview.LoadTexture( _bitmap );
                SetFrame( Frame );
            }
            else {
                _UVPreview.LoadModel( null );
            }
        }

        public void SetFrame(int frame ) {
            CurrentRotation = GetCurveValue( UVSet._Rotation, frame, 0 );
            CurrentScale = Get2AxisValue( UVSet._Scale, frame, new Vector2( 1, 1 ) );
            CurrentScroll = Get2AxisValue( UVSet._Scroll, frame, new Vector2( 0, 0 ) );

            if(_UVPreview._CurrentUV == this ) {
                _UVPreview.AnimData[0] = CurrentScale.X;
                _UVPreview.AnimData[1] = CurrentScale.Y;
                _UVPreview.AnimData[2] = CurrentScroll.X;
                _UVPreview.AnimData[3] = CurrentScroll.Y;
                _UVPreview.AnimData[4] = CurrentRotation;
            }
        }

        public static Vector2 Get2AxisValue(UICurve2Axis curve, int frame, Vector2 defaultValue) {
            if( !curve.Assigned ) return default;
            if( curve._AxisConnect.Literal.Value == AxisConnect.X_Y || curve._AxisConnect.Literal.Value == AxisConnect.X_YZ ) {
                var v = GetCurveValue( curve._X, frame, defaultValue.X );
                return new Vector2(v,v);
            }
            else if( curve._AxisConnect.Literal.Value == AxisConnect.Y_X || curve._AxisConnect.Literal.Value == AxisConnect.Y_XZ ) {
                var v = GetCurveValue( curve._Y, frame, defaultValue.Y );
                return new Vector2( v, v );
            }
            return new Vector2(
                GetCurveValue( curve._X, frame, defaultValue.X ),
                GetCurveValue( curve._Y, frame, defaultValue.Y )
            );
        }
        public static float GetCurveValue(UICurve curve, int frame, float defaultValue ) {
            return curve.Assigned && curve.Curve.Keys.Count > 0 ? curve.CurveEdit.GetAtTime( frame ) : defaultValue;
        }
        public static Vector2 MoveUV(Vector2 coord, Vector2 scale, Vector2 scroll, float rotate ) {
            return RotateUV(new Vector2(0.5f) + (coord - scroll - new Vector2(0.5f, 0.5f)) / scale, -rotate );
        }
        public static Vector2 RotateUV(Vector2 coord, float rotation ) {
            return new Vector2(
                (float)(Math.Cos(rotation) * (coord.X - 0.5) + Math.Sin(rotation) * (coord.Y - 0.5) + 0.5),
                (float)(Math.Cos(rotation) * (coord.Y - 0.5) - Math.Sin(rotation) * (coord.X - 0.5) + 0.5)
            );
        }

        public override string GetText() {
            return "UV Animation";
        }
    }
}
