using UnityEngine;
using UnityEngine.Audio;

public class GameAudioManager : MonoBehaviour
{
    public static GameAudioManager Instance { get; private set; }
    public AudioMixer mixer;
    public AudioSource backgroundMusic;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Lấy giá trị âm lượng đã lưu từ menu chính
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", -40);
        mixer.SetFloat("volume", savedVolume);
        
        // Đảm bảo nhạc nền được phát qua mixer
        if (backgroundMusic != null && mixer != null)
        {
            backgroundMusic.outputAudioMixerGroup = mixer.FindMatchingGroups("Master")[0];
        }
    }

    // Hàm static để phát âm thanh thông qua AudioMixer
    public static void PlaySoundThroughMixer(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (Instance == null || Instance.mixer == null || clip == null)
        {
            Debug.LogWarning("GameAudioManager không được thiết lập đúng.");
            return;
        }
        
        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.position = position;

        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.clip = clip;
        
        // Kết nối với AudioMixer
        aSource.outputAudioMixerGroup = Instance.mixer.FindMatchingGroups("Master")[0];
        
        aSource.spatialBlend = 0f; // 0 = 2D sound
        aSource.Play();

        Destroy(tempGO, clip.length);
    }
}