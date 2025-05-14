using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour{

    [SerializeField] private AudioClip click;
    [SerializeField] private AudioClip deselect;
    [SerializeField] private AudioClip match;
    [SerializeField] private AudioClip noMatch;
    [SerializeField] private AudioClip whoosh;
    [SerializeField] private AudioClip pop;

    private AudioSource audioSource;

    private void Awake(){
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayClick() => audioSource.PlayOneShot(click);
    public void PlayDeselect() => audioSource.PlayOneShot(deselect);
    public void PlayMatch() => audioSource.PlayOneShot(match);
    public void PlayNoMatch() => audioSource.PlayOneShot(noMatch);
    public void PlayPop() => PlayRandomPitch(pop);
    public void PlayWhoosh() => PlayRandomPitch(whoosh);

    private void PlayRandomPitch(AudioClip audioClip){
        audioSource.pitch = Random.Range(0.8f, 1.1f);
        audioSource.PlayOneShot(audioClip);
        audioSource.pitch = 1;
    }
}