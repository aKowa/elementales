// Copyright (C) Lumen Section - All Rights Reserved
// Written by Nicolas Baillard <nicolas.baillard@gmail.com>
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using static UnityEngine.Mathf;
using static LumenSection.LevelLinker.MathUtils;



namespace LumenSection.LevelLinker
{
public class EditorLevelConnectionMap : EditorWindow
{
  // Types
  private class DragOperation
  {
    public Action UpdateAction;
  }



  private delegate void DelayedAction();



  // Internals
  private LevelConnectionMap mMap;
  private bool               mMustRecomputeViewSize;

  // Widgets
  private DropdownField mMapSelectionDropDown;
  private TextElement   mNoMapText;
  private ScrollView    mScrollView;
  private Texture2D     mBackgroundTexture;

  // Drag & Drop
  private DragOperation mDragOperation;

  // Hovered Element
  private VisualElement mHoveredElement;
  private VisualElement mSelectedLevel;

  // Connections
  private VisualElement mHighlightedConnection;
  private VisualElement mSelectedConnection;
  private Vector2?      mConnectionDragStartPosition;
  private Vector2[]     mConnetionDragInitialPoints;
  private int           mConnectionDraggedPointIndex;



  #region Init
  private void Reset()
  {
    mDragOperation               = null;
    mHighlightedConnection       = null;
    mSelectedConnection          = null;
    mConnectionDragStartPosition = null;
    mHoveredElement              = null;
    mSelectedLevel               = null;
    mConnectionDraggedPointIndex = -1;
    mConnetionDragInitialPoints  = null;
  }
  #endregion



  #region Utils
  private static Vector2 SnapPosition(Vector2 position)
  {
    return RoundPosition(position, new Vector2(10f, 10f), Vector2.zero, Vector2.zero, Vector2.zero);
  }

  private void SelectMap(LevelConnectionMap map)
  {
    if (mMap != null)
      mMap.OnChanged -= Refresh;

    mMap = map;

    if (mMap != null)
      mMap.OnChanged += Refresh;
  }

  private static void HoverElement(VisualElement element)
  {
    element.AddToClassList("hovered");
  }

  private static void UnhoverElement(VisualElement element)
  {
    element.RemoveFromClassList("hovered");
  }
  #endregion



  #region Menu
  [MenuItem("Window/Level Connection Map")]
  public static void ShowFromMenu()
  {
    var window = GetWindow<EditorLevelConnectionMap>(title: "Level Connection Map", focus: true);
    window.Show();
  }

  [MenuItem("Window/Level Connection Map", true)]
  public static bool CanShowFromMenu()
  {
    return !EditorApplication.isPlayingOrWillChangePlaymode;
  }
  #endregion



  #region Background
  private void DrawBackground(MeshGenerationContext context)
  {
    var  element = context.visualElement;
    Rect r       = element.localBound;
    
    if (r.width < 0.01f || r.height < 0.01f)
      return;

    const float left   = 0;
    float       right  = r.width;
    const float top    = 0;
    float       bottom = r.height;

    Vertex[] kVertices = new Vertex[4];
    ushort[] kIndices  = {0, 1, 2, 2, 3, 0};

    kVertices[0].position = new Vector3(left,  bottom, Vertex.nearZ);
    kVertices[1].position = new Vector3(left,  top,    Vertex.nearZ);
    kVertices[2].position = new Vector3(right, top,    Vertex.nearZ);
    kVertices[3].position = new Vector3(right, bottom, Vertex.nearZ);

    kVertices[0].tint = Color.white;
    kVertices[1].tint = Color.white;
    kVertices[2].tint = Color.white;
    kVertices[3].tint = Color.white;

    MeshWriteData mwd = context.Allocate(kVertices.Length, kIndices.Length, mBackgroundTexture);

    Rect  uvRegion = mwd.uvRegion;
    float xuv      = right / 100f;
    float yuv      = bottom / 100f;
    kVertices[0].uv = new Vector2(0,   yuv) * uvRegion.size + uvRegion.min;
    kVertices[1].uv = new Vector2(0,   0) * uvRegion.size + uvRegion.min;
    kVertices[2].uv = new Vector2(xuv, 0) * uvRegion.size + uvRegion.min;
    kVertices[3].uv = new Vector2(xuv, yuv) * uvRegion.size + uvRegion.min;

    mwd.SetAllVertices(kVertices);
    mwd.SetAllIndices(kIndices);
  }
  #endregion



