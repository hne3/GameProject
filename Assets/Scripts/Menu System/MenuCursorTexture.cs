﻿using UnityEngine;

/// <summary>
/// Sets the cursor texture for the game. This cursor texture persists between
/// scenes until set again.
/// </summary>
public class MenuCursorTexture : MonoBehaviour
{
    /// <summary>
    /// The texture to set to the cursor.
    /// </summary>
    public Texture2D cursorTexture;

    /// <summary>
    /// The cursor mode.
    /// </summary>
    public CursorMode cursorMode = CursorMode.Auto;

    /// <summary>
    /// The cursor hotspot. The point in the cursor where the "point" is.
    /// </summary>
    public Vector2 hotSpot = Vector2.zero;

    /// <summary>
    /// Run at Start of scene by Unity.
    /// </summary>
    void Start()
    {
        // Check for a cursor texture.
        if (this.cursorTexture == null)
        {
            Debug.LogError("MenuCursorTexture script requires a Cursor texture.");
            return;
        }

        // Check that mouse hotspot is within the cursor bounds.
        if (this.hotSpot.x > this.cursorTexture.width || this.hotSpot.y > this.cursorTexture.height)
        {
            Debug.LogError("MenuCursorTexture script hotspot is outside of the cursor texture bounds.");
        }

        Cursor.SetCursor(this.cursorTexture, this.hotSpot, this.cursorMode);
    }
}
