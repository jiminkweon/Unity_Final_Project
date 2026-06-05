using UnityEngine;
using TMPro; // TextMeshPro를 제어
using UnityEngine.SceneManagement; // 씬 전환

public class RoomTimer : MonoBehaviour
{
    [Header("설정")]
    public float limitTime = 10f; // 제한시간 (10초)
    public string nextSceneName = "Room1-2"; // 시간이 다 되면 이동할 씬 이름

    [Header("연결할 UI")]
    public TextMeshProUGUI timerText; // 화면에 보여줄 텍스트 오브젝트

    private bool isTimeOver = false;

    void Update()
    {
        // 이미 시간이 다 끝났다면 아래 코드를 실행하지 않음
        if (isTimeOver) return;

        // 매 프레임마다 제한시간을 감소시킴 (deltaTime 활용)
        limitTime -= Time.deltaTime;

        // 시간이 감소할 때 소수점을 버리고 정수(0, 1, 2...) 형태로만 텍스트에 표시
        if (timerText != null)
        {
            // Mathf.Max(0, ...)를 써서 시간이 0 이하의 마이너스로 내려가 표시되지 않게 막음
            timerText.text = Mathf.CeilToInt(Mathf.Max(0, limitTime)).ToString();
        }

        // 시간이 0초 이하가 되면 다음 씬으로 전환
        if (limitTime <= 0f)
        {
            isTimeOver = true;
            TimeOverAndNextRoom();
        }
    }

    // 씬을 전환하는 함수
    void TimeOverAndNextRoom()
    {
        Debug.Log("10초가 지났습니다! 다음 방으로 이동합니다.");
        SceneManager.LoadScene(nextSceneName);
    }
}