using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/**
 *       TITLE: MainSceneController.cs
 *      AUTHOR: 곽수환
 *        DATE: 2018-06-05
 * DESCRIPTION: 메인 씬의 UI와 씬간 연결을 진행하는 스크립트
 *     DEV LOG: 
 */

public class MainSceneController : MonoBehaviour
{
    [Header("UI Display List")]
    [SerializeField] Canvas canvas;
    [SerializeField] GameObject uiTeamLogo;
    [SerializeField] float fTeamLogoPlayTime = 3.0f;
    [SerializeField] GameObject uiPressAnyKey;
    [SerializeField] GameObject uiMissionSelect;

    [Header("Scene Icon")]
    [SerializeField] GameObject iconModeSelector;
    int nDisplayedIconNum = 0;

    [Header("Scene List")]
    [SerializeField] GameModeSceneInfo[] gmsiList;

    enum UIStatus { None = 0, TeamLogo = 1, PressAnyKey = 2, MissionSelect = 3 }
    UIStatus uiStatus = UIStatus.None;

    Vector3 v3LeftBottomCorner = new Vector3(-73.0f, -37.0f, 0.0f),
            v3RightTopCorner   = new Vector3( 73.0f,  37.0f, 0.0f);

    bool bIsDisplayingContract = false;

	void Start ()
    {
		// @TODO: 씬 로딩할 때 uiStatus값을 읽어옴

        switch (uiStatus)
        {
            // 처음 켤 경우 팀 로고를 보여준다
            case UIStatus.None:
                uiStatus = UIStatus.TeamLogo;
                StartCoroutine(PlayTeamLogo());
                break;

            // 씬을 임무 선택 화면으로 시작할 경우
            case UIStatus.MissionSelect:
                uiStatus = UIStatus.MissionSelect;
                break;
        }
	}
	
	void Update ()
    {
        switch (uiStatus)
        {
            case UIStatus.PressAnyKey:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    uiPressAnyKey.SetActive(false);
                    uiMissionSelect.SetActive(true);

                    uiStatus = UIStatus.MissionSelect;
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    UnityEditor.EditorApplication.isPlaying = false;
                }
                break;

            case UIStatus.MissionSelect:
                uiMissionSelect.SetActive(true);

                if (nDisplayedIconNum < gmsiList.Length && !bIsDisplayingContract)
                {
                    StartCoroutine(DisplayContractIcon());
                }
                break;
        }
    }

    IEnumerator PlayTeamLogo()
    {
        float fPlayTime = Time.time;

        uiTeamLogo.SetActive(true);

        while (Time.time < fPlayTime + fTeamLogoPlayTime)
        {
            yield return new WaitForEndOfFrame();
        }

        uiTeamLogo.SetActive(false);
        uiPressAnyKey.SetActive(true);

        uiStatus = UIStatus.PressAnyKey;
    }

    IEnumerator DisplayContractIcon()
    {
        bIsDisplayingContract = true;

        for (int i = 0; i < gmsiList.Length; i++)
        {
            if (!gmsiList[i].bDisplayed)
            {
                gmsiList[i].bDisplayed = true;

                gmsiList[i].v3Position.x = Random.Range(v3LeftBottomCorner.x, v3RightTopCorner.x);
                gmsiList[i].v3Position.y = Random.Range(v3LeftBottomCorner.y, v3RightTopCorner.y);
                gmsiList[i].v3Position.z = canvas.transform.position.z;

                Debug.Log(gmsiList[i].v3Position);

                GameObject objIcon = Instantiate(iconModeSelector, gmsiList[i].v3Position, Quaternion.identity, uiMissionSelect.transform);
                objIcon.transform.localPosition = gmsiList[i].v3Position;
                objIcon.GetComponent<Image>().material.mainTexture = gmsiList[i].textureIcon;

                nDisplayedIconNum++;

                yield return new WaitForSeconds(1.5f);
            }
        }

        bIsDisplayingContract = false;
    }
}