  #region Draw Utils
  public static void DrawCable(Vector2[] points, float thickness, Color color, MeshGenerationContext context)
  {
    List<Vertex> vertices = new List<Vertex>();
    List<ushort> indices  = new List<ushort>();

    const float aaThickNess = 1f;
    var         transparent = new Color(color.r, color.g, color.b, 0f);

    float previousOffsetX         = 0f;
    float previousOffsetY         = 0f;
    float previousExternalOffsetX = 0;
    float previousExternalOffsetY = 0;

    for (int i = 0; i < points.Length - 1; i++)
    {
      var pointA = points[i];
      var pointB = points[i + 1];

      float angle           = Atan2(pointB.y - pointA.y, pointB.x - pointA.x);
      float offsetX         = thickness / 2 * Sin(angle);
      float offsetY         = thickness / 2 * Cos(angle);
      float externalOffsetX = (thickness + aaThickNess) / 2 * Sin(angle);
      float externalOffsetY = (thickness + aaThickNess) / 2 * Cos(angle);

      if (i > 0)
      {
        var a = Vector2.SignedAngle((pointB - pointA), (points[i - 1] - pointA));
        var s = Sign(a);

        if (s < 0)
        {
          vertices.Add(new Vertex // 0
          {
            position = new Vector3(pointA.x, pointA.y, Vertex.nearZ), tint = color
          });
          vertices.Add(new Vertex // 1
          {
            position = new Vector3(pointA.x - offsetX, pointA.y + offsetY, Vertex.nearZ), tint = color
          });
          vertices.Add(new Vertex // 2
          {
            position = new Vector3(pointA.x - previousOffsetX, pointA.y + previousOffsetY, Vertex.nearZ), tint = color
          });

          vertices.Add(new Vertex // 3
          {
            position = new Vector3(pointA.x - offsetX, pointA.y + offsetY, Vertex.nearZ), tint = color
          });
          vertices.Add(new Vertex // 5
          {
            position = new Vector3(pointA.x - previousExternalOffsetX, pointA.y + previousExternalOffsetY, Vertex.nearZ), tint = transparent
          });
          vertices.Add(new Vertex // 4
          {
            position = new Vector3(pointA.x - previousOffsetX, pointA.y + previousOffsetY, Vertex.nearZ), tint = color
          });

          vertices.Add(new Vertex // 3
          {
            position = new Vector3(pointA.x - offsetX, pointA.y + offsetY, Vertex.nearZ), tint = color
          });
          vertices.Add(new Vertex // 5
          {
            position = new Vector3(pointA.x - externalOffsetX, pointA.y + externalOffsetY, Vertex.nearZ), tint = transparent
          });
          vertices.Add(new Vertex // 5
          {
            position = new Vector3(pointA.x - previousExternalOffsetX, pointA.y + previousExternalOffsetY, Vertex.nearZ), tint = transparent
          });

          ushort nbIndice = (ushort)indices.Count;

          indices.Add((ushort)(0 + nbIndice));
          indices.Add((ushort)(1 + nbIndice));
          indices.Add((ushort)(2 + nbIndice));

          indices.Add((ushort)(3 + nbIndice));
          indices.Add((ushort)(4 + nbIndice));
          indices.Add((ushort)(5 + nbIndice));

          indices.Add((ushort)(6 + nbIndice));
          indices.Add((ushort)(7 + nbIndice));
          indices.Add((ushort)(8 + nbIndice));
        }

        else
        {
          vertices.Add(new Vertex // 0
          {
            position = new Vector3(pointA.x, pointA.y, Vertex.nearZ), tint = color
          });
          vertices.Add(new Vertex // 1
          {
            position = new Vector3(pointA.x + offsetX, pointA.y - offsetY, Vertex.nearZ), tint = color
          });
          vertices.Add(new Vertex // 2
          {
            position = new Vector3(pointA.x + previousOffsetX, pointA.y - previousOffsetY, Vertex.nearZ), tint = color
          });

          vertices.Add(new Vertex // 3
          {
            position = new Vector3(pointA.x + offsetX, pointA.y - offsetY, Vertex.nearZ), tint = color
          });
          vertices.Add(new Vertex // 5
          {
            position = new Vector3(pointA.x + previousExternalOffsetX, pointA.y - previousExternalOffsetY, Vertex.nearZ), tint = transparent
          });
          vertices.Add(new Vertex // 4
          {
            position = new Vector3(pointA.x + previousOffsetX, pointA.y - previousOffsetY, Vertex.nearZ), tint = color
          });

          vertices.Add(new Vertex // 3
          {
            position = new Vector3(pointA.x + offsetX, pointA.y - offsetY, Vertex.nearZ), tint = color
          });
          vertices.Add(new Vertex // 5
          {
            position = new Vector3(pointA.x + externalOffsetX, pointA.y - externalOffsetY, Vertex.nearZ), tint = transparent
          });
          vertices.Add(new Vertex // 5
          {
            position = new Vector3(pointA.x + previousExternalOffsetX, pointA.y - previousExternalOffsetY, Vertex.nearZ), tint = transparent
          });

          ushort nbIndice = (ushort)indices.Count;

          indices.Add((ushort)(0 + nbIndice));
          indices.Add((ushort)(2 + nbIndice));
          indices.Add((ushort)(1 + nbIndice));

          indices.Add((ushort)(3 + nbIndice));
          indices.Add((ushort)(5 + nbIndice));
          indices.Add((ushort)(4 + nbIndice));

          indices.Add((ushort)(6 + nbIndice));
          indices.Add((ushort)(8 + nbIndice));
          indices.Add((ushort)(7 + nbIndice));
        }
      }

      previousOffsetX         = offsetX;
      previousOffsetY         = offsetY;
      previousExternalOffsetX = externalOffsetX;
      previousExternalOffsetY = externalOffsetY;

      vertices.Add(new Vertex // 0
      {
        position = new Vector3(pointA.x + offsetX, pointA.y - offsetY, Vertex.nearZ), tint = color
      });
      vertices.Add(new Vertex // 1
      {
        position = new Vector3(pointB.x + offsetX, pointB.y - offsetY, Vertex.nearZ), tint = color
      });
      vertices.Add(new Vertex // 2
      {
        position = new Vector3(pointB.x - offsetX, pointB.y + offsetY, Vertex.nearZ), tint = color
      });

      vertices.Add(new Vertex // 3
      {
        position = new Vector3(pointB.x - offsetX, pointB.y + offsetY, Vertex.nearZ), tint = color
      });
      vertices.Add(new Vertex // 4
      {
        position = new Vector3(pointA.x - offsetX, pointA.y + offsetY, Vertex.nearZ), tint = color
      });
      vertices.Add(new Vertex // 5
      {
        position = new Vector3(pointA.x + offsetX, pointA.y - offsetY, Vertex.nearZ), tint = color
      });

      vertices.Add(new Vertex // 6
      {
        position = new Vector3(pointA.x + externalOffsetX, pointA.y - externalOffsetY, Vertex.nearZ), tint = transparent
      });
      vertices.Add(new Vertex // 7
      {
        position = new Vector3(pointB.x + externalOffsetX, pointB.y - externalOffsetY, Vertex.nearZ), tint = transparent
      });
      vertices.Add(new Vertex // 8
      {
        position = new Vector3(pointB.x + offsetX, pointB.y - offsetY, Vertex.nearZ), tint = color
      });

      vertices.Add(new Vertex // 9
      {
        position = new Vector3(pointB.x + offsetX, pointB.y - offsetY, Vertex.nearZ), tint = color
      });
      vertices.Add(new Vertex // 10
      {
        position = new Vector3(pointA.x + offsetX, pointA.y - offsetY, Vertex.nearZ), tint = color
      });
      vertices.Add(new Vertex // 11
      {
        position = new Vector3(pointA.x + externalOffsetX, pointA.y - externalOffsetY, Vertex.nearZ), tint = transparent
      });

      vertices.Add(new Vertex // 12
      {
        position = new Vector3(pointA.x - offsetX, pointA.y + offsetY, Vertex.nearZ), tint = color
      });
      vertices.Add(new Vertex // 13
      {
        position = new Vector3(pointB.x - offsetX, pointB.y + offsetY, Vertex.nearZ), tint = color
      });
      vertices.Add(new Vertex // 14
      {
        position = new Vector3(pointB.x - externalOffsetX, pointB.y + externalOffsetY, Vertex.nearZ), tint = transparent
      });

      vertices.Add(new Vertex // 15
      {
        position = new Vector3(pointB.x - externalOffsetX, pointB.y + externalOffsetY, Vertex.nearZ), tint = transparent
      });
      vertices.Add(new Vertex // 16
      {
        position = new Vector3(pointA.x - externalOffsetX, pointA.y + externalOffsetY, Vertex.nearZ), tint = transparent
      });
      vertices.Add(new Vertex // 17
      {
        position = new Vector3(pointA.x - offsetX, pointA.y + offsetY, Vertex.nearZ), tint = color
      });

      {
        ushort nbIndice = (ushort)indices.Count;

        indices.Add((ushort)(0 + nbIndice));
        indices.Add((ushort)(1 + nbIndice));
        indices.Add((ushort)(2 + nbIndice));

        indices.Add((ushort)(3 + nbIndice));
        indices.Add((ushort)(4 + nbIndice));
        indices.Add((ushort)(5 + nbIndice));

        indices.Add((ushort)(6 + nbIndice));
        indices.Add((ushort)(7 + nbIndice));
        indices.Add((ushort)(8 + nbIndice));

        indices.Add((ushort)(9 + nbIndice));
        indices.Add((ushort)(10 + nbIndice));
        indices.Add((ushort)(11 + nbIndice));

        indices.Add((ushort)(12 + nbIndice));
        indices.Add((ushort)(13 + nbIndice));
        indices.Add((ushort)(14 + nbIndice));

        indices.Add((ushort)(15 + nbIndice));
        indices.Add((ushort)(16 + nbIndice));
        indices.Add((ushort)(17 + nbIndice));
      }
    }

    var mesh = context.Allocate(vertices.Count, indices.Count);
    mesh.SetAllVertices(vertices.ToArray());
    mesh.SetAllIndices(indices.ToArray());
  }
  #endregion



