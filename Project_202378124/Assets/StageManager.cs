using UnityEngine;

using UnityEngine.SceneManagement;



public class StageManager : MonoBehaviour

{

    public static StageManager Instance { get; private set; }



    // 최종적으로 리트라이할 때 돌아갈 메인 스테이지 이름

    public string lastPlayedScene { get; private set; } = "Room1"; 



    void Awake()

    {

        if (Instance == null)

        {

            Instance = this;

            DontDestroyOnLoad(gameObject);

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

        string currentSceneName = scene.name;



        // 게임오버나 오프닝 씬은 저장에서 제외

        if (currentSceneName == "GameOverScene" || currentSceneName == "OpeningScene")

        {

            return;

        }



        // [핵심 로직] 씬 이름에 특정 단어가 포함되어 있는지 검사합니다.

        if (currentSceneName.Contains("Room1"))

        {

            lastPlayedScene = "Room1"; // Room1-1, Room1-2 등은 모두 Room1으로 통일

        }

        else if (currentSceneName.Contains("Room2"))

        {

            lastPlayedScene = "Room2"; // Room2-2 등은 모두 Room2로 통일

        }

        else if (currentSceneName.Contains("Room3"))

        {

            lastPlayedScene = "Room3"; // Room3-2 등은 모두 Room3로 통일

        }

        else

        {

            // 혹시나 다른 이름의 스테이지가 있다면 그 이름 그대로 저장

            lastPlayedScene = currentSceneName;

        }



        Debug.Log("세이브 포인트 지정 완료! 리트라이 시 이동할 곳: " + lastPlayedScene);

    }

}