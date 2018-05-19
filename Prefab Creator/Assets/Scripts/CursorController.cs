using System;
using UnityEngine;

[Serializable]
public class CursorController {

    public Texture2D reticleTexture;
    public Texture2D grabTexture;

    public void SetCursorNormal()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void SetCursorReticle()
    {
        Cursor.SetCursor(reticleTexture, new Vector2(reticleTexture.width / 2, reticleTexture.height / 2), CursorMode.Auto);
    }

    public void SetCursorGrab()
    {
        Cursor.SetCursor(grabTexture, new Vector2(grabTexture.width / 2, grabTexture.height / 2), CursorMode.Auto);
    }
}