  #region Level UI
  private VisualElement CreateLevelElement(LevelConnectionMap.LevelDeclaration level, Dictionary<string, VisualElement> doorToPlug)
  {
    // Element
    var levelElement = new VisualElement();
    levelElement.AddToClassList("level");
    levelElement.style.position = new StyleEnum<Position>(Position.Absolute);
    levelElement.pickingMode    = PickingMode.Ignore;
    levelElement.userData       = level;

    // Box
    var levelBox = new VisualElement();
    levelBox.name = "Box";
    levelBox.AddToClassList("box");
    levelElement.Add(levelBox);

    // Name
    var levelNameElement = new TextElement();
    levelNameElement.AddToClassList("level-name");
    levelNameElement.text        = mMap.FormatSceneName(level.Name);
    levelNameElement.pickingMode = PickingMode.Ignore;
    levelNameElement.RegisterCallback<GeometryChangedEvent>(SnapElementSize);
    levelElement.Add(levelNameElement);

    // Doors
    foreach (var door in level.Doors)
    {
      // Element
      var doorElement = new VisualElement();
      doorElement.userData = door;
      doorElement.AddToClassList("door");
      doorElement.pickingMode = PickingMode.Ignore;
      doorElement.RegisterCallback<GeometryChangedEvent>(SnapElementSize);
      levelElement.Add(doorElement);

      // Text
      var doorText = new TextElement();
      doorText.text        = mMap.FormatDoorName(door.Name);
      doorText.pickingMode = PickingMode.Ignore;
      doorElement.Add(doorText);

      // Plug
      var doorPlug = MakeDoorPlug(door);
      doorElement.Add(doorPlug);
      doorToPlug[door.Guid] = doorPlug;
    }

    // Position
    levelElement.transform.position = new Vector3(level.PositionOnMap.x, level.PositionOnMap.y, levelElement.transform.position.z);
    
    // Events
    if (!EditorApplication.isPlaying)
    {
      // Drag callback, start dragging when mouse is down
      levelBox.RegisterCallback<MouseDownEvent>(StartLevelDrag);

      // Hover
      levelBox.RegisterCallback<MouseEnterEvent>(OnMouseEnterLevel);
      levelBox.RegisterCallback<MouseLeaveEvent>(OnMouseLeaveLevel);
    }

    // Done
    return levelElement;
  }

  private static void SnapElementSize(GeometryChangedEvent evt)
  {
    var element = (VisualElement)evt.target;
    element.UnregisterCallback<GeometryChangedEvent>(SnapElementSize);
    var size = element.layout.size;
    size.y               = Ceil(size.y / 10f) * 10f;
    element.style.height = size.y;
    element.RegisterCallback<GeometryChangedEvent>(SnapElementSize);
  }

  private void OnMouseEnterLevel(MouseEnterEvent evt)
  {
    var levelBoxElement = (VisualElement)evt.target;
    mHoveredElement = levelBoxElement;

    // Prevent hover if mouse is on a connection
    if (mHighlightedConnection == null)
      HoverElement(mHoveredElement);
  }

  private void OnMouseLeaveLevel(MouseLeaveEvent evt)
  {
    var levelBoxElement = (VisualElement)evt.target;
    if (mHoveredElement == levelBoxElement)
    {
      UnhoverElement(mHoveredElement);
      mHoveredElement = null;
    }
  }

  private void BringLevelToFront(VisualElement levelElement)
  {
    // Bring level element in front of other levels but behind connections
    int lastLevelIndex = 0;
    for (int i = 0; i < mScrollView.childCount; ++i)
    {
      var child = mScrollView.ElementAt(i);
      if (child.userData is LevelConnectionMap.LevelDeclaration)
        lastLevelIndex = i;
    }
    mScrollView.Remove(levelElement);
    mScrollView.Insert(lastLevelIndex, levelElement);
  }

  private void SelectLevel(VisualElement levelElement)
  {
    mSelectedLevel = levelElement;
    mSelectedLevel.AddToClassList("selected");
  }

  private void DeselectSelectedLevel()
  {
    if (mSelectedLevel != null)
    {
      mSelectedLevel.RemoveFromClassList("selected");
      mSelectedLevel = null;
    }
  }

  private void DeleteSelectedLevel()
  {
    if (mSelectedLevel == null)
      return;

    var levelElement = mSelectedLevel;
    var level        = (LevelConnectionMap.LevelDeclaration)levelElement.userData;

    var levelBoxElement = levelElement.Q("Box");
    if (mHoveredElement == levelBoxElement)
      mHoveredElement = null;

    Undo.RecordObject(mMap, "Delete Level");
    mMap.DeleteLevel(level);
  }
  #endregion



  #region Level Drag & Drop
  private class LevelDragOperation : DragOperation
  {
    public Vector2 MousePosition;
    public Vector2 Offset;
    public Vector2 StartPosition;
  }



  private void StartLevelDrag(MouseDownEvent evt)
  {
    // Ignore if it's not left mouse button
    if (evt.button != 0)
      return;

    // Get dragged level element
    VisualElement levelElement = ((VisualElement)evt.target).parent;

    // Select level
    SelectLevel(levelElement);

    // Create drag operation
    var operation = new LevelDragOperation();
    Debug.Assert(mDragOperation == null);
    operation.MousePosition =  evt.mousePosition;
    operation.UpdateAction  += () => DragLevelUpdate(levelElement);
    mDragOperation          =  operation;

    // Record offset between level element corner and mouse position
    var     container       = mScrollView.contentContainer;
    Vector2 elementPosition = levelElement.transform.position;
    operation.Offset        = elementPosition - rootVisualElement.ChangeCoordinatesTo(container, operation.MousePosition);
    operation.StartPosition = elementPosition;

    // Bring level element in front of other levels
    BringLevelToFront(levelElement);

    // Capture mouse so that only this element get mouse events
    levelElement.CaptureMouse();

    // Register drag callback
    levelElement.RegisterCallback<MouseMoveEvent>(DragLevel);
    levelElement.RegisterCallback<MouseUpEvent>(StopLevelDrag);
  }

  private void DragLevel(MouseMoveEvent evt)
  {
    // Get operation
    var operation = (LevelDragOperation)mDragOperation;
    Debug.Assert(operation != null);

    // Set mouse position
    operation.MousePosition = evt.mousePosition;
  }

  private void SetLevelPosition(VisualElement levelElement, Vector2 elementPosition, bool final)
  {
    // Snap position
    Vector2 snappedPosition = SnapPosition(elementPosition);

    // Apply position to element
    levelElement.transform.position = new Vector3(snappedPosition.x, snappedPosition.y, levelElement.transform.position.z);

    // Record level position
    if (final)
    {
      var level = (LevelConnectionMap.LevelDeclaration)levelElement.userData;
      mMap.SetLevelPosition(level.Guid, snappedPosition);
    }

    // Redraw connections
    RedrawConnectionsForLevel(levelElement);
  }

