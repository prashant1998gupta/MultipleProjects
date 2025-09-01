using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    public Material lineMaterial;
    public float lineWidth = 0.1f;

    private LineRenderer currentLine;
    public List<Vector3> currentPoints = new List<Vector3>();
    public Stack<GameObject> drawnLines = new Stack<GameObject>();
    public Stack<GameObject> redoLines = new Stack<GameObject>();

    void Update()
    {
        HandleDrawing();
        HandleUndoRedo();
    }

    void HandleDrawing()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartNewLine();
        }

        if (Input.GetMouseButton(0) && currentLine != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;

            if (currentPoints.Count == 0 || Vector3.Distance(currentPoints[currentPoints.Count - 1], mousePos) > 0.05f)
            {
                currentPoints.Add(mousePos);
                currentLine.positionCount = currentPoints.Count;
                currentLine.SetPositions(currentPoints.ToArray());
            }
        }

        if (Input.GetMouseButtonUp(0) && currentLine != null)
        {
            if (currentPoints.Count < 2)
            {
                Destroy(currentLine.gameObject); // Ignore tiny strokes
            }
            else
            {
                drawnLines.Push(currentLine.gameObject);
            }

            currentLine = null;
            currentPoints.Clear();
            redoLines.Clear(); // Invalidate redo stack
        }
    }

    void StartNewLine()
    {
        GameObject lineObj = new GameObject("Line");
        currentLine = lineObj.AddComponent<LineRenderer>();
        currentLine.material = lineMaterial;
        currentLine.startWidth = lineWidth;
        currentLine.endWidth = lineWidth;
        currentLine.positionCount = 0;
        currentLine.useWorldSpace = true;
        currentLine.numCapVertices = 8;
        currentLine.numCornerVertices = 8;
        currentLine.alignment = LineAlignment.TransformZ;
        currentPoints.Clear();
    }

    void HandleUndoRedo()
    {
        if (Input.GetKeyDown(KeyCode.Z) && drawnLines.Count > 0)
        {
            GameObject line = drawnLines.Pop();
            line.SetActive(false);
            redoLines.Push(line);
        }

        if (Input.GetKeyDown(KeyCode.Y) && redoLines.Count > 0)
        {
            GameObject line = redoLines.Pop();
            line.SetActive(true);
            drawnLines.Push(line);
        }
    }

    public void ClearAllLines()
    {
        foreach (var line in drawnLines)
        {
            Destroy(line);
        }
        drawnLines.Clear();
        redoLines.Clear();
    }

    public void Undo()
    {
        if (drawnLines.Count > 0)
        {
            GameObject line = drawnLines.Pop();
            line.SetActive(false);
            redoLines.Push(line);
        }
    }

    public void Redo()
    {
        if (redoLines.Count > 0)
        {
            GameObject line = redoLines.Pop();
            line.SetActive(true);
            drawnLines.Push(line);
        }
    }

}
