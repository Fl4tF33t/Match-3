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

    private void Start() {
        GameManager.Instance.OnSelection += arg => audioSource.PlayOneShot(arg ? click : deselect);
        GameManager.Instance.OnFindMatch += arg => audioSource.PlayOneShot(arg ? match : noMatch);
        BoardManager.Instance.OnGemCreation += () => PlayRandomPitch(pop);
        AnimationManager.Instance.OnGemFall += () => PlayRandomPitch(whoosh);
        AnimationManager.Instance.OnDeleteMatches += () => PlayRandomPitch(pop);
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