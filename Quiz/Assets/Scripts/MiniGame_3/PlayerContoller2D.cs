using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContoller2D : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float smoothing = 0.15f;

    Rigidbody2D rb;
    Vector2 velocitySmooth;

    bool isDragging = false;
    float dragOffsetX;

    float lastX;              
    Animator anim;
    Camera cam;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        anim = GetComponent<Animator>();
        lastX = transform.position.x;
    }

    void Update()
    {
        if (GameManagerMiniGame3.Instance.IsGameOver || !GameManagerMiniGame3.Instance.isGameStart || GameManagerMiniGame3.Instance.isGameWin)
            return;

        HandleTouchOrMouse();
    }

    void HandleTouchOrMouse()
    {
        // TOUCH
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            Vector3 worldPos = cam.ScreenToWorldPoint(t.position);
            worldPos.z = 0;

            if (t.phase == TouchPhase.Began)
            {
                if (IsTouchingPlayer(worldPos))
                {
                    isDragging = true;
                    dragOffsetX = worldPos.x - transform.position.x;
                    lastX = transform.position.x;
                }
            }
            else if (t.phase == TouchPhase.Moved && isDragging)
            {
                float targetX = worldPos.x - dragOffsetX;
                MovePlayer(targetX);
            }
            else if (t.phase == TouchPhase.Ended)
            {
                isDragging = false;
                anim.SetBool("Run", false);
            }

            return;
        }

        // MOUSE
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0;

            if (IsTouchingPlayer(worldPos))
            {
                isDragging = true;
                dragOffsetX = worldPos.x - transform.position.x;
                lastX = transform.position.x;
            }
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector3 worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0;

            float targetX = worldPos.x - dragOffsetX;
            MovePlayer(targetX);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            anim.SetBool("Run", false);
        }
    }

    bool IsTouchingPlayer(Vector2 point)
    {
        return GetComponent<Collider2D>() == Physics2D.OverlapPoint(point);
    }

    void MovePlayer(float targetX)
    {
        float newX = Mathf.Lerp(rb.position.x, targetX, smoothing);
        rb.MovePosition(new Vector2(newX, rb.position.y));

        if (Mathf.Abs(newX - lastX) > 0.01f)
        {
            anim.SetBool("Run", true);

            // ËÁØ¹«éÒÂ/¢ÇÒ
            if (targetX > transform.position.x)
                transform.rotation = Quaternion.Euler(0, 0, 0); // ¢ÇÒ
            else
                transform.rotation = Quaternion.Euler(0, 180, 0); // «éÒÂ
        }
        else
        {
            anim.SetBool("Run", false);
        }

        lastX = newX;
    }
}
