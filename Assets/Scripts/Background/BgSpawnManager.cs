using UnityEngine;

public class BgSpawnManager : MonoBehaviour
{
    [Header("Boss Fight Settings")]
    public float bossTime = 300f;
    public float playerSpeed = 5f;
    public float bgLength = 19.2f;
    public Transform bgParent;

    void Start()
    {
        float totalDistance = playerSpeed * bossTime;
        int requiredBgCount = Mathf.CeilToInt(totalDistance / bgLength);

        if (bgParent == null)
        {
            // 关键修复：给Debug加完整命名空间
            UnityEngine.Debug.LogError("Assign bgParent!");
            return;
        }

        Transform bgTemplate = bgParent.GetChild(0);
        // 若有地面模板，按实际层级修改索引
        Transform groundTemplate = bgParent.GetChild(2);

        int existingGroups = bgParent.childCount / 2;
        if (existingGroups < requiredBgCount)
        {
            AddMissingBackgrounds(existingGroups, requiredBgCount, bgTemplate, groundTemplate);
        }
    }

    void AddMissingBackgrounds(int startIndex, int endIndex, Transform bgTemplate, Transform groundTemplate)
    {
        for (int i = startIndex; i < endIndex; i++)
        {
            Transform newBg = Instantiate(bgTemplate, bgParent);
            newBg.localPosition = new Vector3(i * bgLength, 0, 0);

            Transform newGround = Instantiate(groundTemplate, bgParent);
            newGround.localPosition = new Vector3(i * bgLength, -5.5f, 0);

            if (i % 2 == 1)
            {
                SpriteRenderer bgSr = newBg.GetComponent<SpriteRenderer>();
                SpriteRenderer groundSr = newGround.GetComponent<SpriteRenderer>();
                if (bgSr != null) bgSr.flipX = true;
                if (groundSr != null) groundSr.flipX = true;
            }
        }
    }
}