using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

namespace NKKD.EDIT
{
    public class TackPointView
    {
        public TackPointModel model;
        public TackPointViewModel viewModel;

        public void SetModels(TackPointModel model, TackPointViewModel viewModel)
        {
            this.model = model;
            this.viewModel = viewModel;
        }

        public void InitializeTackTexture(Texture2D baseTex)
        {
            GenerateTextureFromBaseTexture(baseTex, model.Index);

            viewModel.labelStyle_ = new GUIStyle();
            viewModel.labelStyle_.normal.textColor = Color.white;
            viewModel.labelStyle_.fontSize = 10;
            viewModel.labelStyle_.wordWrap = true;
        }
        public Texture2D GetColorTex()
        {
            return viewModel.tackColorTex_;
        }

        public void GenerateTextureFromBaseTexture(Texture2D baseTex, int index)
        {
            var samplingColor = baseTex.GetPixels()[0];
            var rgbVector = new Vector3(samplingColor.r, samplingColor.g, samplingColor.b);
            var rotatedVector = Quaternion.AngleAxis(12.5f * index, new Vector3(1.5f * index, 1.25f * index, 1.37f * index)) * rgbVector;
            var slidedColor = new Color(rotatedVector.x, rotatedVector.y, rotatedVector.z, 1);

            viewModel.tackBackTransparentTex_ = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            viewModel.tackBackTransparentTex_.SetPixel(0, 0, new Color(slidedColor.r, slidedColor.g, slidedColor.b, 0.5f));
            viewModel.tackBackTransparentTex_.Apply();

            viewModel.tackColorTex_ = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            viewModel.tackColorTex_.SetPixel(0, 0, new Color(slidedColor.r, slidedColor.g, slidedColor.b, 1.0f));
            viewModel.tackColorTex_.Apply();
        }

        public Rect DrawTackPointInRect(float startX, float startY)
        {
            var tackStartPointX = startX + (model.Start * WindowSettings.TACK_FRAME_WIDTH);
            var end = model.Start + model.Span - 1;
            var tackEndPointX = startX + (end * WindowSettings.TACK_FRAME_WIDTH);
            var tackBGRect = new Rect(tackStartPointX, startY, model.Span * WindowSettings.TACK_FRAME_WIDTH + 1f, WindowSettings.TACK_HEIGHT);

            switch (viewModel.mode_)
            {
                case TackModifyMode.DRAG_BODY:
                    {
                        tackStartPointX = startX + (model.Start * WindowSettings.TACK_FRAME_WIDTH) + viewModel.distance_.x;
                        tackEndPointX = startX + (end * WindowSettings.TACK_FRAME_WIDTH) + viewModel.distance_.x;
                        tackBGRect = new Rect(tackStartPointX,
                            startY,
                            model.Span * WindowSettings.TACK_FRAME_WIDTH + 1f,
                            WindowSettings.TACK_HEIGHT);
                        break;
                    }
                case TackModifyMode.DRAG_END:
                    {
                        tackEndPointX = startX + (end * WindowSettings.TACK_FRAME_WIDTH) + viewModel.distance_.x;
                        tackBGRect = new Rect(tackStartPointX,
                            startY,
                            model.Span * WindowSettings.TACK_FRAME_WIDTH + viewModel.distance_.x + 1f,
                            WindowSettings.TACK_HEIGHT);
                        break;
                    }
            }

            // draw tack.
            {
                // draw bg.
                var frameBGRect = new Rect(tackBGRect.x, tackBGRect.y, tackBGRect.width, WindowSettings.TACK_FRAME_HEIGHT);

                GUI.DrawTexture(frameBGRect, viewModel.tackBackTransparentTex_);

                // draw points.
                {
                    // tackpoint back line.
                    if (model.Span == 1)
                        GUI.DrawTexture(new Rect(tackBGRect.x + (WindowSettings.TACK_FRAME_WIDTH / 3) + 1,
                                startY + (WindowSettings.TACK_FRAME_HEIGHT / 3) - 1,
                                (WindowSettings.TACK_FRAME_WIDTH / 3) - 1, 11),
                            viewModel.tackColorTex_);

                    if (1 < model.Span)
                        GUI.DrawTexture(new Rect(tackBGRect.x + (WindowSettings.TACK_FRAME_WIDTH / 2),
                                startY + (WindowSettings.TACK_FRAME_HEIGHT / 3) - 1,
                                tackEndPointX - tackBGRect.x, 11),
                            viewModel.tackColorTex_);

                    // frame start point.
                    DrawTackPoint(model.Start, tackBGRect.x, startY);

                    // frame end point.
                    if (1 < model.Span)DrawTackPoint(end, tackEndPointX, startY);
                }

                var routineComponentY = startY + WindowSettings.TACK_FRAME_HEIGHT;

                // routine component.
                {
                    var height = WindowSettings.ROUTINE_HEIGHT_DEFAULT;
                    if (model.Active)GUI.DrawTexture(new Rect(tackBGRect.x, routineComponentY, tackBGRect.width, height), WindowSettings.activeTackBaseTex);

                    GUI.DrawTexture(new Rect(tackBGRect.x + 1, routineComponentY, tackBGRect.width - 2, height - 1), viewModel.tackColorTex_);

                    string labelText = model.Start.ToString() + " - " + model.Span.ToString(); // +"\n"+ title_;

                    GUI.Label(new Rect(tackBGRect.x + 1, routineComponentY, tackBGRect.width - 2, height - 1), labelText, viewModel.labelStyle_);
                }
            }

            return tackBGRect;
        }

