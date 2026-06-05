using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class RoomGameManager : MonoBehaviour
{
    [Header("카메라 설정")]
    public Camera uiCamera;

    [Header("타이머 설정")]
    public float limitTime = 60f;
    public TextMeshProUGUI timerText;

    [Header("성공 / 실패 시 이동할 씬 이름")]
    public string successSceneName = "Room2";
    public string failSceneName = "GameOverScene";

    [Header("틀린 개수 설정")]
    public int totalAnomalies = 3;
    private int foundCount = 0;

    [Header("UI 연출 오브젝트")]
    public RectTransform circleUI;
    public RectTransform crossUI;

    [Header("효과음 사운드 설정")]
    public AudioClip correctSFX;
    public AudioClip wrongSFX;
    
    [Header("남은 시간별 경고음 (3초/2초/1초)")]
    public AudioClip warningThreeSecSFX;  // ⏰ 3초 남았을 때 소리
    public AudioClip warningTwoSecSFX;    // ⏰ 2초 남았을 때 소리
    public AudioClip warningOneSecSFX;    // ⏰ 1초 남았을 때 소리

    [Header("성공 클리어 효과음")]
    public AudioClip successClearSFX;      // 🏆 [추가] 방을 클리어했을 때 오프닝/다음 씬 가기 전 터질 효과음!
    
    private AudioSource audioSource;

    private bool isGameOver = false;
    private bool isSuccessDelayRunning = false;

    // 🔒 [추가] 정답 클릭 시 뒤에 깔린 배경까지 동시에 클릭되어 엑스가 같이 뜨는 걸 막는 방패
    private bool isCorrectProcessedThisFrame = false;

    // 각 초마다 소리가 중복해서 마구 터지지 않게 체크해주는 안전장치들
    private bool played3Sec = false;
    private bool played2Sec = false;
    private bool played1Sec = false;

    private HashSet<GameObject> foundObjects = new HashSet<GameObject>();

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        PrepareUI(circleUI);
        PrepareUI(crossUI);

        if (circleUI != null)
            circleUI.gameObject.SetActive(false);

        if (crossUI != null)
            crossUI.gameObject.SetActive(false);

        if (uiCamera == null)
            uiCamera = Camera.main;
    }

    void Update()
    {
        if (isGameOver)
            return;

        limitTime -= Time.deltaTime;

        if (timerText != null)
        {
            int time = Mathf.CeilToInt(Mathf.Max(0, limitTime));
            timerText.text = time.ToString();

            // 3초 이하일 때 텍스트 색상을 빨간색으로 변경
            if (time <= 3)
            {
                timerText.color = Color.red;

                // 🎵 정교하게 남은 시간을 판단하여 3초, 2초, 1초에 각각 한 번씩만 재생
                if (time == 3 && !played3Sec)
                {
                    played3Sec = true;
                    PlayWarningSound(warningThreeSecSFX);
                }
                else if (time == 2 && !played2Sec)
                {
                    played2Sec = true;
                    PlayWarningSound(warningTwoSecSFX);
                }
                else if (time == 1 && !played1Sec)
                {
                    played1Sec = true;
                    PlayWarningSound(warningOneSecSFX);
                }
            }
            else
            {
                timerText.color = Color.white;
            }
        }

        if (limitTime <= 0f)
            GameResult(false);
    }

    // 🔒 [추가] 매 프레임이 끝나는 시점에 정답 방패를 해제해 줍니다.
    void LateUpdate()
    {
        isCorrectProcessedThisFrame = false;
    }

    // 소리를 중복 없이 안전하게 재생해주는 헬퍼 함수
    void PlayWarningSound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void CorrectClick(GameObject clickedObject, Vector3 mousePosition)
    {
        if (isGameOver || isSuccessDelayRunning)
            return;

        if (foundObjects.Contains(clickedObject))
        {
            Debug.Log("이미 찾은 틀린그림입니다.");
            return;
        }

        // 🔒 정답이 감지되었으므로 방패 활성화 (배경 클릭 무시용)
        isCorrectProcessedThisFrame = true;

        foundObjects.Add(clickedObject);
        foundCount++;

        Debug.Log("정답! 찾은 개수: " + foundCount + "/" + totalAnomalies);

        if (correctSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(correctSFX);
        }

        ShowCircle(mousePosition);

        if (foundCount >= totalAnomalies)
            StartCoroutine(SuccessDelay());
    }

    public void WrongClick(Vector3 mousePosition)
    {
        if (isGameOver)
            return;

        // 🔒 [추가] 만약 정답 처리가 이미 일어난 프레임이라면 엑스 창(오답) 처리를 강제로 스킵합니다!
        if (isCorrectProcessedThisFrame)
            return;

        Debug.Log("오답!");

        if (wrongSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(wrongSFX);
        }

        ShowCross(mousePosition);
    }

    public void BackgroundImageClick(Vector3 mousePosition)
    {
        WrongClick(mousePosition);
    }

    public void WrongObjectClick(Vector3 mousePosition)
    {
        WrongClick(mousePosition);
    }

    void PrepareUI(RectTransform ui)
    {
        if (ui == null)
            return;

        ui.SetAsLastSibling();

        ui.anchorMin = new Vector2(0.5f, 0.5f);
        ui.anchorMax = new Vector2(0.5f, 0.5f);
        ui.pivot = new Vector2(0.5f, 0.5f);
        ui.sizeDelta = new Vector2(140f, 140f);
        ui.localScale = Vector3.one;

        Image img = ui.GetComponent<Image>();
        if (img != null)
            img.color = Color.white;
    }

    void SetUIPosition(RectTransform ui, Vector3 screenPosition)
    {
        RectTransform canvasRect = ui.parent as RectTransform;
        if (canvasRect == null)
            return;

        Canvas canvas = canvasRect.GetComponent<Canvas>();

        Camera cam = null;
        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            cam = uiCamera;

        Vector2 localPos;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPosition,
            cam,
            out localPos))
        {
            ui.anchoredPosition = localPos;
        }
    }

    void ShowCircle(Vector3 mousePosition)
    {
        if (circleUI == null)
        {
            Debug.Log("Circle UI가 연결 안 됨");
            return;
        }

        RectTransform newCircle = Instantiate(circleUI, circleUI.parent);
        PrepareUI(newCircle);
        SetUIPosition(newCircle, mousePosition);
        newCircle.gameObject.SetActive(true);

        Debug.Log("동그라미 생성됨");
    }

    void ShowCross(Vector3 mousePosition)
    {
        if (crossUI == null)
        {
            Debug.Log("Cross UI가 연결 안 됨");
            return;
        }

        PrepareUI(crossUI);
        SetUIPosition(crossUI, mousePosition);
        crossUI.gameObject.SetActive(true);

        StartCoroutine(HideCross());
    }

    IEnumerator HideCross()
    {
        yield return new WaitForSeconds(0.5f);

        if (crossUI != null)
            crossUI.gameObject.SetActive(false);
    }

    IEnumerator SuccessDelay()
    {
        isSuccessDelayRunning = true;

        // 🎵 [추가] 다음 씬으로 부드럽게 넘어가기 전, 클리어 성공 효과음을 우렁차게 재생합니다.
        if (successClearSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(successClearSFX);
        }

        // 효과음이 다 들릴 수 있도록 0.8초 동안 대기 여운을 줍니다.
        yield return new WaitForSeconds(0.8f);

        GameResult(true);
    }

    void GameResult(bool isSuccess)
    {
        if (isGameOver)
            return;

        isGameOver = true;

        if (isSuccess)
        {
            Debug.Log("성공! 다음 방으로 이동합니다.");
            SceneManager.LoadScene(successSceneName);
        }
        else
        {
            Debug.Log("실패! 게임 오버.");
            SceneManager.LoadScene(failSceneName);
        }
    }
}