using UnityEngine;

public class Music : MonoBehaviour {
    [SerializeField] private AudioSource musicSource;
    static float currentMusicTime = 0.0f;

    public AudioClip background;


    private void Start() {
        musicSource.clip = background;

        musicSource.Play();
        musicSource.time = currentMusicTime;
    }

    void OnSceneLoaded() {
        currentMusicTime = musicSource.time;
    }
}
