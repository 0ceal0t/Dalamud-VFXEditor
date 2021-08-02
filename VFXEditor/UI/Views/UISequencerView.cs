using Dalamud.Interface;
using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.UI.VFX;

namespace VFXEditor.UI.Views {
    public abstract class UISequencerView<T> : UIBase where T : UIItem {
        public abstract int GetStart( T item );
        public abstract int GetEnd( T item );
        public abstract void SetStart( T item, int start );
        public abstract void SetEnd( T item, int end );
        public abstract void OnDelete( T item );
        public abstract T OnNew();

        public Plugin _plugin;
        public List<T> Items;

        public bool AllowNewDelete = true;

        public UISequencerView( List<T> _items, bool allowNewDelete = true ) {
            AllowNewDelete = allowNewDelete;
            Items = _items;
            SetupIdx();
        }

        public void SetupIdx() {
            for( var i = 0; i < Items.Count; i++ ) {
                Items[i].Idx = i;
            }
        }

        public float MinVisible = 0;
        public float MaxVisible = 60;
        public float Max = 60;
        public float Min = 0;

        private static readonly int LeftWidth = 125;
        private static readonly int HeaderHeight = 25;
        private static readonly int FooterHeight = 15;
        private static readonly int RowHeight = 20;
        private static readonly int GrabWidth = 10;

        private static readonly uint BlackLine_Color = ImGui.GetColorU32( new Vector4( 0.1f, 0.1f, 0.1f, 1 ) );
        private static readonly uint BlackLine_Color_Soft = ImGui.GetColorU32( new Vector4( 0.1f, 0.1f, 0.1f, 0.5f ) );
        private static readonly uint ContentColor_Odd = ImGui.GetColorU32( new Vector4( 0.43f, 0.43f, 0.43f, 0.5f ) );
        private static readonly uint ContentColor_Even = ImGui.GetColorU32( new Vector4( 0.23f, 0.23f, 0.23f, 0.4f ) );
        private static readonly uint ContentColor_HandleColor = ImGui.GetColorU32( new Vector4( 0.9f, 0.07f, 0, 1 ) );
        private static readonly uint BarColor = ImGui.GetColorU32( new Vector4( 0.5f, 0.5f, 0.7f, 1 ) );
        private static readonly uint BarColor_SelectedColor = ImGui.GetColorU32( new Vector4( 0.7f, 0.4f, 0.2f, 0.5f ) );
        private static readonly uint HeaderColor = ImGui.GetColorU32( new Vector4( 0.216f, 0.216f, 0.216f, 1 ) );
        private static readonly uint FooterColor = ImGui.GetColorU32( new Vector4( 0.133f, 0.133f, 0.133f, 1 ) );
        private static readonly uint FooterColor_ScrollColor = ImGui.GetColorU32( new Vector4( 0.314f, 0.314f, 0.314f, 1 ) );
        private static readonly uint FooterColor_HandleColor = ImGui.GetColorU32( new Vector4( 0.414f, 0.414f, 0.414f, 1 ) );
        private static readonly uint LeftColor = ImGui.GetColorU32( new Vector4( 0.133f, 0.133f, 0.133f, 1 ) );
        private static readonly uint BGColor2 = ImGui.GetColorU32( new Vector4( 0.133f, 0.133f, 0.133f, 1 ) );
        private static readonly uint LeftTextColor = ImGui.GetColorU32( new Vector4( 1, 1, 1, 1 ) );
        private static readonly uint LeftTextColor_SelectedColor = ImGui.GetColorU32( new Vector4( 1.0f, 0.6f, 0.4f, 1 ) );
        private static readonly uint TickColor_TextColor = ImGui.GetColorU32( new Vector4( 0.73f, 0.73f, 0.73f, 1 ) );
        private static readonly uint TickColor = ImGui.GetColorU32( new Vector4( 0.376f, 0.376f, 0.376f, 1 ) );

        private float ScrollY = 0;

        public T Selected = null;

