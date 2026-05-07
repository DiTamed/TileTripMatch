using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D cursorTexture;

    void Start()
    {
        Vector2 hotspot = Vector2.zero;

        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
    }
}