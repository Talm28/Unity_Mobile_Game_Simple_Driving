using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{  
    [SerializeField] private TMP_Text _highScoreText;
    [SerializeField] private TMP_Text _energyText;
    [SerializeField] private Button _playButton;
    [SerializeField] private AndroidNotificationHandler androidNotificationHandler;
    [SerializeField] private int _maxEnergy;
    [SerializeField] private int _energyRechargeDuration;

    private int _energy;
    private const string EnergyKey = "Energy";
    private const string EnergyReadyKey = "EnergyReady";

    void Start()
    {
        OnApplicationFocus(true);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) {return;}

        // Cancel other invkoed method if app reopen
        CancelInvoke();

        // High score load
        int highScore = PlayerPrefs.GetInt(ScoreSystem.HighScoreKey, 0);
        _highScoreText.text = $" High Score: {highScore}";

        // Energy load
        _energy = PlayerPrefs.GetInt(EnergyKey, _maxEnergy);

        if(_energy == 0)
        {
            // Get current energy ready datetime
            string energyReadyString = PlayerPrefs.GetString(EnergyReadyKey, string.Empty);

            if(energyReadyString == string.Empty)
                return;
            
            DateTime energyReady = DateTime.Parse(energyReadyString);

            // Recharge energy
            if(DateTime.Now > energyReady)
            {
                _energy = _maxEnergy;
                PlayerPrefs.SetInt(EnergyKey, _energy);
            }
            else
            {
                _playButton.interactable = false;
                Invoke(nameof(EnergyRecharged), (energyReady - DateTime.Now).Seconds);
            }
        }

        _energyText.text = $"Play ({_energy})";
    }

    private void EnergyRecharged()
    {
        _playButton.interactable = true;
        _energy = _maxEnergy;
        PlayerPrefs.SetInt(EnergyKey, _energy);
        _energyText.text = $"Play ({_energy})";
    }

    public void PlayGame()
    {
        if(_energy > 0)
        {
            _energy -= 1;
            PlayerPrefs.SetInt(EnergyKey, _energy);

            if(_energy == 0)
            {
                DateTime nextRechargeDateTime = DateTime.Now.AddMinutes(_energyRechargeDuration);
                PlayerPrefs.SetString(EnergyReadyKey, nextRechargeDateTime.ToString());

                // Schedule a notification
#if UNITY_ANDROID
                androidNotificationHandler.ScheduleNotification(nextRechargeDateTime);
#endif
            }

            SceneManager.LoadScene(1);
        }

        
    }
}
