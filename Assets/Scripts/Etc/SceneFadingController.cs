using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFadingController : MonoBehaviour
{
    [Header("Fading Setting")]
    [SerializeField] float fFadingSpeed = 2.0f;

    [SerializeField] Image imgPanel;

    Color colorStartFadeIn = new Color(0.0f, 0.0f, 0.0f, 1.0f);
    Color colorStartFadeOut = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    Color colorCurrent = new Color();

    bool bIsFading = false;

    void Start ()
    {
        imgPanel = GetComponent<Image>();
        imgPanel.raycastTarget = false;

        imgPanel.color = colorStartFadeOut;
    }

    public void FadeIn() { if (!bIsFading) StartCoroutine("_FadeIn"); }
    // 일반적인 페이드 인
	IEnumerator _FadeIn()
    {
        bIsFading = true;
        colorCurrent = colorStartFadeIn;

        while (colorCurrent.a > 0.0f)
        {
            colorCurrent.a -= (1.0f / fFadingSpeed) * Time.deltaTime;
            imgPanel.color = colorCurrent;

            yield return new WaitForEndOfFrame();
        }

        imgPanel.color = colorStartFadeOut;
        bIsFading = false;
    }

    public void FadeOut(bool bLoadCurrentScene) { if (!bIsFading) StartCoroutine("_FadeOut", bLoadCurrentScene); }
    // 일반적인 페이드 아웃
    IEnumerator _FadeOut(bool bLoadCurrentScene = false)
    {
        bIsFading = true;
        colorCurrent = colorStartFadeOut;

        while (colorCurrent.a < 1.0f)
        {
            colorCurrent.a += (1.0f / fFadingSpeed) * Time.deltaTime;
            imgPanel.color = colorCurrent;

            yield return new WaitForEndOfFrame();
        }

        imgPanel.color = colorStartFadeIn;

        bIsFading = false;

        if (bLoadCurrentScene)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        else
            SceneManager.LoadScene("Lobby");
    }

    public void FadeOutForLoad(string strSceneName) { if (!bIsFading) StartCoroutine("_FadeOutForLoad", strSceneName); }
    // 다른 씬을 불러오기 위한 페이드 아웃
    IEnumerator _FadeOutForLoad(string strSceneName)
    {
        bIsFading = true;
        colorCurrent = colorStartFadeOut;

        while (colorCurrent.a < 1.0f)
        {
            colorCurrent.a += (1.0f / fFadingSpeed) * Time.deltaTime;
            imgPanel.color = colorCurrent;

            yield return new WaitForEndOfFrame();
        }

        imgPanel.color = colorStartFadeIn;

        bIsFading = false;

        SceneManager.LoadScene(strSceneName);
    }
    public void FadeOutForLoad(int nSceneNum) { if (!bIsFading) StartCoroutine("_FadeOutForLoad", nSceneNum); }
    // Override
    IEnumerator _FadeOutForLoad(int nSceneNum)
    {
        bIsFading = true;
        colorCurrent = colorStartFadeOut;

        while (colorCurrent.a < 1.0f)
        {
            colorCurrent.a += (1.0f / fFadingSpeed) * Time.deltaTime;
            imgPanel.color = colorCurrent;

            yield return new WaitForEndOfFrame();
        }

        imgPanel.color = colorStartFadeIn;

        bIsFading = false;

        SceneManager.LoadScene(nSceneNum);
    }

    public void FadeOutForExit() { if (!bIsFading) StartCoroutine("_FadeOutForExit"); }
    // 종료용 페이드 아웃
    IEnumerator _FadeOutForExit()
    {
        bIsFading = true;
        colorCurrent = colorStartFadeOut;

        while (colorCurrent.a < 1.0f)
        {
            colorCurrent.a += (1.0f / fFadingSpeed) * Time.deltaTime;
            imgPanel.color = colorCurrent;

            yield return new WaitForEndOfFrame();
        }

        imgPanel.color = colorStartFadeIn;
        bIsFading = false;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void FadeOutAndIn(System.Action callback) { if (!bIsFading) StartCoroutine("_FadeOutAndIn", callback); }
    // 장면 전환용 페이드 아웃과 인
    IEnumerator _FadeOutAndIn(System.Action callback)
    {
        bIsFading = true;
        colorCurrent = colorStartFadeOut;

        while (colorCurrent.a < 1.0f)
        {
            colorCurrent.a += (1.0f / fFadingSpeed) * Time.deltaTime;
            imgPanel.color = colorCurrent;

            yield return new WaitForEndOfFrame();
        }

        callback();
        imgPanel.color = colorStartFadeIn;

        while (colorCurrent.a > 0.0f)
        {
            colorCurrent.a -= (1.0f / fFadingSpeed) * Time.deltaTime;
            imgPanel.color = colorCurrent;

            yield return new WaitForEndOfFrame();
        }

        imgPanel.color = colorStartFadeOut;
        bIsFading = false;
    }
}