  private void DragLevelUpdate(VisualElement levelElement)
  {
    // Get operation
    var operation = (LevelDragOperation)mDragOperation;
    Debug.Assert(operation != null);

    // Scroll view content
    var container = mScrollView.contentContainer;

    // Get mouse position
    Vector2 mousePosition = rootVisualElement.ChangeCoordinatesTo(container, operation.MousePosition);

    // Get element position
    Vector2 elementPosition = levelElement.transform.position;

    // Get element size
    Vector2 elementSize = levelElement.layout.size;

    // Set element position, 
    elementPosition.x = mousePosition.x + operation.Offset.x;
    elementPosition.y = mousePosition.y + operation.Offset.y;

    // Clamp it to scroll view content with a small max tolerance to allow for scrolling
    const float scrollTolerance = 2f;
    elementPosition.x = Min(Max(elementPosition.x, 0), (container.layout.width - levelElement.layout.width + scrollTolerance));
    elementPosition.y = Min(Max(elementPosition.y, 0), (container.layout.height - levelElement.layout.height + scrollTolerance));

    // Clamp to viewport with a small tolerance to allow for scrolling
    Vector2 viewportMin = mScrollView.contentViewport.ChangeCoordinatesTo(container, Vector2.zero);
    Vector2 viewportMax = mScrollView.contentViewport.ChangeCoordinatesTo(container, mScrollView.contentViewport.layout.size);
    elementPosition.x = Min(Max(elementPosition.x, viewportMin.x - scrollTolerance), viewportMax.x - elementSize.x + scrollTolerance);
    elementPosition.y = Min(Max(elementPosition.y, viewportMin.y - scrollTolerance), viewportMax.y - elementSize.y + scrollTolerance);

    // Get element rect in viewport space
    var elementMin         = elementPosition;
    var elementMax         = elementPosition + levelElement.layout.size;
    var elementViewportMin = container.ChangeCoordinatesTo(mScrollView.contentViewport, elementMin);
    var elementViewportMax = container.ChangeCoordinatesTo(mScrollView.contentViewport, elementMax);

    // Check if me must recompute scroll view content size
    bool mustRecomputeScrollview = false;

    // Scroll Up
    if (elementViewportMin.y < mScrollView.contentViewport.localBound.yMin)
    {
      var scrollOffset = mScrollView.scrollOffset;
      scrollOffset.y           = elementMin.y;
      mScrollView.scrollOffset = scrollOffset;
    }

    // Scroll Down
    else if (elementViewportMax.y > mScrollView.contentViewport.localBound.yMax)
    {
      var scrollOffset = mScrollView.scrollOffset;
      scrollOffset.y           = elementMax.y - mScrollView.contentViewport.layout.height;
      mScrollView.scrollOffset = scrollOffset;
      mustRecomputeScrollview  = true;
    }

    // Scroll Left
    if (elementViewportMin.x < mScrollView.contentViewport.localBound.xMin)
    {
      var scrollOffset = mScrollView.scrollOffset;
      scrollOffset.x           = elementMin.x;
      mScrollView.scrollOffset = scrollOffset;
    }

    // Scroll Right
    else if (elementViewportMax.x > mScrollView.contentViewport.localBound.xMax)
    {
      var scrollOffset = mScrollView.scrollOffset;
      scrollOffset.x           = elementMax.x - mScrollView.contentViewport.layout.width;
      mScrollView.scrollOffset = scrollOffset;
      mustRecomputeScrollview  = true;
    }

    // Recompute if needed
    if (mustRecomputeScrollview)
      RecomputeScrollViewContainerSize(null);

    // Apply level position
    SetLevelPosition(levelElement, elementPosition, false);
  }

  private void StopLevelDrag(MouseUpEvent evt)
  {
    // Get dragged level element and level
    VisualElement levelElement = (VisualElement)evt.target;
    var           level        = (LevelConnectionMap.LevelDeclaration)levelElement.userData;

    // Get operation
    var operation = (LevelDragOperation)mDragOperation;
    Debug.Assert(operation != null);

    // Move back level to initial position for undo
    Vector2 finalPosition = levelElement.transform.position;
    SetLevelPosition(levelElement, operation.StartPosition, false);

    // Record for undo
    Undo.RecordObject(mMap, "Move Level");

    // Make level last in list so that next time we refresh it will be in front of other levels
    mMap.BringLevelToFront(level);

    // Apply final level position
    SetLevelPosition(levelElement, finalPosition, true);

    // Adjust scroll view size
    RecomputeScrollViewContainerSize(null);

    // Unregister callbacks
    levelElement.UnregisterCallback<MouseMoveEvent>(DragLevel);
    levelElement.UnregisterCallback<MouseUpEvent>(StopLevelDrag);

    // Release mouse
    levelElement.ReleaseMouse();

    // Drop operation
    mDragOperation = null;
  }
  #endregion



  #region Plug UI
  private VisualElement MakeDoorPlug(LevelConnectionMap.LevelDoorDeclaration door)
  {
    var doorPlug = new VisualElement();
    doorPlug.name     = "Plug";
    doorPlug.userData = door;
    doorPlug.AddToClassList("plug");
    if (door.LeftSide)
      doorPlug.AddToClassList("left-side");
    
    // Events
    if (!EditorApplication.isPlaying)
    {
      // Mouse down callback, start drag or right click
      doorPlug.RegisterCallback<MouseDownEvent>(DoorPlugMouseDown);

      // Hover callback
      doorPlug.RegisterCallback<MouseEnterEvent>(OnEnterDoorPlug);
      doorPlug.RegisterCallback<MouseLeaveEvent>(OnLeaveDoorPlug);
    }

    // Done
    return doorPlug;
  }

  private void OnEnterDoorPlug(MouseEnterEvent evt)
  {
    var plugElement = (VisualElement)evt.target;
    mHoveredElement = plugElement;
    if (mHighlightedConnection == null)
      HoverElement(mHoveredElement);
  }

  private void OnLeaveDoorPlug(MouseLeaveEvent evt)
  {
    var plugElement = (VisualElement)evt.target;
    if (mHoveredElement == plugElement)
    {
      UnhoverElement(mHoveredElement);
      mHoveredElement = null;
    }
  }

  private void DoorPlugMouseDown(MouseDownEvent evt)
  {
    // If left mouse button then start drag
    if (evt.button == 0)
      StartDoorPlugDrag(evt);

    // If right mouse button then open context menu
    else if (evt.button == 1)
      ShowDoorPlugContextMenu(evt);
  }

  private void ShowDoorPlugContextMenu(MouseDownEvent evt)
  {
    // Prevent view to get event
    evt.StopPropagation();

    // Get element and door
    var plugElement = (VisualElement)evt.target;
    var door        = (LevelConnectionMap.LevelDoorDeclaration)plugElement.userData;

    // Create context menu
    var menu = new GenericMenu();

    // Move Left
    if (door.LeftSide)
      menu.AddDisabledItem(new GUIContent("Move Left"));
    else
      menu.AddItem(new GUIContent("Move Left"), false, plug => MovePlugToLeft((VisualElement)plug), plugElement);

    // Move Right
    if (!door.LeftSide)
      menu.AddDisabledItem(new GUIContent("Move Right"));
    else
      menu.AddItem(new GUIContent("Move Right"), false, plug => MovePlugToRight((VisualElement)plug), plugElement);

    // Get nb door in the level and index of this door
    (int doorIndex, int nbDoor) = mMap.GetDoorIndexInLevel(door);

    // Move Up
    if (doorIndex == 0 || nbDoor == 1)
      menu.AddDisabledItem(new GUIContent("Move Up"));
    else
      menu.AddItem(new GUIContent("Move Up"), false, plug => MovePlugUp((VisualElement)plug), plugElement);

    // Move Down
    if (doorIndex == nbDoor - 1 || nbDoor == 1)
      menu.AddDisabledItem(new GUIContent("Move Down"));
    else
      menu.AddItem(new GUIContent("Move Down"), false, plug => MovePlugDown((VisualElement)plug), plugElement);

    // Get position of menu on top of target element.
    var menuPosition = new Vector2(plugElement.layout.xMin, plugElement.layout.height);
    menuPosition = rootVisualElement.LocalToWorld(menuPosition);
    var menuRect = new Rect(menuPosition, Vector2.zero);

    // Open menu
    menu.DropDown(menuRect);
  }

  private static Vector2 GetPlugExtremity(VisualElement plugElement)
  {
    var door = (LevelConnectionMap.LevelDoorDeclaration)plugElement.userData;
    if (door.LeftSide)
      return new Vector2(plugElement.worldBound.min.x, plugElement.worldBound.center.y);
    return new Vector2(plugElement.worldBound.max.x, plugElement.worldBound.center.y);
  }

  private void MovePlugToRight(VisualElement plugElement)
  {
    // Get plug door
    var door = (LevelConnectionMap.LevelDoorDeclaration)plugElement.userData;

    // Do nothing if already of right side
    if (!door.LeftSide)
      return;

    // Record map for undo
    Undo.RecordObject(mMap, "Move Plug to Right");

    // Make door right side
    door.LeftSide = false;
    plugElement.RemoveFromClassList("left-side");

    // Update connection after plug has moved
    RedrawConnectionForPlug(plugElement);
  }