        public void DrawTackPoint(int frame, float pointX, float pointY)
        {
            if (model.Span == 1)
            {
                if (frame % 5 == 0 && 0 < frame)
                {
                    GUI.DrawTexture(new Rect(pointX + 2, pointY + (WindowSettings.TACK_FRAME_HEIGHT / 3) - 2, WindowSettings.TACK_POINT_SIZE, WindowSettings.TACK_POINT_SIZE), WindowSettings.grayPointSingleTex);
                }
                else
                {
                    GUI.DrawTexture(new Rect(pointX + 2, pointY + (WindowSettings.TACK_FRAME_HEIGHT / 3) - 2, WindowSettings.TACK_POINT_SIZE, WindowSettings.TACK_POINT_SIZE), WindowSettings.whitePointSingleTex);
                }
                return;
            }

            if (frame % 5 == 0 && 0 < frame)
            {
                GUI.DrawTexture(new Rect(pointX + 2, pointY + (WindowSettings.TACK_FRAME_HEIGHT / 3) - 2, WindowSettings.TACK_POINT_SIZE, WindowSettings.TACK_POINT_SIZE), WindowSettings.grayPointTex);
            }
            else
            {
                GUI.DrawTexture(new Rect(pointX + 2, pointY + (WindowSettings.TACK_FRAME_HEIGHT / 3) - 2, WindowSettings.TACK_POINT_SIZE, WindowSettings.TACK_POINT_SIZE), WindowSettings.whitePointTex);
            }
        }

        public void DrawTack(Rect limitRect, string parentTimelineId, float startX, float startY, bool isUnderEvent)
        {
            if (!model.IsExistTack)
                return;
            model.ParentTimelineId = parentTimelineId;

            var tackBGRect = DrawTackPointInRect(startX, startY);
            var globalMousePos = Event.current.mousePosition;
            var useEvent = false;
            var localMousePos = new Vector2(globalMousePos.x - tackBGRect.x, globalMousePos.y - tackBGRect.y);
            var sizeRect = new Rect(0, 0, tackBGRect.width, tackBGRect.height);

            if (!isUnderEvent)return;

            // mouse event Armling.
            switch (viewModel.mode_)
            {
                case TackModifyMode.NONE:
                    {
                        useEvent = BeginTackModify(tackBGRect, globalMousePos);
                        break;
                    }

                case TackModifyMode.GRAB_BODY:
                case TackModifyMode.GRAB_END:
                    {
                        useEvent = RecognizeTackModify(globalMousePos);
                        break;
                    }
                case TackModifyMode.DRAG_BODY:
                case TackModifyMode.DRAG_END:
                    {
                        useEvent = UpdateTackModify(limitRect, tackBGRect, globalMousePos);
                        break;
                    }
            }

            if (sizeRect.Contains(localMousePos))
            {
                switch (Event.current.type)
                {
                    case EventType.ContextClick:
                        {
                            ShowContextMenu();
                            useEvent = true;
                            break;
                        }
                    case EventType.MouseUp:
                        {
                            // right click.
                            if (Event.current.button == 1)
                            {
                                ShowContextMenu();
                                useEvent = true;
                                break;
                            }

                            TackPoint.Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_OBJECT_SELECTED, model.TackId));
                            useEvent = true;
                            break;
                        }
                }
            }

