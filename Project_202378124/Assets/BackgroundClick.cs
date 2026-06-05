using UnityEngine;



public class BackgroundClick : MonoBehaviour

{

    private RoomGameManager gameManager;



    void Start()

    {

        gameManager = Object.FindFirstObjectByType<RoomGameManager>();

    }



    // 배경 클릭 시 발동되는 원래 방식

    private void OnMouseDown()

    {

        if (gameManager != null)

        {

            // 마우스 현재 위치(화면 좌표)를 그대로 넘겨줌

            gameManager.BackgroundImageClick(Input.mousePosition);

        }

    }

}