using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    public PlayerController player;

    public void FireBullet()
    {
        if (player != null)
            player.FireBullet();
    }
}
