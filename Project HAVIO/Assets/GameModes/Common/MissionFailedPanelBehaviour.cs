﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using TMPro;

public class MissionFailedPanelBehaviour : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] Button btnRestart;
    [SerializeField] Button btnQuit;
    [Space]
    [Header("Texts")]
    [SerializeField] TextMeshProUGUI txtMissionTime;
    [Space]
    [Header("Fader")]
    [SerializeField] SceneFadingController sceneFadingController;

    void Start()
    {
        btnRestart.onClick.AddListener(OnButtonRestartClicked);
        btnQuit.onClick.AddListener(OnButtonQuitClicked);

        txtMissionTime.text = ((int)(Time.timeSinceLevelLoad / 60.0f)).ToString("00") + ":" + ((int)Time.timeSinceLevelLoad % 60).ToString("00");
    }

    void Update()
    {

    }

    void OnButtonRestartClicked()
    {
        sceneFadingController.FadeOut(true);
    }

    void OnButtonQuitClicked()
    {
        sceneFadingController.FadeOut(false);
    }
}