  private void MovePlugToLeft(VisualElement plugElement)
  {
    // Get plug door
    var door = (LevelConnectionMap.LevelDoorDeclaration)plugElement.userData;

    // Do nothing if already of right side
    if (door.LeftSide)
      return;

    // Record map for undo
    Undo.RecordObject(mMap, "Move Plug to Left");

    // Make door right side
    door.LeftSide = true;
    plugElement.AddToClassList("left-side");

    // Update connection after plug has moved
    RedrawConnectionForPlug(plugElement);
  }

  private void MovePlugUp(VisualElement plugElement)
  {
    // Get plug door
    var door = (LevelConnectionMap.LevelDoorDeclaration)plugElement.userData;

    // Record map for undo
    Undo.RecordObject(mMap, "Move Plug Up");

    // Change door index
    mMap.DecreaseDoorIndex(door);

    // Get door and level element the plug belong to
    var doorElement  = plugElement.parent;
    var levelElement = doorElement.parent;

    // Get door element index
    int doorElementIndex = levelElement.IndexOf(doorElement);
    levelElement.Remove(doorElement);
    levelElement.Insert(doorElementIndex - 1, doorElement);

    // Redraw all connections to this level
    RedrawConnectionsForLevel(levelElement);
  }

  private void MovePlugDown(VisualElement plugElement)
  {
    // Get plug door
    var door = (LevelConnectionMap.LevelDoorDeclaration)plugElement.userData;

    // Record map for undo
    Undo.RecordObject(mMap, "Move Plug Down");

    // Change door index
    mMap.IncreaseDoorIndex(door);

    // Get door and level element the plug belong to
    var doorElement  = plugElement.parent;
    var levelElement = doorElement.parent;

    // Get door element index
    int doorElementIndex = levelElement.IndexOf(doorElement);
    levelElement.Remove(doorElement);
    levelElement.Insert(doorElementIndex + 1, doorElement);

    // Redraw all connections to this level
    RedrawConnectionsForLevel(levelElement);
  }
  #endregion



  #region Plug Drag & Drop
  private class PlugDragOperation : DragOperation
  {
    public Vector2       MousePosition;
    public VisualElement BeginPlug;
    public VisualElement EndPlug;
    public Vector2       BeginPosition;
    public Vector2       EndPosition;
  }



  private void StartDoorPlugDrag(MouseDownEvent evt)
  {
    // Get dragged plug element
    VisualElement plugElement = (VisualElement)evt.target;

    // Prevent drag if plug is already connected
    var door = (LevelConnectionMap.LevelDoorDeclaration)plugElement.userData;
    if (mMap.HasConnectionForDoor(door))
      return;

    // Create a visual element for the connection
    var plugConnectionElement = new VisualElement();
    plugConnectionElement.generateVisualContent = DrawDraggedPlugConnection;
    plugConnectionElement.pickingMode           = PickingMode.Ignore;
    mScrollView.Add(plugConnectionElement);

    // Create operation
    var operation = new PlugDragOperation();
    Debug.Assert(mDragOperation == null);
    operation.UpdateAction += () => DragDoorPlugUpdate(plugConnectionElement);
    mDragOperation         =  operation;

    // Keep reference to dragged plug
    operation.BeginPlug = plugElement;

    // Keep mouse position
    operation.MousePosition = evt.mousePosition;

    // Plug style
    plugElement.AddToClassList("dragged");

    // Original position is center of the plug element
    operation.BeginPosition = mScrollView.contentContainer.WorldToLocal(GetPlugExtremity(plugElement));
    operation.EndPosition   = operation.BeginPosition;

    // Connection capture mouse so only it will receive mouse events
    plugConnectionElement.CaptureMouse();

    // Register connection callbacks
    plugConnectionElement.RegisterCallback<MouseMoveEvent>(DragDoorPlug);
    plugConnectionElement.RegisterCallback<MouseUpEvent>(StopDoorPlugDrag);
  }

  private void DrawDraggedPlugConnection(MeshGenerationContext context)
  {
    // Get operation
    var operation = (PlugDragOperation)mDragOperation;
    Debug.Assert(operation != null);

    // Draw
    DrawCable(new[] {operation.BeginPosition, operation.EndPosition},
              1f,
              Color.red,
              context);
  }

  private void DragDoorPlug(MouseMoveEvent evt)
  {
    // Get operation
    var operation = (PlugDragOperation)mDragOperation;
    Debug.Assert(operation != null);

    // Set mouse position
    operation.MousePosition = evt.mousePosition;
  }

  private void DragDoorPlugUpdate(VisualElement plugConnectionElement)
  {
    // Get operation
    var operation = (PlugDragOperation)mDragOperation;
    Debug.Assert(operation != null);

    // Get scroll view container
    var container = mScrollView.contentContainer;

    // Get mouse position
    Vector2 mousePosition = rootVisualElement.ChangeCoordinatesTo(container, rootVisualElement.WorldToLocal(operation.MousePosition));

    // Adjust end position
    operation.EndPosition = mousePosition;

    // Clamp it to scroll view content
    operation.EndPosition.x = Min(Max(operation.EndPosition.x, 0), (container.layout.width));
    operation.EndPosition.y = Min(Max(operation.EndPosition.y, 0), (container.layout.height));

    // Clamp to viewport with a small tolerance to allow for scrolling
    const float scrollTolerance = 2f;
    Vector2     viewportMin     = mScrollView.contentViewport.ChangeCoordinatesTo(container, Vector2.zero);
    Vector2     viewportMax     = mScrollView.contentViewport.ChangeCoordinatesTo(container, mScrollView.contentViewport.layout.size);
    operation.EndPosition.x = Min(Max(operation.EndPosition.x, viewportMin.x - scrollTolerance), viewportMax.x + scrollTolerance);
    operation.EndPosition.y = Min(Max(operation.EndPosition.y, viewportMin.y - scrollTolerance), viewportMax.y + scrollTolerance);

    // Redraw connection
    plugConnectionElement.MarkDirtyRepaint();

    // Convert end position to viewport space
    var viewportEndPosition = container.ChangeCoordinatesTo(mScrollView.contentViewport, operation.EndPosition);

    // Scroll Up
    if (viewportEndPosition.y < mScrollView.contentViewport.localBound.yMin)
    {
      var scrollOffset = mScrollView.scrollOffset;
      scrollOffset.y           = operation.EndPosition.y;
      mScrollView.scrollOffset = scrollOffset;
    }

    // Scroll Down
    else if (viewportEndPosition.y > mScrollView.contentViewport.localBound.yMax)
    {
      var scrollOffset = mScrollView.scrollOffset;
      scrollOffset.y           = operation.EndPosition.y - mScrollView.contentViewport.layout.height;
      mScrollView.scrollOffset = scrollOffset;
    }

    // Scroll Left
    if (viewportEndPosition.x < mScrollView.contentViewport.localBound.xMin)
    {
      var scrollOffset = mScrollView.scrollOffset;
      scrollOffset.x           = operation.EndPosition.x;
      mScrollView.scrollOffset = scrollOffset;
    }

    // Scroll Right
    else if (viewportEndPosition.x > mScrollView.contentViewport.localBound.xMax)
    {
      var scrollOffset = mScrollView.scrollOffset;
      scrollOffset.x           = operation.EndPosition.x - mScrollView.contentViewport.layout.width;
      mScrollView.scrollOffset = scrollOffset;
    }

    // Get element under mouse
    var plugUnderMouse = rootVisualElement.panel.Pick(operation.MousePosition);

    // Ignore if element under mouse is not a plug or if it is the begin plug or if we can't connect to that plug
    if (plugUnderMouse is not {userData: LevelConnectionMap.LevelDoorDeclaration doorUnderMouse} ||
        plugUnderMouse == operation.BeginPlug ||
        mMap.HasConnectionForDoor(doorUnderMouse) ||
        mMap.BelongToSameLevel((LevelConnectionMap.LevelDoorDeclaration)operation.BeginPlug.userData, doorUnderMouse))
    {
      plugUnderMouse = null;
    }

    // If plug under mouse is not the current end plug
    if (plugUnderMouse != operation.EndPlug)
    {
      // Un style current end plug
      if (operation.EndPlug != null)
        operation.EndPlug.RemoveFromClassList("dragged");

      // Style plug under mouse
      if (plugUnderMouse != null)
        plugUnderMouse.AddToClassList("dragged");

      // Keep plug under mouse as current end plug (or null)
      operation.EndPlug = plugUnderMouse;
    }
  }

