using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    public PlayerController player;  // assign this in Inspector

    // This gets called by the animation event
    public void FireBullet()
    {
        if (player != null)
            player.FireBullet();
    }
}
