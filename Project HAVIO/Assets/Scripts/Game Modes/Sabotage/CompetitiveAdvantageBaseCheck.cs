using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompetitiveAdvantageBaseCheck : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] GameObject[] objTarget;
    [SerializeField] GameObject[] objAAGun;
    [Space]
    [Header("SFX")]
    [SerializeField] CompetitiveAdvantageSFXPlayer sfxPlayer;
    [Space]
    [SerializeField] AudioClip sfxSecured;
    [Space]
    [SerializeField] bool bIsLastBase = false;
    [SerializeField] AudioClip sfxRTB;
    [Space]
    [SerializeField] PDStyleBGMPlayer bgmplayer;
    [Space]
    [Header("End this Base")]
    [SerializeField] GameObject objBlocking;

    int nTargetRemained;
    int nAAGunRemained;

	void Start ()
    {
        nTargetRemained = objTarget.Length;
        nAAGunRemained = objAAGun.Length;
    }
	
	void Update ()
    {
        CheckTargetDestroyed();
        CheckAAGunDestroyed();

        if (nTargetRemained == 0 && nAAGunRemained == 0)
        {
            StartCoroutine(BaseSecured());
            Destroy(objBlocking);
            bgmplayer.GoStealthBGM();
            (this as MonoBehaviour).enabled = false;
        }
    }

    void CheckTargetDestroyed()
    {
        for (int i = 0; i < objTarget.Length; i++)
        {
            if (objTarget[i] != null && objTarget[i].GetComponent<HAVIODurability>().nCurrDurability <= 0)
            {
                sfxPlayer.DestroyedConfirmed();
                nTargetRemained--;
                objTarget[i] = null;
            }
        }
    }
    void CheckAAGunDestroyed()
    {
        for (int i = 0; i < objAAGun.Length; i++)
        {
            if (objAAGun[i] != null && objAAGun[i].GetComponent<HAVIODurability>().nCurrDurability <= 0)
            {
                sfxPlayer.PlayDestroyedConfirmedForAAGun();
                nAAGunRemained--;
                objAAGun[i] = null;
            }
        }
    }

    IEnumerator BaseSecured()
    {
        yield return new WaitForSeconds(2.0f);

        sfxPlayer.GetComponent<AudioSource>().PlayOneShot(sfxSecured);
        yield return new WaitForSeconds(sfxSecured.length + 0.5f);

        if (bIsLastBase)
        {
            sfxPlayer.GetComponent<AudioSource>().PlayOneShot(sfxRTB);
            yield return new WaitForSeconds(sfxRTB.length);
        }
    }
}
