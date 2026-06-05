using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class OpeningController : MonoBehaviour
{
    [Header("움직이는 효과 설정")]
    public float breatheSpeed = 1.0f;     
    public float breatheAmount = 0.03f;   

    [Header("클릭 효과 설정")]
    public float clickScaleAmount = 0.9f; // 클릭했을 때 줄어들 크기 비율
    public float clickDuration = 0.1f;    // 밟히는 연출 시간

    private GameObject[] buttons;
    private Vector3[] initialScales;     
    private bool[] isClicking;            

    void Start()
    {
        buttons = new GameObject[3];
        buttons[0] = GameObject.Find("easy_0");
        buttons[1] = GameObject.Find("normal_0");
        buttons[2] = GameObject.Find("hard_0");

        initialScales = new Vector3[3];
        isClicking = new bool[3];

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
            // 클릭 연출 중이 아닐 때만 효과 적용
            if (buttons[i] != null && !isClicking[i])
            {
                float wave = Mathf.PingPong(Time.time * breatheSpeed + (i * 0.5f), 1f);
                
                float breatheFactor = Mathf.Pow(wave, 2); 

                float currentScaleChange = 1f + (breatheFactor * breatheAmount);
                buttons[i].transform.localScale = initialScales[i] * currentScaleChange;
            }
        }
    }

    // --- 클릭 시 호출할 함수들 ---

    public void OnEasyModeClick()
    {
        StartCoroutine(ClickEffectRoutine(0, "Room1"));
    }

    public void OnNormalModeClick()
    {
        StartCoroutine(ClickEffectRoutine(1, "Room2"));
    }

    public void OnHardModeClick()
    {
        StartCoroutine(ClickEffectRoutine(2, "Room3"));
    }

    // --- 클릭 연출 코루틴 (인덱스를 받아 처리) ---
    IEnumerator ClickEffectRoutine(int index, string sceneName)
    {
        GameObject targetButton = buttons[index];
        if (targetButton == null) yield break;

        isClicking[index] = true; 

        Vector3 currentScale = targetButton.transform.localScale;
        Vector3 targetScale = initialScales[index] * clickScaleAmount;

        // 1. 꾹 눌리는 느낌 (원래 크기 기준으로 작아짐)
        float elapsedTime = 0f;
        while (elapsedTime < clickDuration)
        {
            targetButton.transform.localScale = Vector3.Lerp(currentScale, targetScale, elapsedTime / clickDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        targetButton.transform.localScale = targetScale;

        // 2. 튀어나오는 느낌
        elapsedTime = 0f;
        while (elapsedTime < clickDuration)
        {
            targetButton.transform.localScale = Vector3.Lerp(targetScale, initialScales[index], elapsedTime / clickDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        targetButton.transform.localScale = initialScales[index];

        isClicking[index] = false; 

        // 다음 방으로 이동
        SceneManager.LoadScene(sceneName);
    }
}