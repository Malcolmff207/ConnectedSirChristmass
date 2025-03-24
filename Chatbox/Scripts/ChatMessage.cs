using TMPro;
using UnityEngine;

/// <summary>
/// Represents a single chat message in the chatbox system.
/// Handles message content, styling, and dynamic size adjustments based on text length.
/// </summary>
public class ChatMessage : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("RectTransform of the chat message container.")]
    [SerializeField] private RectTransform messageRect; 
    
    [Tooltip("TextMeshPro component for displaying the message.")]
    [SerializeField] private TMP_Text messageText;

    /// <summary>
    /// Sets the message content and adjusts the message's appearance dynamically.
    /// </summary>
    /// <param name="text">The content of the message.</param>
    public void SetMessage(string text)
    {
        if (!ValidateComponents()) return;

        // Set the message text
        messageText.text = text;

        // Adjust the message height dynamically
        AdjustHeightBasedOnText(text);
    }

    /// <summary>
    /// Sets the message as a system message with specific styling and content.
    /// </summary>
    /// <param name="text">The content of the system message.</param>
    public void SetSystemMessage(string text)
    {
        if (!ValidateComponents()) return;

        // Set the system message text
        messageText.text = $"System: {text}";
        messageText.color = Color.red;

        // Adjust the message height dynamically
        AdjustHeightBasedOnText(text);
    }
    
    /// <summary>
    /// Sets the message as a debug message with specific styling and content.
    /// </summary>
    /// <param name="text">The content of the debug message.</param>
    public void SetDebugMessage(string text)
    {
        if (!ValidateComponents()) return;

        // Set the system message text
        messageText.text = text;
        messageText.color = Color.blue;

        // Adjust the message height dynamically
        AdjustHeightBasedOnText(text);
    }

    /// <summary>
    /// Validates that all required components are assigned and logs errors if not.
    /// </summary>
    /// <returns>True if all components are valid; otherwise, false.</returns>
    private bool ValidateComponents()
    {
        if (messageText == null)
        {
            Debug.LogError("ChatMessage: TMP_Text component is not assigned.");
            return false;
        }

        if (messageRect == null)
        {
            Debug.LogError("ChatMessage: RectTransform component is not assigned.");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Dynamically adjusts the height of the chat message based on text length.
    /// </summary>
    /// <param name="text">The message text used to calculate height.</param>
    private void AdjustHeightBasedOnText(string text)
    {
        // Force TextMeshPro to update its geometry to calculate preferred height
        messageText.ForceMeshUpdate();

        // Calculate the preferred height of the text
        float preferredHeight = messageText.preferredHeight;

        // Adjust height: double for long messages, normal for short
        messageRect.sizeDelta = new Vector2(
            messageRect.sizeDelta.x,
            text.Length > 60 ? preferredHeight * 2 : preferredHeight
        );
    }
}
