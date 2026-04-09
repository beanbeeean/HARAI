using UnityEngine;

public class FlashlightAnimator : MonoBehaviour
{

    private Animator anim;
    private PlayerMove2D playerMove2D;

    private readonly int hashDirX = Animator.StringToHash("DirX");
    private readonly int hashDirY = Animator.StringToHash("DirY");

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMove2D = GetComponentInParent<PlayerMove2D>();
    }

    private void Update()
    {
        if (playerMove2D == null) return;

        Vector2 dirVec = Direction8.ToVector2(playerMove2D.CurrentDirection);

        anim.SetFloat(hashDirX, dirVec.x);
        anim.SetFloat(hashDirY, dirVec.y);
    }
    //public void UpdateFlashlight(float x, float y)
    //{
    //    animator.SetFloat(hashDirX, x);
    //    animator.SetFloat(hashDirY, y);
    //}
}
