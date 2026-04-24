using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;


    [SerializeField] private TextMeshProUGUI nameBox;
    [SerializeField] private TextMeshProUGUI textBox;
    [SerializeField] private GameObject dialogueObj;
    [SerializeField] private GameObject blinkerBtn;

    [SerializeField] private DialogueEntry[] startDialogues;

    private int dialogueIdx = 0;

    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private bool isTyping = false;
    [SerializeField] private Coroutine playingCoroutine;

    [SerializeField] private Color textEnemyColor;
    Color textOriginColor;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        textOriginColor = textBox.color;
        StartDialogue();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            if (isTyping)
            {
                StopCoroutine(playingCoroutine);
                ShowFullText();
            }
            else
            {
                NextDialogue();
            }
        }


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopCoroutine(playingCoroutine);
            dialogueIdx = startDialogues.Length - 1;
            NextDialogue();
        }

    }

    private void NextDialogue()
    {
        dialogueIdx++;

        if (dialogueIdx < startDialogues.Length)
        {
            SoundManager.Instance.StopSFX();
            blinkerBtn.SetActive(false);
            StartDialogue();
        }
        else
        {
            dialogueObj.SetActive(false);
            GameSceneManager.Instance.LoadSceneByName("Game");
        }
    }
    
    

    public void StartDialogue()
    {

        playingCoroutine = StartCoroutine(TypingDialogue(startDialogues[dialogueIdx]));
    }

    IEnumerator TypingDialogue(DialogueEntry entry)
    {
        if (entry.soundType != SoundType.None)
        {
            SoundManager.Instance.PlaySFX(entry.soundType);
        }

        
        if(entry.speakerType == SpeakerType.Enemy)
        {
            textBox.color = textEnemyColor;
        }
        else
        {
            textBox.color = textOriginColor;
        }
        
        isTyping = true;
        textBox.text = "";
        nameBox.text = entry.speakerName;

        foreach (char c in entry.dialogueText)
        {
            textBox.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        blinkerBtn.SetActive(true);
    }
    
    public void ShowFullText()
    {
        textBox.text = startDialogues[dialogueIdx].dialogueText;
        isTyping = false;
        blinkerBtn.SetActive(true);
    }
}
