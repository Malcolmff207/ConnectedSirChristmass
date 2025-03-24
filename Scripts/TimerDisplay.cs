using TMPro;
using UnityEngine;
using Unity.Netcode;
public class TimerDisplay : NetworkBehaviour
{

    [SerializeField] private TMP_Text timerText;
    private GameTimer _gameTimer;

    public override void OnNetworkSpawn()
    {
        if (!IsClient) return;
        _gameTimer = FindObjectOfType<GameTimer>();
        //Subscribing to changes in the timeRemaining network variable
        _gameTimer.timeRemaining.OnValueChanged += OnTimerChanged;
    }

    public override void OnNetworkDespawn()
    {
        //Unsubscribing to changes in the timeRemaining network variable
        _gameTimer.timeRemaining.OnValueChanged -= OnTimerChanged;
    }
    
    private void OnTimerChanged(float prevTime, float newTime)
    {
        UpdateTimerDisplay(newTime);
    }
    
    private void UpdateTimerDisplay(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}
