using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause_UI : MonoBehaviour
{
    public Button resume_button;
    public Button Setting_button;
    public Button PauseButton;
    public GameObject OnPauseUI;
    private void Awake()
    {
        resume_button.onClick.AddListener(() =>
        {
            OnPauseUI.SetActive(false);
            Time.timeScale = 1;
        });
        PauseButton.onClick.AddListener(() =>
        {
            OnPauseUI.SetActive(true);
            Time.timeScale = 0;
        });
        OnPauseUI.SetActive(false);
    }
    private void Update()
    {
      
    }
}
