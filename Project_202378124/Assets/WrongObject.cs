using UnityEngine;

public class WrongObject : MonoBehaviour
{
    private RoomGameManager gameManager;
    private Camera mainCamera;

    void Start()
    {
        gameManager = Object.FindFirstObjectByType<RoomGameManager>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePoint = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

            Collider2D hit = Physics2D.OverlapPoint(mousePoint);

            if (hit != null && hit.gameObject == gameObject)
            {
                Debug.Log("틀린그림 클릭됨: " + gameObject.name);

                if (gameManager != null)
                    gameManager.CorrectClick(gameObject, Input.mousePosition);
            }
        }
    }
}