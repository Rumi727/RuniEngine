#nullable enable
using System;
using UnityEditor;
using UnityEngine;

namespace RuniEngine.Editor
{
    //https://github.com/miguel12345/EditorGUISplitView/blob/master/Assets/EditorGUISplitView/Scripts/Editor/EditorGUISplitView.cs
    public class EditorGUISplitView
    {
        public enum Direction
        {
            Horizontal,
            Vertical
        }



        public Direction splitDirection { get; }
        public float splitNormalizedPosition { get; set; }
        
        public Vector2 formerScrollPosition { get; set; }
        public Vector2 latterScrollPosition { get; set; }
        public Rect availableRect { get; private set; }



        public event Action? repaintAction;



        public EditorGUISplitView(Direction splitDirection, float initialValue = 0.3f, Action? repaintAction = null)
        {
            splitNormalizedPosition = initialValue;

            this.splitDirection = splitDirection;
            this.repaintAction = repaintAction;
        }

        public void BeginSplitView()
        {
            EditorGUILayout.BeginScrollView(Vector2.zero);

            Rect rect;
            if (splitDirection == Direction.Horizontal)
                rect = EditorGUILayout.BeginHorizontal();
            else
                rect = EditorGUILayout.BeginVertical();

            if (rect.width > 0.0f)
                availableRect = rect;

            if (splitDirection == Direction.Horizontal)
                formerScrollPosition = EditorGUILayout.BeginScrollView(formerScrollPosition, GUILayout.Width(availableRect.width * splitNormalizedPosition - 4f));
            else
                formerScrollPosition = EditorGUILayout.BeginScrollView(formerScrollPosition, GUILayout.Height(availableRect.height * splitNormalizedPosition - 2f));
        }

        public void Split()
        {
            EditorGUILayout.EndScrollView();

            GUILayout.Space(4);

            ResizeSplitFirstView();

            latterScrollPosition = EditorGUILayout.BeginScrollView(latterScrollPosition);
        }

        public void EndSplitView()
        {
            EditorGUILayout.EndScrollView();

            if (splitDirection == Direction.Horizontal)
                EditorGUILayout.EndHorizontal();
            else
                EditorGUILayout.EndVertical();

            EditorGUILayout.EndScrollView();
        }

        bool resize;
        void ResizeSplitFirstView()
        {
            if (Event.current.type == EventType.Layout)
                return;

            Rect resizeHandleDrawRect;
            Rect resizeHandleRect;

            if (splitDirection == Direction.Horizontal)
            {
                resizeHandleRect = new Rect(availableRect.width * splitNormalizedPosition - 4f, availableRect.y, 8f, availableRect.height);
                resizeHandleDrawRect = new Rect(availableRect.width * splitNormalizedPosition - 0.5f, availableRect.y, 1f, availableRect.height);
            }
            else
            {
                resizeHandleRect = new Rect(availableRect.x, availableRect.height * splitNormalizedPosition - 4f, availableRect.width, 8f);
                resizeHandleDrawRect = new Rect(availableRect.x, availableRect.height * splitNormalizedPosition - 0.5f, availableRect.width, 1f);
            }

            EditorGUI.DrawRect(resizeHandleDrawRect, new Color(0.4980392f, 0.4980392f, 0.4980392f));

            if (splitDirection == Direction.Horizontal)
                EditorGUIUtility.AddCursorRect(resizeHandleRect, MouseCursor.ResizeHorizontal);
            else
                EditorGUIUtility.AddCursorRect(resizeHandleRect, MouseCursor.ResizeVertical);

            if (Event.current.type == EventType.MouseDown && resizeHandleRect.Contains(Event.current.mousePosition))
                resize = true;
            if (resize)
            {
                if (splitDirection == Direction.Horizontal)
                    splitNormalizedPosition = (Event.current.mousePosition.x.Clamp(0) / availableRect.width).Clamp(0.1f, 0.9f);
                else
                    splitNormalizedPosition = (Event.current.mousePosition.y.Clamp(0) / availableRect.height).Clamp(0.1f, 0.9f);

                repaintAction?.Invoke();
            }
            
            if (Event.current.type == EventType.MouseUp)
                resize = false;
        }
    }

}