        private enum DragState {
            None,
            Dragging,
            DraggingLeft,
            DraggingRight
        };

        private DragState Footer_Dragging = DragState.None;
        private Vector2 Footer_LastDrag;

        private DragState Content_Dragging = DragState.None;
        private Vector2 Content_LastDrag;

        private bool DrawOnce = false;
        public override void Draw( string parentId ) {
            var Id = parentId + "-Sequence";

            // ====== FIX UP BOUNDS ==========
            Max = 0;
            foreach( var item in Items ) {
                if( GetEnd( item ) > Max ) {
                    Max = GetEnd( item );
                }
            }
            if( Max <= 0 ) { // all "infinite"
                Max = 60;
            }
            if( Min < 0 ) {
                Min = 0;
            }
            if( MinVisible < 0 ) {
                MinVisible = 0;
            }
            if( MaxVisible > Max ) {
                MaxVisible = Max;
            }
            if( MinVisible < Min ) {
                MinVisible = Min;
            }

            if( !DrawOnce ) {
                DrawOnce = true;
                MaxVisible = Max * 0.9f;
            }

            var space = ImGui.GetContentRegionAvail();
            var Size = new Vector2( space.X, 200 );
            var DrawList = ImGui.GetWindowDrawList();

            ImGui.BeginGroup();
            ImGui.InvisibleButton( "##NodeEmpty", Size );
            var cursor = ImGui.GetCursorPos();

            var CanvasTopLeft = ImGui.GetItemRectMin();
            var CanvasBottomRight = ImGui.GetItemRectMax();
            DrawList.AddRectFilled( CanvasTopLeft, CanvasBottomRight, BGColor2 );

            var LeftPosition = new Vector2( CanvasTopLeft.X, CanvasTopLeft.Y + HeaderHeight );
            var TimelinePosition = new Vector2( CanvasTopLeft.X + LeftWidth, CanvasTopLeft.Y );
            var ContentPosition = new Vector2( CanvasTopLeft.X + LeftWidth, CanvasTopLeft.Y + HeaderHeight );
            var FooterPosition = new Vector2( CanvasTopLeft.X + LeftWidth + 5, CanvasBottomRight.Y - FooterHeight );

            var ContentSize = Size - new Vector2( LeftWidth, HeaderHeight + FooterHeight );
            var HeaderSize = new Vector2( Size.X, HeaderHeight );
            var TimelineSize = new Vector2( Size.X - LeftWidth, HeaderHeight );
            var LeftSize = new Vector2( LeftWidth, Size.Y - ( HeaderHeight + FooterHeight ) );
            var FooterSize = new Vector2( ContentSize.X - 15, FooterHeight );

            var Diff = MaxVisible - MinVisible;
            var PixelsPerFrame = TimelineSize.X / Diff;

            DrawList.AddRectFilled( CanvasTopLeft, CanvasTopLeft + HeaderSize, HeaderColor );
            DrawList.AddRectFilled( ContentPosition, ContentPosition + ContentSize, HeaderColor );
            DrawList.AddRectFilled( LeftPosition, LeftPosition + LeftSize, LeftColor );
            DrawList.AddRectFilled( FooterPosition, FooterPosition + FooterSize, FooterColor );

            // ===== NEW / DELETE =======
            if( AllowNewDelete ) {
                DrawList.AddText( UiBuilder.IconFont, 15, CanvasTopLeft + new Vector2( 10, 5 ), LeftTextColor, $"{( char )FontAwesomeIcon.PlusSquare}" );
                if( Selected != null ) {
                    DrawList.AddText( UiBuilder.IconFont, 15, CanvasTopLeft + new Vector2( 40, 5 ), ContentColor_HandleColor, $"{( char )FontAwesomeIcon.Trash}" );
                }
                if( ImGui.IsItemClicked( ImGuiMouseButton.Left ) ) {
                    var pos = ImGui.GetMousePos() - CanvasTopLeft;
                    if( pos.X >= 10 && pos.X <= 25 && pos.Y >= 5 && pos.Y <= 20 ) { // gross, but regular buttons dont' work for some reason
                        var item = OnNew();
                        if( item != null ) {
                            item.Idx = Items.Count;
                            Items.Add( item );
                        }
                    }
                    if( pos.X >= 40 && pos.X <= 55 && pos.Y >= 5 && pos.Y <= 20 ) {
                        Items.Remove( Selected );
                        OnDelete( Selected );
                        SetupIdx();
                        Selected = null;
                    }
                }
            }

            // ==== DRAW TIMELINE ====
            DrawList.PushClipRect( TimelinePosition, TimelinePosition + TimelineSize + new Vector2( 0, ContentSize.Y ), true );
            var _smallTick = 0.5f; // how many frames per tick?
            var _largeTick = 1.0f;
            if( PixelsPerFrame < 5 ) {
                _smallTick = 10;
                _largeTick = 50;
            }
            else if( PixelsPerFrame < 20 ) {
                _smallTick = 1;
                _largeTick = 10;
            }
            else if( PixelsPerFrame < 40 ) {
                _smallTick = 1;
                _largeTick = 5;
            }

            var _minSmall = MinVisible - ( MinVisible % _smallTick );
            var _maxSmall = MaxVisible - ( MaxVisible % _smallTick );
            var _minLarge = MinVisible - ( MinVisible % _largeTick );
            var _maxLarge = MaxVisible - ( MaxVisible % _largeTick );
            var _smallOffset = -( MinVisible % _smallTick );
            var _largeOffset = -( MinVisible % _largeTick );

            for( var i = 0; i <= ( _maxSmall - _minSmall ) / _smallTick; i++ ) {
                var position = TimelinePosition + new Vector2( ( _smallOffset + i * _smallTick ) * PixelsPerFrame, HeaderHeight );
                DrawList.AddLine( position + new Vector2( 0, ContentSize.Y ), position + new Vector2( 0, -7 ), TickColor, 1 );
            }
            for( var i = 0; i <= ( _maxLarge - _minLarge ) / _largeTick; i++ ) {
                var _time = _minLarge + i * _largeTick;
                var position = TimelinePosition + new Vector2( ( _largeOffset + i * _largeTick ) * PixelsPerFrame, HeaderHeight );
                DrawList.AddLine( position + new Vector2( 0, ContentSize.Y ), position + new Vector2( 0, -20 ), TickColor, 1 );
                DrawList.AddText( position + new Vector2( 5, -25 ), TickColor_TextColor, $"{_time}" );
            }

            DrawList.PopClipRect();

            // ===== DRAW CONTENT ============
            var row_delta = ImGui.GetMousePos().Y - LeftPosition.Y + ScrollY;
            var row_idx = ( int )Math.Floor( row_delta / RowHeight );
            if( row_idx >= Items.Count ) row_idx = -1;
            var content_hovering = Contained( ImGui.GetMousePos(), ContentPosition, ContentSize );

            DrawList.PushClipRect( ContentPosition, ContentPosition + ContentSize, true );
            for( var idx = 0; idx < Math.Ceiling( Math.Max( Items.Count, ContentSize.Y / RowHeight ) ); idx++ ) {
                var _position = ContentPosition + new Vector2( 0, -ScrollY + idx * RowHeight );
                DrawList.AddRectFilled( _position, _position + new Vector2( ContentSize.X, RowHeight ), idx % 2 == 0 ? ContentColor_Even : ContentColor_Odd ); // STRIPE
            }
            for( var idx = 0; idx < Items.Count; idx++ ) {
                var item = Items[idx];
                float _start = GetStart( item );
                float _end = GetEnd( item );
                var _isInfinite = GetEnd( item ) == -1;
                if( _isInfinite ) { // stretch it to the end
                    _end = MaxVisible;
                }
                var _startOffset = PixelsPerFrame * ( _start - MinVisible );
                var _endOffset = PixelsPerFrame * ( _end - MinVisible );
                var _position = ContentPosition + new Vector2( 0, -ScrollY + idx * RowHeight );
                DrawList.AddLine( _position, _position + new Vector2( ContentSize.X, 0 ), BlackLine_Color_Soft );
                if( item == Selected ) {
                    DrawList.AddRectFilled( _position, _position + new Vector2( ContentSize.X, RowHeight ), BarColor_SelectedColor ); // selected bar
                }
                DrawList.AddRectFilled( _position + new Vector2( _startOffset, 2 ), _position + new Vector2( _endOffset, RowHeight - 2 ), BarColor, 2 ); // regular bar

                // ==== HANDLES ====
                if( content_hovering && row_idx == idx && Content_Dragging == DragState.None ) {
                    var delta = ImGui.GetMousePos().X - ContentPosition.X;
                    if( !_isInfinite && delta >= _endOffset - GrabWidth && delta <= _endOffset + GrabWidth ) { // HOVER RIGHT
                        DrawList.AddLine( _position + new Vector2( _endOffset, 0 ), _position + new Vector2( _endOffset, RowHeight ), ContentColor_HandleColor, 3 );
                    }
                    else if( delta >= _startOffset - GrabWidth && delta <= _startOffset + GrabWidth ) { // HOVER LEFT
                        DrawList.AddLine( _position + new Vector2( _startOffset, 0 ), _position + new Vector2( _startOffset, RowHeight ), ContentColor_HandleColor, 3 );
                    }
                }
                else if( Content_Dragging == DragState.DraggingRight && Selected == item ) { // DRAG RIGHT
                    DrawList.AddLine( _position + new Vector2( _endOffset, 0 ), _position + new Vector2( _endOffset, RowHeight ), ContentColor_HandleColor, 3 );
                }
                else if( Content_Dragging == DragState.DraggingLeft && Selected == item ) { // DRAG LEFT
                    DrawList.AddLine( _position + new Vector2( _startOffset, 0 ), _position + new Vector2( _startOffset, RowHeight ), ContentColor_HandleColor, 3 );
                }

                if( _isInfinite ) {
                    DrawList.AddText( UiBuilder.IconFont, 12, _position + new Vector2( _endOffset - 25, 5 ), LeftTextColor, $"{( char )FontAwesomeIcon.Infinity}" );
                }

            }
            DrawList.PopClipRect();
            DrawList.AddLine( ContentPosition, ContentPosition + new Vector2( ContentSize.X, 0 ), BlackLine_Color );

            // ======= DRAW LEFT =========
            DrawList.PushClipRect( LeftPosition, LeftPosition + LeftSize, true );
            for( var idx = 0; idx < Items.Count; idx++ ) {
                var item = Items[idx];
                var _position = LeftPosition + new Vector2( 0, -ScrollY + idx * RowHeight );
                DrawList.AddText( _position + new Vector2( 5, 2 ), item == Selected ? LeftTextColor_SelectedColor : LeftTextColor, item.GetText() );
            }
            DrawList.PopClipRect();

            // ======= DRAW FOOTER =========
            var scrollWidth = FooterSize.X * ( MaxVisible - MinVisible ) / ( Max - Min );
            var offsetLeft = FooterSize.X * ( MinVisible ) / ( Max - Min );
            var scrollStartPosition = FooterPosition + new Vector2( offsetLeft, 0 );
            var scrollEndPosition = scrollStartPosition + new Vector2( scrollWidth, 0 );
            DrawList.AddRectFilled( scrollStartPosition + new Vector2( 0, 3 ), scrollStartPosition + new Vector2( scrollWidth, FooterHeight - 4 ), FooterColor_ScrollColor );
            DrawList.AddCircleFilled( scrollStartPosition + new Vector2( 0, 7 ), 6, FooterColor_HandleColor );
            DrawList.AddCircleFilled( scrollEndPosition + new Vector2( 0, 7 ), 6, FooterColor_HandleColor );

            // ==== CONTENT CHILD ======
            ImGui.SetCursorPos( ImGui.GetCursorPos() + new Vector2( LeftSize.X, -LeftSize.Y - FooterHeight - 4 ) );
            ImGui.BeginChild( Id + "-Content", ContentSize, false, ImGuiWindowFlags.NoScrollbar );
            ImGui.InvisibleButton( Id + " -ContentButton", new Vector2( ContentSize.X, RowHeight * Items.Count ) );

            if( content_hovering ) { // CONTENT SCROLLING
                ScrollY = ImGui.GetScrollY();
            }
            else {
                ImGui.SetScrollY( ScrollY );
            }

            if( ImGui.IsItemActive() ) {
                if( ImGui.IsMouseDragging( ImGuiMouseButton.Left ) && Selected != null ) {
                    var dragPos = ImGui.GetMouseDragDelta();
                    if( Content_Dragging == DragState.None ) { // START DRAGGING
                        var delta = ImGui.GetMousePos().X - ContentPosition.X;
                        var _left = ( GetStart( Selected ) - MinVisible ) * PixelsPerFrame;
                        var _isInfinite = GetEnd( Selected ) == -1;
                        var _right = _isInfinite ? ContentSize.X : ( GetEnd( Selected ) - MinVisible ) * PixelsPerFrame;
                        if( delta >= _left - GrabWidth && delta <= _left + GrabWidth ) {
                            Content_Dragging = DragState.DraggingLeft;
                        }
                        else if( !_isInfinite && delta >= _right - GrabWidth && delta <= _right + GrabWidth ) {
                            Content_Dragging = DragState.DraggingRight;
                        }
                        else if( delta < _right - GrabWidth && delta > _left + GrabWidth ) {
                            Content_Dragging = DragState.Dragging;
                        }
                    }
                    else { // CURRENTLY DRAGGING
                        var dX = dragPos.X - Content_LastDrag.X;
                        var moveFramesX = ( dX / ContentSize.X ) * ( MaxVisible - MinVisible );
                        var _isInfinite = GetEnd( Selected ) == -1;

                        if( Content_Dragging == DragState.Dragging ) {
                            moveFramesX -= Math.Min( GetStart( Selected ) + moveFramesX - Min, 0 );
                            if( !_isInfinite ) { // adjust max if needed
                                var increaseMax = Math.Max( GetEnd( Selected ) + moveFramesX - Max, 0 );
                                Max += increaseMax;
                                SetEnd( Selected, ( int )Math.Round( GetEnd( Selected ) + moveFramesX ) );
                            }
                            SetStart( Selected, ( int )Math.Round( GetStart( Selected ) + moveFramesX ) );
                        }
                        else if( Content_Dragging == DragState.DraggingLeft ) {
                            moveFramesX -= Math.Min( GetStart( Selected ) + moveFramesX - Min, 0 );
                            SetStart( Selected, ( int )Math.Round( GetStart( Selected ) + moveFramesX ) );
                        }
                        else if( Content_Dragging == DragState.DraggingRight ) {
                            var increaseMax = Math.Max( GetEnd( Selected ) + moveFramesX - Max, 0 );
                            Max += increaseMax;
                            SetEnd( Selected, ( int )Math.Round( GetEnd( Selected ) + moveFramesX ) );
                        }
                    }
                    Content_LastDrag = dragPos;
                }
                else if( ImGui.IsItemClicked() ) {
                    if( row_idx != -1 ) {
                        Selected = Items[row_idx];
                    }
                }
                else {
                    Content_Dragging = DragState.None;
                }
            }
            else {
                Content_Dragging = DragState.None;
            }
            ImGui.EndChild();
            ImGui.SetCursorPos( cursor );

            // ==== LEFT CHILD =======
            ImGui.SetCursorPos( ImGui.GetCursorPos() + new Vector2( 0, -LeftSize.Y - FooterHeight - 4 ) );
            ImGui.BeginChild( Id + "-Left", LeftSize, false );
            ImGui.InvisibleButton( Id + " -LeftButton", new Vector2( LeftSize.X, RowHeight * Items.Count ) );
            if( Contained( ImGui.GetMousePos(), LeftPosition, LeftSize ) ) {
                ScrollY = ImGui.GetScrollY();
            }
            else {
                ImGui.SetScrollY( ScrollY );
            }
            if( ImGui.IsItemClicked() ) {
                if( row_idx != -1 ) {
                    Selected = Items[row_idx];
                }
            }
            ImGui.EndChild();
            ImGui.SetCursorPos( cursor );

            // ===== FOOTER CHILD ======
            ImGui.SetCursorPos( ImGui.GetCursorPos() + new Vector2( LeftSize.X, -FooterHeight - 4 ) );
            ImGui.BeginChild( Id + "-Footer", FooterSize, false, ImGuiWindowFlags.NoScrollbar );
            ImGui.InvisibleButton( Id + " -FooterButton", FooterSize );
            if( ImGui.IsItemActive() ) {
                // TODO: click move
                if( ImGui.IsMouseDragging( ImGuiMouseButton.Left ) ) {
                    var dragPos = ImGui.GetMouseDragDelta();
                    if( Footer_Dragging == DragState.None ) { // START DRAGGING
                        var delta = ImGui.GetMousePos().X - FooterPosition.X;
                        var _left = offsetLeft;
                        var _right = offsetLeft + scrollWidth;
                        if( delta >= _left - GrabWidth && delta <= _left + GrabWidth ) {
                            Footer_Dragging = DragState.DraggingLeft;
                        }
                        else if( delta >= _right - GrabWidth && delta <= _right + GrabWidth ) {
                            Footer_Dragging = DragState.DraggingRight;
                        }
                        else if( delta < _right - GrabWidth && delta > _left + GrabWidth ) {
                            Footer_Dragging = DragState.Dragging;
                        }
                    }
                    else { // CURRENTLY DRAGGING
                        var dX = dragPos.X - Footer_LastDrag.X;
                        var moveFramesX = ( dX / FooterSize.X ) * ( Max - Min );
                        if( Footer_Dragging == DragState.Dragging ) { // MOVE BOTH TOGETHER
                            moveFramesX -= Math.Min( MinVisible + moveFramesX - Min, 0 ); // don't move left past zero (min)
                            moveFramesX -= Math.Max( MaxVisible + moveFramesX - Max, 0 ); // don't move right past max
                            MinVisible += moveFramesX;
                            MaxVisible += moveFramesX;
                        }
                        else if( Footer_Dragging == DragState.DraggingLeft ) { // MOVE LEFT HANDLE
                            MinVisible += moveFramesX;
                            if( MinVisible < Min ) {
                                MinVisible = Min;
                            }
                            if( MinVisible > MaxVisible - 1 ) {
                                MinVisible = MaxVisible - 1;
                            }
                        }
                        else if( Footer_Dragging == DragState.DraggingRight ) { // MOVE RIGHT HANDLE
                            MaxVisible += moveFramesX;
                            if( MaxVisible > Max ) {
                                MaxVisible = Max;
                            }
                            if( MaxVisible < MinVisible + 1 ) {
                                MaxVisible = MinVisible + 1;
                            }
                        }
                    }
                    Footer_LastDrag = dragPos;
                }
                else {
                    Footer_Dragging = DragState.None;
                }
            }
            else {
                Footer_Dragging = DragState.None;
            }

            ImGui.EndChild();
            ImGui.EndGroup();
            ImGui.SetCursorPos( cursor );

            // ==== DRAW THE ITEM ======
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            if( Selected != null ) {
                Selected.DrawBody( parentId );
            }
            else {
                ImGui.Text( "Select an item..." );
            }
        }

        public static bool Contained( Vector2 pos, Vector2 p1, Vector2 size ) {
            if( pos.X < p1.X || pos.X > ( p1.X + size.X ) || pos.Y < p1.Y || pos.Y > ( p1.Y + size.Y ) ) return false;
            return true;
        }
    }
}
