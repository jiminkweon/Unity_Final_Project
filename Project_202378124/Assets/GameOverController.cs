using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverController : MonoBehaviour
{
    [Header("숨쉬는 효과 설정")]
    public float breatheSpeed = 1.5f;     
    public float breatheAmount = 0.03f;   

    [Header("클릭 효과 설정")]
    public float clickScaleAmount = 0.9f; 
    public float clickDuration = 0.1f;    

    private GameObject[] buttons;
    private Vector3[] initialScales;      
    private bool[] isClicking;            

    void Start()
    {
        buttons = new GameObject[2];
        buttons[0] = GameObject.Find("g1_0"); // 리트라이 버튼
        buttons[1] = GameObject.Find("g2_0"); // 메인메뉴 버튼

        initialScales = new Vector3[2];
        isClicking = new bool[2];

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null)
            {
                initialScales[i] = buttons[i].transform.localScale;
                isClicking[i] = false;
            }
        }
    }

    void Update()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null && !isClicking[i])
            {
                float wave = Mathf.PingPong(Time.time * breatheSpeed + (i * 0.5f), 1f);
                float breatheFactor = Mathf.Pow(wave, 2); 
                float currentScaleChange = 1f + (breatheFactor * breatheAmount);
                buttons[i].transform.localScale = initialScales[i] * currentScaleChange;
            }
        }
    }

    // --- 리트라이 버튼 클릭 ---
    public void OnRetryClick()
    {
        string targetScene = "Room1"; // 기본 사태 대비 백업용

        // StageManager가 가공해서 기억해둔 씬(Room1, Room2, Room3)을 가져옴
        if (StageManager.Instance != null)
        {
            targetScene = StageManager.Instance.lastPlayedScene;
        }

        StartCoroutine(ClickEffectRoutine(0, targetScene));
    }

    // --- 메인 메뉴 버튼 클릭 (무조건 오프닝으로) ---
    public void OnMainMenuClick()
    {
        StartCoroutine(ClickEffectRoutine(1, "OpeningScene"));
    }

    IEnumerator ClickEffectRoutine(int index, string sceneName)
    {
        GameObject targetButton = buttons[index];
        if (targetButton == null) yield break;

        isClicking[index] = true; 

        Vector3 currentScale = targetButton.transform.localScale;
        Vector3 targetScale = initialScales[index] * clickScaleAmount;

        float elapsedTime = 0f;
        while (elapsedTime < clickDuration)
        {
            targetButton.transform.localScale = Vector3.Lerp(currentScale, targetScale, elapsedTime / clickDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        targetButton.transform.localScale = targetScale;

        elapsedTime = 0f;
        while (elapsedTime < clickDuration)
        {
            targetButton.transform.localScale = Vector3.Lerp(targetScale, initialScales[index], elapsedTime / clickDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        targetButton.transform.localScale = initialScales[index];

        isClicking[index] = false; 

        SceneManager.LoadScene(sceneName);
    }
}