  private void StopDoorPlugDrag(MouseUpEvent evt)
  {
    // Get dragged connection
    VisualElement plugConnectionElement = (VisualElement)evt.target;

    // Get operation
    var operation = (PlugDragOperation)mDragOperation;
    Debug.Assert(operation != null);

    // Unregister callbacks
    plugConnectionElement.UnregisterCallback<MouseMoveEvent>(DragDoorPlug);
    plugConnectionElement.UnregisterCallback<MouseUpEvent>(StopDoorPlugDrag);

    // Release mouse
    plugConnectionElement.ReleaseMouse();

    // Drop connection element
    mScrollView.Remove(plugConnectionElement);

    // Un style dragged plug
    operation.BeginPlug.RemoveFromClassList("dragged");

    // If mouse is on a plug
    if (operation.EndPlug != null)
    {
      // Un style end plug
      operation.EndPlug.RemoveFromClassList("dragged");

      // Create connection
      var beginDoor = (LevelConnectionMap.LevelDoorDeclaration)operation.BeginPlug.userData;
      var endDoor   = (LevelConnectionMap.LevelDoorDeclaration)operation.EndPlug.userData;
      Undo.RecordObject(mMap, "Create Level Connection");
      var connection        = mMap.CreateConnection(beginDoor, endDoor);
      var connectionElement = CreateConnectionElement(connection, operation.BeginPlug, operation.EndPlug);
      mScrollView.Add(connectionElement);
    }

    // Drop operation
    mDragOperation = null;
  }
  #endregion



  #region Connection UI
  private class ConnectionUserData
  {
    public LevelConnectionMap.LevelConnection Connection;
    public VisualElement                      Plug1;
    public VisualElement                      Plug2;
  }



  private List<Vector2> GetConnectionPoints(ConnectionUserData connection)
  {
    var container = mScrollView.contentContainer;
    var points    = new List<Vector2>(connection.Connection.Points.Count + 2);
    points.Add(container.WorldToLocal(GetPlugExtremity(connection.Plug1)));
    foreach (var point in connection.Connection.Points)
      points.Add(point);
    points.Add(container.WorldToLocal(GetPlugExtremity(connection.Plug2)));
    return points;
  }

  private void DrawConnection(MeshGenerationContext context)
  {
    var connectionElement = context.visualElement;
    var connection        = (ConnectionUserData)connectionElement.userData;
    var points            = GetConnectionPoints(connection);
    DrawCable(points.ToArray(),
              LevelConnectionMap.ConnectionThickness,
              connectionElement.style.backgroundColor.value,
              context);
  }

  private VisualElement CreateConnectionElement(LevelConnectionMap.LevelConnection connection, VisualElement plug1, VisualElement plug2)
  {
    var connectionElement = new VisualElement();
    var userData          = new ConnectionUserData();
    userData.Connection                     = connection;
    userData.Plug1                          = plug1;
    userData.Plug2                          = plug2;
    connectionElement.style.backgroundColor = LevelConnectionMap.ConnectionColor;
    connectionElement.userData              = userData;
    connectionElement.generateVisualContent = DrawConnection;
    return connectionElement;
  }

  private void RedrawConnectionsForLevel(VisualElement levelElement)
  {
    foreach (var connectionElement in mScrollView.Children())
    {
      if (connectionElement.userData is ConnectionUserData connection && (connection.Plug1.parent.parent == levelElement || connection.Plug2.parent.parent == levelElement))
        connectionElement.MarkDirtyRepaint();
    }
  }

  private void RedrawConnectionForPlug(VisualElement plugElement)
  {
    foreach (var connectionElement in mScrollView.Children())
    {
      if (connectionElement.userData is ConnectionUserData connection && (connection.Plug1 == plugElement || connection.Plug2 == plugElement))
        connectionElement.MarkDirtyRepaint();
    }
  }
  #endregion



  #region Connection Edition
  private Color SelectConnectionColor(VisualElement connectionElement)
  {
    if (connectionElement == mSelectedConnection && connectionElement == mHighlightedConnection)
      return LevelConnectionMap.SelectedHighlightedConnectionColor;
    if (connectionElement == mHighlightedConnection)
      return LevelConnectionMap.HighlightedConnectionColor;
    if (connectionElement == mSelectedConnection)
      return LevelConnectionMap.SelectedConnectionColor;
    return LevelConnectionMap.ConnectionColor;
  }

  private void SetConnectionColor(VisualElement connectionElement)
  {
    if (connectionElement == null)
      return;
    connectionElement.style.backgroundColor = SelectConnectionColor(connectionElement);
    connectionElement.MarkDirtyRepaint();
  }

  private VisualElement GetConnectionUnderMouse(Vector2 mousePosition)
  {
    foreach (var element in mScrollView.Children())
    {
      var connection = element.userData as ConnectionUserData;
      if (connection == null)
        continue;

      var points = GetConnectionPoints(connection);
      
      for (int i = 1; i < points.Count; ++i)
      {
        Vector2 point         = points[i];
        Vector2 previousPoint = points[i - 1];
        if (IsOnSegment2D(point, previousPoint, mousePosition, LevelConnectionMap.ConnectionThickness, out _, out _))
          return element;
      }
    }
    return null;
  }

  private void HighlightConnectionUnderMouse(MouseMoveEvent evt)
  {
    // Ignore if any button pressed
    if (evt.pressedButtons != 0)
      return;

    // Get mouse position
    Vector2 mousePosition = rootVisualElement.ChangeCoordinatesTo(mScrollView.contentContainer, rootVisualElement.WorldToLocal(evt.mousePosition));

    // Get connection under mouse
    var connection = GetConnectionUnderMouse(mousePosition);

    // If clicked on a connection then don't propagate event
    if (connection != null)
    {
      if (mHoveredElement != null)
      {
        UnhoverElement(mHoveredElement);
      }
    }
    else if (mHighlightedConnection != null)
    {
      if (mHoveredElement != null)
      {
        HoverElement(mHoveredElement);
      }
    }

    // Highlight connection
    if (connection != mHighlightedConnection)
    {
      var previous = mHighlightedConnection;
      mHighlightedConnection = connection;
      SetConnectionColor(previous);
      SetConnectionColor(mHighlightedConnection);
    }
  }

