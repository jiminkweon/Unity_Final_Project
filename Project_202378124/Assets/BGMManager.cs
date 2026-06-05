using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance { get; private set; }

    [Header("각 스테이지별 음악 등록")]
    public AudioClip openingAndGameOverBGM;
    public AudioClip room1BGM;
    public AudioClip room2BGM;
    public AudioClip room3BGM;

    [Header("씬 진입 효과음")]
    public AudioClip gameOverSFX;
    public AudioClip gameClearSFX;

    private AudioSource bgmSource;
    private AudioSource sfxSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            bgmSource = GetComponent<AudioSource>();

            if (bgmSource == null)
                bgmSource = gameObject.AddComponent<AudioSource>();

            bgmSource.loop = true;
            bgmSource.playOnAwake = false;

            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;
        AudioClip targetBGM = null;

        if (sceneName == "OpeningScene" ||
            sceneName == "GameOverScene" ||
            sceneName == "GameClear")
        {
            targetBGM = openingAndGameOverBGM;
        }
        else if (sceneName == "Room1" || sceneName == "Room1-2")
        {
            targetBGM = room1BGM;
        }
        else if (sceneName == "Room2" || sceneName == "Room2-2")
        {
            targetBGM = room2BGM;
        }
        else if (sceneName == "Room3" || sceneName == "Room3-2")
        {
            targetBGM = room3BGM;
        }

        if (targetBGM != null)
        {
            if (bgmSource.clip != targetBGM)
            {
                bgmSource.clip = targetBGM;
                bgmSource.Play();
            }
        }
        else
        {
            bgmSource.Stop();
            bgmSource.clip = null;
        }

        if (sceneName == "GameOverScene" && gameOverSFX != null)
        {
            sfxSource.PlayOneShot(gameOverSFX);
        }

        if (sceneName == "GameClear" && gameClearSFX != null)
        {
            sfxSource.PlayOneShot(gameClearSFX);
        }
    }

    
}