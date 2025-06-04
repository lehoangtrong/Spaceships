using UnityEngine;
using UnityEngine.Audio;

public class GameAudioManager : MonoBehaviour
{
    public AudioMixer mixer;
    public AudioSource backgroundMusic;

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
}