using System.Collections;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]

public class PlayerMove : MonoBehaviour
{
    private StatusContainer statusContainer;
    private float speed;

    private Vector2 moveInput;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private BoxCollider2D bc;
    private bool escaping = false;

    // ���͒l��n��
    public void SetMoveInput(Vector2 input) => moveInput = input;

    void Start()
    {
        statusContainer = GetComponent<StatusContainer>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();
    }

    // Update����Ă�
    public void TickMove()
    {
        Vector3 movement = new Vector3(moveInput.x, moveInput.y, 0f);
        speed = statusContainer.GetStatus().Calculate(StatusCategory.Speed);
        transform.position += movement * speed * Time.deltaTime;
    }

    public void GhostEscape(Vector3 direction)
    {
        if (escaping) return;
        StartCoroutine(GhostEscapeCoroutine(direction));
    }

    private IEnumerator GhostEscapeCoroutine(Vector3 direction)
    {
        escaping = true;

        // ちょっと透明にする
        Color currentColor = sr.color;
        currentColor.a = 0.5f;
        sr.color = currentColor;
        // 決められた方向に突進
        rb.linearVelocity = -direction * 30.0f;
        // 敵は透過するようにする
        bc.isTrigger = true;

        // 数秒間待つ
        yield return new WaitForSeconds(2.0f);

        // 透明化を解除する
        currentColor.a = 1.0f;
        sr.color = currentColor;
        // 透過しないようにする
        bc.isTrigger = false;

        escaping = false;
    }
}
