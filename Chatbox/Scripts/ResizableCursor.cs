using System;
using UnityEngine;

/// <summary>
/// Provides functionality for a resizable chatbox using mouse interaction.
/// Allows resizing when the mouse is near the top-right edge of the chat panel.
/// </summary>
public class ResizableChatBox : MonoBehaviour
{
    [Header("Chat Panel Configuration")]
    [Tooltip("The RectTransform of the chat panel to resize.")]
    [SerializeField] private RectTransform chatPanel;

    [Tooltip("Distance from the edge to detect resizing.")]
    [SerializeField] private float edgeThreshold = 5f;

    [Tooltip("Minimum width of the chatbox.")]
    [SerializeField] private float minWidth = 500f;

    [Tooltip("Minimum height of the chatbox.")]
    [SerializeField] private float minHeight = 300f;

    [Tooltip("Optional cursor texture to indicate resizing.")]
    [SerializeField] private Texture2D cursorTexture;

    private bool isResizing = false;

    void Update()
    {
        if (!chatPanel) return;

        // Get the local mouse position relative to the chat panel
        Vector2 localMousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            chatPanel,
            Input.mousePosition,
            null, // Assuming Screen Space Overlay Canvas
            out localMousePosition
        );

        // Check if the mouse is near the resizing edge
        if (IsMouseNearEdge(localMousePosition))
        {
            // Change the cursor to the resize cursor
            SetCursor(cursorTexture);

            // Start resizing on left mouse button press
            if (Input.GetMouseButtonDown(0))
            {
                isResizing = true;
            }
        }
        else if (!isResizing)
        {
            // Reset to the default pointer cursor
            SetCursor(null);
        }

        // Handle resizing logic while the left mouse button is held
        if (isResizing && Input.GetMouseButton(0))
        {
            ResizePanel(localMousePosition);
            ChatBox.Instance.ResetHideTimer();
        }

        // Stop resizing when the left mouse button is released
        if (Input.GetMouseButtonUp(0))
        {
            isResizing = false;
        }
    }

    /// <summary>
    /// Determines if the mouse is near the top-right corner of the chat panel.
    /// </summary>
    /// <param name="localMousePosition">Mouse position relative to the chat panel.</param>
    /// <returns>True if the mouse is near the resizing edge, otherwise false.</returns>
    private bool IsMouseNearEdge(Vector2 localMousePosition)
    {
        if (!chatPanel) return false;

        float width = chatPanel.rect.width;
        float height = chatPanel.rect.height;

        // Check if the mouse is within the edge threshold for resizing
        return localMousePosition.x > width - edgeThreshold &&
               localMousePosition.x < width + edgeThreshold&&
               localMousePosition.y > height - edgeThreshold &&
               localMousePosition.y < height + edgeThreshold;
    }

    /// <summary>
    /// Resizes the chat panel based on the current mouse position.
    /// </summary>
    private void ResizePanel(Vector2 localMousePosition)
    {
        if (!chatPanel) return;
        // Calculate new size clamped to screen dimensions and minimum sizes
        
        Vector2 newSize = new Vector2(
            Mathf.Clamp(localMousePosition.x, minWidth, Screen.width),
            Mathf.Clamp(localMousePosition.y, minHeight, Screen.height)
        );

        // Apply the new size to the chat panel
        chatPanel.sizeDelta = newSize;
    }

    /// <summary>
    /// Sets the mouse cursor to the specified texture.
    /// </summary>
    /// <param name="cursorTexture">The texture to use for the cursor. Use null for the default cursor.</param>
    private void SetCursor(Texture2D cursorTexture)
    {
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }
}
