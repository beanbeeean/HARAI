using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EndingManager : MonoBehaviour
{
    [SerializeField] private Image logo;

    void Start()
    {
        EndingScenario();
    }

    void EndingScenario()
    {

        StartCoroutine(EndingRoutine());
        
    }
    
    IEnumerator EndingRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        SoundManager.Instance.PlaySFXEnding(SoundType.EnemyLaugh_3, 0.5f);
        yield return new WaitForSeconds(2.6f);
        SoundManager.Instance.PlaySFXEnding(SoundType.Flashlight, 1.0f);
        logo.enabled = false;

        yield return new WaitForSeconds(2f);

        GameSceneManager.Instance.LoadSceneByName("Title");
    }
}
