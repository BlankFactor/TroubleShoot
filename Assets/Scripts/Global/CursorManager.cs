using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager instance;

    public Texture2D cursor_Normal;
    public Texture2D cursor_Dist;
    public Texture2D cursor_Detect;
    public Texture2D cursor_Troubleshoot;
    public Texture2D cursor_Temper;

    private CursorMode cm = CursorMode.Auto;

    private void Awake()
    {
        instance = this;
    }

    public void SetCursor_Normal() {
        Cursor.SetCursor(cursor_Normal, Vector2.zero, cm);
    }
    public void SetCursor_Dist()
    {
        Cursor.SetCursor(cursor_Dist, Vector2.zero, cm);
    }
    public void SetCursor_Detect()
    {
        Cursor.SetCursor(cursor_Detect, Vector2.zero, cm);
    }
    public void SetCursor_Troubleshoot()
    {
        Cursor.SetCursor(cursor_Troubleshoot, Vector2.zero, cm);
    }
    public void SetCursor_Temper()
    {
        Cursor.SetCursor(cursor_Temper, Vector2.zero, cm);
    }
}
