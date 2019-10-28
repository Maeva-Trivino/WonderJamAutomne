using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Image curtain = null;
    [SerializeField]
    private Text nbSeedText = null,
        nbLevelText = null,
        nbTreeText = null,
        timerText = null;
    [SerializeField]
    private Image bucketImg = null;
    [SerializeField]
    private float limitTime = 180;
    [SerializeField]
    private Sprite emptyBucketSprite = null;
    [SerializeField]
    private Sprite filledBucketSprite = null;
    [SerializeField]
    private RectTransform endScreen;
    [SerializeField]
    private GameObject popup;

    //Sound of the theme
    [SerializeField]
    private AudioSource themeSound;
    //Sound of the gameOver
    [SerializeField]
    private AudioSource gameOverSound;

    private int nbSeeds = 10;
    private int nbActualTree;    
    private int nbGoalTree = 1;//
    private int level = 1;
    private float startTime;

    private static UIManager internalInstance;

    public static UIManager instance {
        get
        {
            if (internalInstance == null)
                return FindObjectOfType<UIManager>();
            else
                return internalInstance;
        }
    }

    private bool paused = false;

    // Start is called before the first frame update
    void Start()
    {
        internalInstance = this;
        startTime = Time.time;
        SetLevel(level, nbGoalTree);
        curtain.enabled = true;
        LeanTween.alpha((RectTransform)curtain.transform, 0f, .2f);
        bucketImg.color = new Color(1f, 1f, 1f, .5f);
        themeSound.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if(paused)
            return;
        float t = Time.time - startTime;
        float seconds = (limitTime - t);
        if(nbActualTree == nbGoalTree)
        {

            if(level == 1)
            {
                nbGoalTree+=4;
            }
            else
            {
                nbGoalTree += 5;
            }
            
            level++;
            if (nbGoalTree > 27)
            {
                nbGoalTree = 28;
            }
            SetLevel(level,nbGoalTree) ;
            SetTime(limitTime + limitTime-t);
        }
        
        if (seconds >= 100)
        {
            timerText.text = seconds.ToString("f0");
        }
        else if (seconds >= 10)
        {
            timerText.text = seconds.ToString("f1");
        }
        else if (seconds >= 0)
        {
            timerText.text = seconds.ToString("f2");
        }
        else
        {
            timerText.text = "OUT OF TIME";
            DisplayEndScreen();
        }

        var f2 = System.Array.FindAll(GameObject.FindGameObjectsWithTag("Tree"), o => !o.GetComponent<ForestTree>().IsSoilOrBurnt);
        if (f2.Length == 0 && nbSeeds == 0)
            DisplayEndScreen();
    }

    public void SetTime(float newTime)
    {
        startTime = Time.time;
        limitTime = newTime;
    }

    public void SetLevel(int newLevel, int newNbGoalTree)
    {
        level = newLevel;
        nbGoalTree = newNbGoalTree;
        nbLevelText.text = "Niv " + level + " :";
        nbTreeText.text = nbActualTree + "/" + nbGoalTree;
    }

    public void AddTree()
    {
        nbActualTree += 1;
        nbTreeText.text = nbActualTree + "/" + nbGoalTree;
    }
    public void DeleteTree()
    {
        nbActualTree -= 1;
        nbTreeText.text = nbActualTree + "/" + nbGoalTree;
    }

    public void UpdateSeeds(int nbSeed)
    {
        nbSeeds = nbSeed;
        nbSeedText.text = "x " + nbSeed;
    }

    public void EmptyBucket()
    {
        bucketImg.sprite = emptyBucketSprite;
        bucketImg.color = new Color(1f, 1f, 1f, 1f);
    }
    public void FilledBucket()
    {
        bucketImg.sprite = filledBucketSprite;
        bucketImg.color = new Color(1f, 1f, 1f, 1f);
    }

    public void DropBucket()
    {
        bucketImg.sprite = emptyBucketSprite;
        bucketImg.color = new Color(1f, 1f, 1f, .5f);
    }

    public void PickUpBucket(bool isFilled)
    {
        bucketImg.color = new Color(1f, 1f, 1f, 1f);
        bucketImg.sprite = isFilled ? filledBucketSprite : emptyBucketSprite;
    }

    public void DisplayEndScreen()
    {
        themeSound.Stop();
        gameOverSound.Play();
        paused = true;
        endScreen.GetChild(2).GetComponent<Text>().text = level.ToString();
        StartCoroutine(WaitForUser());
    }

    private IEnumerator WaitForUser()
    {
        LeanTween.alpha(endScreen, 0f, 0f);
        LeanTween.alpha((RectTransform) popup.transform, 0f, 0f);

        yield return null;
        endScreen.gameObject.SetActive(true);
        LeanTween.alpha(endScreen, 1f, .2f).setRecursive(false);
        yield return new WaitForSeconds(.6f);
        LeanTween.alphaText((RectTransform)endScreen.GetChild(0), 1f, 0f);
        yield return new WaitForSeconds(1.4f);
        LeanTween.alphaText((RectTransform)endScreen.GetChild(1), 1f, 0f);
        yield return new WaitForSeconds(.4f);
        LeanTween.alphaText((RectTransform)endScreen.GetChild(2), 1f, 0f);
        yield return new WaitForSeconds(.8f);
        Color color = new Color(168f / 255f, 168f / 255f, 168f / 255f, 1);
        LeanTween.value(0, 1, 1.5f).setLoopClamp().setOnUpdate((float val)
             => endScreen.GetChild(3).GetComponent<Text>().color = color * Mathf.Round(val));
        yield return new WaitUntil(() => InputManager.GetButtonDown(Button.A));
        LeanTween.alpha((RectTransform) curtain.transform, 1f, .2f).setOnComplete(()=> {
            LeanTween.cancelAll();
            SceneManager.LoadScene("StartScene");
        });
    }
}
