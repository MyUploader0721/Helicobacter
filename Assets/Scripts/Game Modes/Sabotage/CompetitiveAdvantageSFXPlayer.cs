using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompetitiveAdvantageSFXPlayer : MonoBehaviour
{
    AudioSource audioSource;

    [Header("Something Destroyed")]
    [SerializeField] AudioClip[] sfxDestroyedConfirmed;
    [SerializeField] AudioClip[] sfxEliminated;
    [SerializeField] AudioClip[] sfxEnemyNeutralized;
    [SerializeField] AudioClip[] sfxAAGunDestroyed;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {

    }

    public void DestroyedConfirmed()
    {
        audioSource.PlayOneShot(sfxDestroyedConfirmed[Random.Range(0, sfxDestroyedConfirmed.Length)]);
    }
    public void Eliminated()
    {
        audioSource.PlayOneShot(sfxEliminated[Random.Range(0, sfxEliminated.Length)]);
    }
    public void EnemyNeutralized()
    {
        audioSource.PlayOneShot(sfxEnemyNeutralized[Random.Range(0, sfxEnemyNeutralized.Length)]);
    }
    public void AAGunDestroyed()
    {
        audioSource.PlayOneShot(sfxAAGunDestroyed[Random.Range(0, sfxAAGunDestroyed.Length)]);
    }
    public void PlayDestroyedConfirmedForAAGun()
    {
        switch (Random.Range(0, 4))
        {
            case 0: DestroyedConfirmed(); break;
            case 1: Eliminated(); break;
            case 2: EnemyNeutralized();  break;
            case 3: AAGunDestroyed(); break;
        }
    }
}
