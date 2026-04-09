using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private PlayerInputReader inputReader;
    private PlayerMove2D playerMove;
    [SerializeField] private FlashlightAnimator flashlightAnimator;


    private readonly int hashIsMoving = Animator.StringToHash("IsMoving");
    private readonly int hashDirX = Animator.StringToHash("DirX");
    private readonly int hashDirY = Animator.StringToHash("DirY");



    private void Awake()
    {
        animator = GetComponent<Animator>();
        inputReader = GetComponent<PlayerInputReader>();
        playerMove = GetComponent<PlayerMove2D>();
        //flashlightAnimator = GetComponentInChildren<FlashlightAnimator>();
    }

    private void Update()
    {
        animator.SetBool(hashIsMoving, inputReader.isMoving);

        //if (inputReader.isMoving)
        //{

        //}

        Vector2 dirVec = Direction8.ToVector2(playerMove.CurrentDirection);

        animator.SetFloat(hashDirX, dirVec.x);
        animator.SetFloat(hashDirY, dirVec.y);
        if(dirVec != Vector2.zero)
        {
            //flashlightAnimator.UpdateFlashlight(dirVec.x, dirVec.y);

        }
    }
}