  private void SelectConnectionUnderMouse(MouseDownEvent evt)
  {
    // Ignore if it's not mouse left button
    if (evt.button != 0)
      return;

    // If a level is selected then unselect it (it will be re selected if we click on it)
    DeselectSelectedLevel();

    // Get mouse position
    Vector2 mousePosition = rootVisualElement.ChangeCoordinatesTo(mScrollView.contentContainer, rootVisualElement.WorldToLocal(evt.mousePosition));

    // Get connection under mouse
    var connection = GetConnectionUnderMouse(mousePosition);

    // If clicked on a connection then don't propagate event
    if (connection != null)
      evt.StopPropagation();

    // Select connection
    if (connection != mSelectedConnection)
    {
      var previous = mSelectedConnection;
      mSelectedConnection = connection;
      SetConnectionColor(previous);
      SetConnectionColor(mSelectedConnection);
    }

    // Store click position
    if (mSelectedConnection != null)
      mConnectionDragStartPosition = mousePosition;
  }

  private void DeleteSelectedConnection()
  {
    if (mSelectedConnection == null)
      return;

    var connectionElement = mSelectedConnection;
    var connection        = (ConnectionUserData)connectionElement.userData;

    mSelectedConnection = null;
    if (mHighlightedConnection == connectionElement)
      mHighlightedConnection = null;

    Undo.RecordObject(mMap, "Delete Level Connection");
    mMap.DeleteConnection(connection.Connection);
    mScrollView.Remove(connectionElement);
  }

  private void DragConnectionUnderMouse(MouseMoveEvent evt)
  {
    // Ignore if left mouse button is not pressed
    if (evt.pressedButtons != 1)
      return;

    // If dragging just started
    if (mConnectionDragStartPosition.HasValue && mSelectedConnection != null)
    {
      // Get point index from initial press position
      var pressPosition = mConnectionDragStartPosition.Value;
      
      // Get connection
      var connection = (ConnectionUserData)mSelectedConnection.userData;
      
      // Get points
      var intermediatePoints = connection.Connection.Points;
      var allPoints          = GetConnectionPoints(connection);

      // Keep current connection points (will need it for undo)
      mConnetionDragInitialPoints = intermediatePoints.ToArray();

      // Check each intermediate point first
      for (int i = 0; i < intermediatePoints.Count; ++i)
      {
        if (PointCircleIntersection(pressPosition, intermediatePoints[i], LevelConnectionMap.ConnectionPointRadius))
        {
          // Set point as dragged point
          mConnectionDraggedPointIndex = i;

          // Done testing
          goto done;
        }
      }

      // Check each segment
      for (int i = 1; i < allPoints.Count; ++i)
      {
        var previousPoint = allPoints[i - 1];
        var point         = allPoints[i];

        if (IsOnSegment2D(point, previousPoint, pressPosition, LevelConnectionMap.ConnectionThickness, out _, out float positionOnSegment))
        {
          // Create new point
          Vector2 segmentDirection = (point - previousPoint).normalized;
          Vector2 newPoint         = previousPoint + segmentDirection * positionOnSegment;
          int     newPointIndex    = i - 1;
          mMap.CreateConnectionPoint(connection.Connection, newPointIndex, newPoint);
          mConnectionDraggedPointIndex = newPointIndex;

          // Done testing
          goto done;
        }
      }

      // Reset
      done:
      mConnectionDragStartPosition = null;
    }

    // If currently dragging
    if (mConnectionDraggedPointIndex >= 0)
    {
      // Must have a selected connection
      Debug.Assert(mSelectedConnection != null);

      // Get connection
      var connection = (ConnectionUserData)mSelectedConnection.userData;

      // Get mouse position
      Vector2 mousePosition = rootVisualElement.ChangeCoordinatesTo(mScrollView.contentContainer, rootVisualElement.WorldToLocal(evt.mousePosition));

      // Set point position
      mMap.SetConnectionPointPosition(connection.Connection, mConnectionDraggedPointIndex, SnapPosition(mousePosition), false);

      // Redraw connection
      mSelectedConnection.MarkDirtyRepaint();
    }
  }

  private void StopConnectionDrag(MouseUpEvent evt)
  {
    // Ignore if it's not mouse left button
    if (evt.button != 0)
      return;

    // If currently dragging
    if (mConnectionDraggedPointIndex >= 0)
    {
      // Must have a selected connection
      Debug.Assert(mSelectedConnection != null);

      // Get connection
      var connection = (ConnectionUserData)mSelectedConnection.userData;

      // Get mouse position
      Vector2 mousePosition = rootVisualElement.ChangeCoordinatesTo(mScrollView.contentContainer, rootVisualElement.WorldToLocal(evt.mousePosition));

      // Set point position
      mMap.SetConnectionPointPosition(connection.Connection, mConnectionDraggedPointIndex, SnapPosition(mousePosition), true);

      // Get connection new points
      var connectionNewPoints = connection.Connection.Points.ToArray();

      // Set back old points and record undo
      connection.Connection.SetAllPoints(mConnetionDragInitialPoints);
      Undo.RecordObject(mMap, "Edit Level Connection");

      // Set back new points
      connection.Connection.SetAllPoints(connectionNewPoints);

      // Redraw connection
      mSelectedConnection.MarkDirtyRepaint();

      // Don't propagate event
      evt.StopPropagation();
    }

    // Reset
    mConnectionDraggedPointIndex = -1;
    mConnectionDragStartPosition = null;
  }
  #endregion



  #region UI
  private void CreateGUI()
  {
    // Load stylesheet
    var styleSheet = Resources.Load<StyleSheet>("LumenSection/LevelLinker/ConnectionMapUi");
    Debug.Assert(styleSheet != null, "Style Sheet not found");

    // Root
    var root = rootVisualElement;
    root.focusable = true;
    root.styleSheets.Add(styleSheet);

    // Map select drop down
    mMapSelectionDropDown = new DropdownField();
    root.Add(mMapSelectionDropDown);
    mMapSelectionDropDown.RegisterValueChangedCallback(OnSelectedMapChanged);

    // Content
    Refresh();
    
    // Refresh on change play mode
    EditorApplication.playModeStateChanged -= OnPlayModeChanged;
    EditorApplication.playModeStateChanged += OnPlayModeChanged;
  }

  private void OnPlayModeChanged(PlayModeStateChange playMode)
  {
    if (playMode is PlayModeStateChange.EnteredEditMode or PlayModeStateChange.EnteredPlayMode)
      Refresh();

    if (playMode is PlayModeStateChange.ExitingPlayMode or PlayModeStateChange.ExitingEditMode)
      UnregisterAllRootCallbacks();
  }

  private void Refresh()
  {
    Refresh(null);
  }

  private void UnregisterAllRootCallbacks()
  {
    var root = rootVisualElement;
    root.UnregisterCallback<MouseDownEvent>(StarPanning);
    root.UnregisterCallback<GeometryChangedEvent>(RecomputeScrollViewContainerSize);
    root.UnregisterCallback<MouseMoveEvent>(HighlightConnectionUnderMouse, TrickleDown.TrickleDown);
    root.UnregisterCallback<MouseDownEvent>(SelectConnectionUnderMouse, TrickleDown.TrickleDown);
    root.UnregisterCallback<MouseMoveEvent>(DragConnectionUnderMouse, TrickleDown.TrickleDown);
    root.UnregisterCallback<MouseUpEvent>(StopConnectionDrag, TrickleDown.TrickleDown);
    root.UnregisterCallback<KeyDownEvent>(OnKeyDown);
  }

