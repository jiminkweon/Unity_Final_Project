using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSaver : MonoBehaviour
{
    // 각 방의 인스펙터 창에서 "Room1", "Room2", "Room3" 중 하나를 직접 적어줄 겁니다.
    public string respawnStageName = "Room1"; 

    void Start()
    {
        // 게임이 시작되면 유니티 시스템에 "이 계열의 시작점은 여기야"라고 저장합니다.
        PlayerPrefs.SetString("LastCheckpoint", respawnStageName);
        PlayerPrefs.Save(); // 데이터 강제 저장
        
        Debug.Log("현재 스테이지 세이브 완료! 리트라이 지점: " + respawnStageName);
    }
}