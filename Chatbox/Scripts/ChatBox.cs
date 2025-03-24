using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ChatBox : RegulatorSingleton<ChatBox>
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField chatInputField; // The input field for entering messages
    [SerializeField] private Transform content; // The parent transform for chat messages
    [SerializeField] private GameObject chatMessagePrefab; // Prefab for chat messages
    [SerializeField] private CanvasGroup chatboxCanvasGroup; // Canvas group for fading the chatbox

    [Header("Chatbox Settings")]
    public bool hideChatbox = false; // Auto-hide feature toggle
    public float hideTimeout = 5f; // Time before chatbox auto-hides
    public bool autoSendMessage = true;
    private Coroutine hideCoroutine;

    [Header("Events")]
    public UnityEvent<string> OnMessageSent; // Event triggered when a message is sent
    public UnityEvent<string> OnCommandSent; // Event triggered when a command (message starting with ::) is sent
    public UnityEvent<string> OnSystemMessageAdded; // Event triggered when a system message is added
    public UnityEvent OnChatCleared; // Event triggered when the chat is cleared

    private Dictionary<string, GameObject> debugMessages = new Dictionary<string, GameObject>();

    void Start()
    {
        // Hook the input field to trigger the SendMessage method
        chatInputField.onSubmit.AddListener(SendChatboxMessage);
        chatInputField.onValueChanged.AddListener(ResetHideTimer); // Reset hide timer on input
    }

    private void Update()
    {
        if (hideChatbox && Input.GetKeyDown(KeyCode.Return))
        {
            ShowChatbox();
            FocusOnInputField();
        }
    }

    /// <summary>
    /// Sends a user message.
    /// </summary>
    /// <param name="message">The message text.</param>
    private void SendChatboxMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return;

        if (message.StartsWith("::"))
        {
            OnCommandSent?.Invoke(message);
        }
        else
        {
            OnMessageSent?.Invoke(message);
            if (autoSendMessage)
            {
                CreateChatMessage(message);
            }
        }

        // Clear the input field and re-focus
        chatInputField.text = string.Empty;
        FocusOnInputField();

        // Reset hide timer
        ResetHideTimer();
    }

    /// <summary>
    /// Adds a system message to the chat.
    /// </summary>
    public void CreateSystemMessage(string message)
    {
        GameObject newMessage = Instantiate(chatMessagePrefab, content);
        ChatMessage chatMessage = newMessage.GetComponent<ChatMessage>();
        chatMessage.SetSystemMessage(message);

        ScrollToBottom();
        OnSystemMessageAdded?.Invoke(message);
        ResetHideTimer();
    }

    /// <summary>
    /// Clears all messages from the chat.
    /// </summary>
    public void ClearChat()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        debugMessages.Clear();
        OnChatCleared?.Invoke();
        ResetHideTimer();
    }

    /// <summary>
    /// Creates and displays a chat message.
    /// </summary>
    public void CreateChatMessage(string message)
    {
        GameObject newMessage = Instantiate(chatMessagePrefab, content);
        ChatMessage chatMessage = newMessage.GetComponent<ChatMessage>();
        chatMessage.SetMessage(message);
        ScrollToBottom();
        ResetHideTimer();
    }

    /// <summary>
    /// Creates or updates a debug message.
    /// </summary>
    /// <param name="prefix">A unique identifier for the debug message. Used to group and overwrite messages with the same prefix.</param>
    /// <param name="message">The content of the debug message to display in the chatbox.</param>
    public void CreateDebugMessage(string prefix, string message)
    {
        if (debugMessages.TryGetValue(prefix, out GameObject existingMessage))
        {
            existingMessage.GetComponent<ChatMessage>().SetMessage($"{prefix}: {message}");
        }
        else
        {
            GameObject newMessage = Instantiate(chatMessagePrefab, content);
            ChatMessage chatMessage = newMessage.GetComponent<ChatMessage>();
            chatMessage.SetDebugMessage($"{prefix}: {message}");
            debugMessages[prefix] = newMessage;
        }

        ScrollToBottom();
        ResetHideTimer();
    }

    /// <summary>
    /// Scrolls the chat to the bottom.
    /// </summary>
    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        GetComponentInChildren<ScrollRect>().verticalNormalizedPosition = 0;
    }

    /// <summary>
    /// Focuses back on the input field.
    /// </summary>
    public void FocusOnInputField()
    {
        chatInputField.Select();
        chatInputField.ActivateInputField();
    }

    /// <summary>
    /// Resets the hide timer for the chatbox.
    /// </summary>
    public void ResetHideTimer(string _ = null)
    {
        if (!hideChatbox) return;

        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        ShowChatbox();
        hideCoroutine = StartCoroutine(HideChatboxAfterTimeout());
    }

    /// <summary>
    /// Hides the chatbox after the timeout period.
    /// </summary>
    private IEnumerator HideChatboxAfterTimeout()
    {
        yield return new WaitForSeconds(hideTimeout);
        HideChatbox();
    }

    /// <summary>
    /// Shows the chatbox.
    /// </summary>
    private void ShowChatbox()
    {
        chatboxCanvasGroup.alpha = 1;
        chatboxCanvasGroup.interactable = true;
        chatboxCanvasGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// Hides the chatbox.
    /// </summary>
    private void HideChatbox()
    {
        chatboxCanvasGroup.alpha = 0;
        chatboxCanvasGroup.interactable = false;
        chatboxCanvasGroup.blocksRaycasts = false;
    }
}
