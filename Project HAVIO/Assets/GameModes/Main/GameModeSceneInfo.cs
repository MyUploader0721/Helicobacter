using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/**
 *       TITLE: GameModeSceneInfo.cs
 *      AUTHOR: 곽수환
 *        DATE: 2018-06-05
 * DESCRIPTION: 게임모드를 실행할 때 사용하는 스크립트입니다. 
 *     DEV LOG: 사실상 게임모드의 정보가 들어있는 스크립트입니다. 
 *              버튼을 클릭하면 게임모드를 시작할 수 있습니다. 
 */

public class GameModeSceneInfo : MonoBehaviour
{
    [Header("Mission Info")]
    public string strSceneName;
    public string strGameModeType;
    public string strGameModeDescription;
    public Sprite spriteDescription;
    public Object scene;
    public Sprite spriteIcon;

    [Header("You don't have to Touch")]
    public Vector3 v3Position = Vector3.zero;
    public bool bDisplayed = false;
    public int nNumber;

	void Start ()
    {
		
	}

    /// <summary>
    /// 게임모드씬 정보를 초기화합니다. 
    /// </summary>
    /// <param name="strSceneName">현재 계약(임무)의 이름(타이틀)입니다. </param>
    /// <param name="strGameModeType">현재 계약(임무)의 종류를 적습니다. </param>
    /// <param name="strGameModeDescription">현재 계약(임무)의 설명을 적습니다. 시간이나 목표 등의 디테일을 적어주면 좋습니다. </param>
    /// <param name="spriteDescription">현재 계약(임무)와 관련된 사진을 할당합니다. </param>
    /// <param name="scene">현재 계약(임무)을 수락할 경우 연결될 씬을 할당합니다. </param>
    public void InitInfo(string strSceneName, string strGameModeType, string strGameModeDescription, Sprite spriteDescription, Object scene)
    {
        this.strSceneName           = strSceneName;
        this.strGameModeType        = strGameModeType;
        this.strGameModeDescription = strGameModeDescription;
        this.spriteDescription      = spriteDescription;
        this.scene                  = scene;
    }
}