  private void Refresh(LevelConnectionMap deletedMap)
  {
    var root = rootVisualElement;

    // Reset
    Reset();
    UnregisterAllRootCallbacks();

    // Give up if UI not created yet
    if (mMapSelectionDropDown == null)
      return;

    // Clear scroll view
    if (mScrollView != null)
      mScrollView.Clear();

    // If selected map is the deleted map then drop it
    if (deletedMap != null && mMap == deletedMap)
      mMap = null;

    // Get all maps and fill selection drop down
    int selected = -1;
    var maps     = new List<LevelConnectionMap>(LevelConnectionMap.GetLevelConnectionMaps());
    if (deletedMap != null)
      maps.Remove(deletedMap);
    var mapNames = new List<string>();
    for (int i = 0; i < maps.Count; i++)
    {
      mapNames.Add(maps[i].name);
      if (maps[i] == mMap)
        selected = i;
    }
    mMapSelectionDropDown.choices = mapNames;
    if (selected >= 0)
      mMapSelectionDropDown.SetValueWithoutNotify(mapNames[selected]);
    else
      mMapSelectionDropDown.SetValueWithoutNotify(null);

    // If no selected map
    if (mMap == null)
    {
      // Drop scroll view and unregister all callbacks
      if (mScrollView != null)
      {
        root.Remove(mScrollView);
        mScrollView = null;

        // Clear callbacks
        UnregisterAllRootCallbacks();
      }

      // Create no map text
      if (mNoMapText == null)
      {
        mNoMapText = new TextElement();
        mNoMapText.AddToClassList("no-map-text");
        mNoMapText.text = "No map selected.";
        root.Add(mNoMapText);
      }

      // Done
      return;
    }

    // Drop no map text
    if (mNoMapText != null)
    {
      root.Remove(mNoMapText);
      mNoMapText = null;
    }

    // Create scroll View
    if (mScrollView == null)
    {
      mScrollView = new ScrollView();
      mScrollView.AddToClassList("scroll-view");
      root.Add(mScrollView);

      // Background
      mScrollView.contentContainer.generateVisualContent = DrawBackground;

      // Force view size recompute after all elements are in place
      mMustRecomputeViewSize = true;
    }

    // Levels
    var doorToPlug = new Dictionary<string, VisualElement>();
    var levels     = mMap.Levels;
    foreach (var level in levels)
    {
      var levelElement = CreateLevelElement(level, doorToPlug);
      mScrollView.Add(levelElement);
    }

    // Connections
    var connections = mMap.Connections;
    foreach (var connection in connections)
    {
      var plug1             = doorToPlug[connection.DoorGuid1];
      var plug2             = doorToPlug[connection.DoorGuid2];
      var connectionElement = CreateConnectionElement(connection, plug1, plug2);
      mScrollView.Add(connectionElement);
    }

    // Register callbacks
    // Panning event
    root.RegisterCallback<MouseDownEvent>(StarPanning);

    // Geometry change event
    root.RegisterCallback<GeometryChangedEvent>(RecomputeScrollViewContainerSize);

    // Connection events
    if (!EditorApplication.isPlaying)
    {
      root.RegisterCallback<MouseMoveEvent>(HighlightConnectionUnderMouse, TrickleDown.TrickleDown);
      root.RegisterCallback<MouseDownEvent>(SelectConnectionUnderMouse, TrickleDown.TrickleDown);
      root.RegisterCallback<MouseMoveEvent>(DragConnectionUnderMouse, TrickleDown.TrickleDown);
      root.RegisterCallback<MouseUpEvent>(StopConnectionDrag, TrickleDown.TrickleDown);
      root.RegisterCallback<KeyDownEvent>(OnKeyDown);
    }
  }

  private void OnKeyDown(KeyDownEvent evt)
  {
    // Suppr key delete selected connection
    if (evt.keyCode == KeyCode.Delete)
    {
      DeleteSelectedConnection();
      DeleteSelectedLevel();
    }
  }

  private void OnSelectedMapChanged(ChangeEvent<string> evt)
  {
    var mapName = evt.newValue;
    var maps    = new List<LevelConnectionMap>(LevelConnectionMap.GetLevelConnectionMaps());
    foreach (var map in maps)
    {
      if (map.name == mapName)
      {
        SelectMap(map);
        Refresh();
        break;
      }
    }
  }
  #endregion



  #region Panning
  private void StarPanning(MouseDownEvent evt)
  {
    // Ignore if not right mouse button
    if (evt.button != 1)
      return;

    // Scroll view content capture mouse
    mScrollView.contentContainer.CaptureMouse();

    // Register mouse move callbacks on scroll view content
    mScrollView.contentContainer.RegisterCallback<MouseMoveEvent>(Pan);
    mScrollView.contentContainer.RegisterCallback<MouseUpEvent>(StopPanning);
  }

  private void Pan(MouseMoveEvent evt)
  {
    var viewSize    = mScrollView.contentViewport.localBound.size;
    var contentSize = mScrollView.contentContainer.localBound.size;
    var current     = mScrollView.scrollOffset;
    mScrollView.scrollOffset = new Vector2(Clamp(current.x - evt.mouseDelta.x, 0f, contentSize.x - viewSize.x), Clamp(current.y - evt.mouseDelta.y, 0f, contentSize.y - viewSize.y));
  }

  private void StopPanning(MouseUpEvent evt)
  {
    // Unregister callbacks
    mScrollView.contentContainer.UnregisterCallback<MouseMoveEvent>(Pan);
    mScrollView.contentContainer.UnregisterCallback<MouseUpEvent>(StopPanning);

    // Release mouse
    mScrollView.contentContainer.ReleaseMouse();
  }
  #endregion



  #region Geometry
  private void RecomputeScrollViewContainerSize(GeometryChangedEvent evt)
  {
    // Take children into account
    Vector2 max = Vector2.zero;
    foreach (var child in mScrollView.Children())
    {
      var childMax = mScrollView.contentContainer.WorldToLocal(child.worldBound.max);
      max.x = Max(max.x, childMax.x + 10);
      max.y = Max(max.y, childMax.y + 10);
    }

    // Take window size into account
    var viewportSize = position.size;
    max.x = Max(max.x, viewportSize.x - 0.01f);
    max.y = Max(max.y, viewportSize.y - 0.01f);

    // Take current scroll into account
    var scrollOffset = mScrollView.scrollOffset + mScrollView.contentViewport.layout.size;
    max.x = Max(max.x, scrollOffset.x);
    max.y = Max(max.y, scrollOffset.y);

    // Apply
    mScrollView.contentContainer.style.width  = max.x;
    mScrollView.contentContainer.style.height = max.y;
    
    // Done
    // Compute might fail if elements are not in place already, if so then try computing again next frame
    if (!float.IsNaN(max.x) && !float.IsNaN(max.y) && max.x > 0f && max.y > 0f)
      mMustRecomputeViewSize = false;
  }
  #endregion



  #region Events
  private void OnEnable()
  {
    mMap = null;
    Reset();

    // Load background texture
    mBackgroundTexture = Resources.Load<Texture2D>("LumenSection/LevelLinker/EditorGrid");

    // Register callback on map file change
    LevelConnectionMap.OnFileChanged += Refresh;

    // Callback for undo
    Undo.undoRedoPerformed += Refresh;

    // Register callback on map change
    var maps = LevelConnectionMap.GetLevelConnectionMaps();
    if (maps.Length > 0)
    {
      SelectMap(maps[0]);
    }
    else
    {
      SelectMap(null);
    }
  }

  private void OnDisable()
  {
    // Unselect map
    SelectMap(null);

    // Unregister callbacks
    LevelConnectionMap.OnFileChanged -= Refresh;
    Undo.undoRedoPerformed           -= Refresh;

    // Drop texture
    mBackgroundTexture = null;

    // Reset
    Reset();
    mMap = null;
  }

  private void OnFocus()
  {
    rootVisualElement.Focus();
  }

  private void Update()
  {
    mDragOperation?.UpdateAction?.Invoke();
    if (mMustRecomputeViewSize)
      RecomputeScrollViewContainerSize(null);
  }
  #endregion
}
}
