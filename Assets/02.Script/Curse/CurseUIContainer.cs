using UnityEngine;

public class CurseUIContainer : MonoBehaviour
{
    [SerializeField] private PlayerInputReader playerInputReader;
    [SerializeField] private GameObject cursePanel;

    [Header("Status")]
    [SerializeField] private bool isActive = false;

    private void OnEnable()
    {
        if (playerInputReader != null)
        {
            playerInputReader.CurseListOpenEvent += ToggleCurseUI;
            playerInputReader.CurseListCloseEvent += ToggleCurseUI;
        }
    }

    private void OnDisable()
    {
        if (playerInputReader != null)
        {
            playerInputReader.CurseListOpenEvent -= ToggleCurseUI;
            playerInputReader.CurseListCloseEvent -= ToggleCurseUI;
        }
    }

    private void ToggleCurseUI()
    {
        Debug.Log("ToggleCurseUI");
        if (!isActive)
        {
            Debug.Log("TTTTT");
            cursePanel.SetActive(true);
            isActive = true;
            CurseUIController cuc = cursePanel.GetComponent<CurseUIController>();
            cuc.RefreshUI();

            Time.timeScale = 0f;
            playerInputReader.SetUIInput(true);
        }
        else
        {
            cursePanel.SetActive(false);
            isActive = false;
            Time.timeScale = 1f;
            playerInputReader.SetUIInput(false);
        }
    }
}
