using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSaver : MonoBehaviour
{
    // 각 방의 인스펙터 창에서 "Room1", "Room2", "Room3" 중 하나
    public string respawnStageName = "Room1"; 

    void Start()
    {
        PlayerPrefs.SetString("LastCheckpoint", respawnStageName);
        PlayerPrefs.Save(); // 데이터 강제 저장
        
        Debug.Log("현재 스테이지 세이브 완료! 리트라이 지점: " + respawnStageName);
    }
}