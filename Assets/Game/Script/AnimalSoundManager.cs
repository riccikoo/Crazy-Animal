using UnityEngine;

public class AnimalSoundManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] animalSounds;

    public void PlayAnimalSound(int index)
    {
        if (index < 0 || index >= animalSounds.Length) return;
        if (animalSounds[index] == null) return;

        audioSource.Stop();
        audioSource.PlayOneShot(animalSounds[index]);
    }
}