            if (useEvent)
            {
                Event.current.Use();
            }
        }

        bool RecognizeTackModify(Vector2 mousePos)
        {

            switch (Event.current.type)
            {
                case EventType.MouseDrag:
                    {
                        switch (viewModel.mode_)
                        {
                            case TackModifyMode.GRAB_BODY:
                                {
                                    viewModel.mode_ = TackModifyMode.DRAG_BODY;
                                    TackPoint.Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_OBJECT_SELECTED, model.TackId));
                                    return true;
                                }
                            case TackModifyMode.GRAB_END:
                                {
                                    viewModel.mode_ = TackModifyMode.DRAG_END;
                                    TackPoint.Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_OBJECT_SELECTED, model.TackId));
                                    return true;
                                }
                        }

                        return false;
                    }
                case EventType.MouseUp:
                    {
                        viewModel.mode_ = TackModifyMode.NONE;
                        TackPoint.Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_OBJECT_SELECTED, model.TackId));
                        return true;
                    }
            }

            return false;
        }

        bool UpdateTackModify(Rect limitRect, Rect tackBGRect, Vector2 draggingPoint)
        {
            if (!limitRect.Contains(draggingPoint))
            {
                ExitUpdate(viewModel.distance_);
                return true;
            }

            // far from bandwidth, exit mode.
            if (draggingPoint.y < 0
                || tackBGRect.height + WindowSettings.TIMELINE_HEADER_HEIGHT < draggingPoint.y)
            {
                ExitUpdate(viewModel.distance_);
                return true;
            }

            switch (Event.current.type)
            {
                case EventType.MouseDrag:
                    {
                        TackPoint.Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TACK_MOVING, model.TackId));

                        viewModel.distance_ = draggingPoint - viewModel.dragBeginPoint_;
                        var distanceToFrame = DistanceToFrame(viewModel.distance_.x);

                        switch (viewModel.mode_)
                        {
                            case TackModifyMode.DRAG_BODY:
                                {
                                    // limit 0 <= start
                                    if ((model.Start + distanceToFrame) < 0)viewModel.distance_.x = -FrameToDistance(model.Start);
                                    break;
                                }
                            case TackModifyMode.DRAG_END:
                                {
                                    // limit start <= end
                                    if ((model.Span + distanceToFrame) <= 1)viewModel.distance_.x = -FrameToDistance(model.Span - 1);
                                    break;
                                }
                        }

                        return true;
                    }
                case EventType.MouseUp:
                    {
                        ExitUpdate(viewModel.distance_);
                        return true;
                    }
            }

            return false;
        }

        void ShowContextMenu()
        {
            var framePoint = model.Start;
            var menu = new GenericMenu();

            var menuItems = new Dictionary<string, OnTrackEvent.EventType>
                { { "Delete This Tack", OnTrackEvent.EventType.EVENT_TACK_DELETED },
                    { "Copy Tack", OnTrackEvent.EventType.EVENT_TACK_COPY },
                };

            foreach (var key in menuItems.Keys)
            {
                var eventType = menuItems[key];
                menu.AddItem(
                    new GUIContent(key),
                    false,
                    () =>
                    {
                        TackPoint.Emit(new OnTrackEvent(eventType, model.TackId, framePoint));
                    }
                );
            }
            menu.ShowAsContext();
        }

        bool BeginTackModify(Rect tackBGRect, Vector2 beginPoint)
        {
            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    {
                        var endRect = new Rect(tackBGRect.x + tackBGRect.width - WindowSettings.TACK_FRAME_WIDTH, tackBGRect.y, WindowSettings.TACK_FRAME_WIDTH, WindowSettings.TACK_FRAME_HEIGHT);
                        if (endRect.Contains(beginPoint))
                        {
                            viewModel.dragBeginPoint_ = beginPoint;
                            viewModel.mode_ = TackModifyMode.GRAB_END;
                            return true;
                        }
                        if (tackBGRect.Contains(beginPoint))
                        {
                            viewModel.dragBeginPoint_ = beginPoint;
                            viewModel.mode_ = TackModifyMode.GRAB_BODY;
                            return true;
                        }
                        return false;
                    }
            }

            return false;
        }

        float FrameToDistance(int frame)
        {
            return WindowSettings.TACK_FRAME_WIDTH * frame;
        }

        int DistanceToFrame(float distX)
        {
            var distanceToFrame = (int)(distX / WindowSettings.TACK_FRAME_WIDTH);
            var distanceDelta = distX % WindowSettings.TACK_FRAME_WIDTH;
            if (WindowSettings.BEHAVE_FRAME_MOVE_RATIO <= distanceDelta)distanceToFrame = distanceToFrame + 1;
            if (distanceDelta <= -WindowSettings.BEHAVE_FRAME_MOVE_RATIO)distanceToFrame = distanceToFrame - 1;
            return distanceToFrame;
        }

        void ExitUpdate(Vector2 currentDistance)
        {
            var distanceToFrame = DistanceToFrame(currentDistance.x);
            TackPoint.Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TACK_MOVED, model.TackId));
            var newStart = model.Start;
            var newSpan = model.Span;
            var lastStart = model.Start;
            var lastSpan = model.Span;

            switch (viewModel.mode_)
            {
                case TackModifyMode.DRAG_BODY:
                    {
                        newStart = model.Start + distanceToFrame;
                        break;
                    }
                case TackModifyMode.DRAG_END:
                    {
                        newSpan = model.Span + distanceToFrame;
                        break;
                    }
            }

            if (newStart < 0)newStart = 0;
            string id = MethodBase.GetCurrentMethod().Name;
            Action action = () =>
            {
                model.Start = newStart;
                model.Span = newSpan;
                viewModel.lastStart_ = lastStart;
                viewModel.lastSpan_ = lastSpan;
            };

            Action undo = () =>
            {
                model.Start = lastStart;
                model.Span = lastSpan;
                viewModel.lastStart_ = lastStart;
                viewModel.lastSpan_ = lastSpan;
            };

            ARIMotionMainWindow.scoreCmd_.Do(new MotionCommand(id, action, undo));
            TackPoint.Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TACK_MOVED_AFTER, model.TackId));

            viewModel.mode_ = TackModifyMode.NONE;
            viewModel.distance_ = Vector2.zero;
        }
    }
}