using UnityEngine;

public class LeftAirWall : MonoBehaviour
{
    [Header("References")]
    public Transform playerTransform; 
    public float disableThresholdX = 5f; 

    private Collider2D _airWallCollider;

    private void Start()
    {
        _airWallCollider = GetComponent<Collider2D>();


        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    private void Update()
    {

        if (playerTransform != null && playerTransform.position.x > disableThresholdX)
        {

            _airWallCollider.enabled = false;
        }
    }
}