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
        GameManager.Instance.OnSelection += (arg, arg2) => audioSource.PlayOneShot(arg ? click : deselect);
        GameManager.Instance.OnFindMatch += arg => audioSource.PlayOneShot(arg ? match : noMatch);
        BoardManager.Instance.OnGemCreation += () => PlayRandomPitch(pop);
        AnimationManager.Instance.OnGemFall += () => PlayRandomPitch(whoosh);
        AnimationManager.Instance.OnDeleteMatches += () => PlayRandomPitch(pop);
    }
    private void PlayRandomPitch(AudioClip audioClip){
        audioSource.pitch = Random.Range(0.8f, 1.1f);
        audioSource.PlayOneShot(audioClip);
        audioSource.pitch = 1;
    }
}