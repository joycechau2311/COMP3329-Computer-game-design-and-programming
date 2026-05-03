using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;


    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        transform.Translate(Vector2.right * h * moveSpeed * Time.deltaTime);

        if (transform.position.x < -8f)
        {
            transform.position = new Vector3(-8f, transform.position.y, transform.position.z);
        }
    }
}