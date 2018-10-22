using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyRadioComBehaviour : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] sfxRadioCom;

    IEnumerator Start()
    {
        int nComm;

        while (true)
        {
            nComm = Random.Range(0, sfxRadioCom.Length);

            audioSource.PlayOneShot(sfxRadioCom[nComm]);
            yield return new WaitForSeconds(sfxRadioCom[nComm].length - 0.05f);
        }
    }
}
