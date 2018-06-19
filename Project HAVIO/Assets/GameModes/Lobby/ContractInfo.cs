using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/**
 *       TITLE: ContractInfo.cs
 *      AUTHOR: 곽수환
 *        DATE: 2018-06-05
 * DESCRIPTION: 게임모드를 실행할 때 사용하는 스크립트입니다. 
 *     DEV LOG: 사실상 게임모드의 정보가 들어있는 스크립트입니다. 
 *              버튼을 클릭하면 게임모드를 시작할 수 있습니다. 
 *     2018-06-19: 사실감을 높이기 위해 계약자의 이름을 추가합니다. 
 *                 계약의 주요사항(시간제한 등)을 기록할 수 있습니다. 
 *                 GameModeSceneInfo에서 ContractInfo로 변경합니다. 
 */

public class ContractInfo : MonoBehaviour
{
    [Header("Contract Info")]
    public string strContractTitle;
    public string strGameModeType;
    public Sprite spriteGameModeIcon;
    [TextArea] public string strContractDescription;
    [TextArea] public string strContractConditions;
    public Sprite spriteContractImage;
    public Object sceneToContinue;

    [HideInInspector] public bool bDisplayed = false;
    [HideInInspector] public string strContractorName;
    [HideInInspector] public string strBtnHashData = "";

    void Start ()
    {
		
	}

    /// <summary>
    /// 계약 정보를 초기화합니다. 언제 또 사용할 지 몰라서 이름을 <code>SetData()</code>로 했습니다. 
    /// </summary>
    /// <param name="strContractTitle">계약 제목</param>
    /// <param name="strGameModeType">계약 게임모드 이름</param>
    /// <param name="spriteGameModeIcon">게임모드 아이콘</param>
    /// <param name="strContractDescription">계약 세부 정보 및 설명</param>
    /// <param name="strContractConditions">계약 조건(리스트로 3줄 정도 추천)</param>
    /// <param name="spriteContractImage">계약 이미지 정보</param>
    /// <param name="sceneToContinue">계약 게임모드의 씬</param>
    /// <param name="strContractorName">계약자 이름 설정</param>
    public void SetData(
        string strContractTitle,      string strGameModeType,
        Sprite spriteGameModeIcon,    string strContractDescription,
        string strContractConditions, Sprite spriteContractImage,
        Object sceneToContinue,       string strContractorName
    )
    {
        this.strContractTitle       = strContractTitle;
        this.strGameModeType        = strGameModeType;
        this.spriteGameModeIcon     = spriteGameModeIcon;
        this.strContractDescription = strContractDescription;
        this.strContractConditions  = strContractConditions;
        this.spriteContractImage    = spriteContractImage;
        this.sceneToContinue        = sceneToContinue;
        this.strContractorName      = strContractorName;
    }
}
