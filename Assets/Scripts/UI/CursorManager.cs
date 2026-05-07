using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;

    [SerializeField]
    private Texture2D cursorTexture;

    [SerializeField]
    private Vector2 hotspot =
        Vector2.zero;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        SetCursor();
    }

    private void SetCursor()
    {
        Cursor.SetCursor(
            cursorTexture,
            hotspot,
            CursorMode.Auto
        );
    }
}