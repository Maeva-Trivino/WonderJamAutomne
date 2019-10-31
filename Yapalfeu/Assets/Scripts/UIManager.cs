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
    private GameObject popup = null;
    [SerializeField]
    private List<RectTransform> hazards = null;


    //Sound of the theme
    [SerializeField]
    private AudioSource themeSound = null;

    [Header("Game Over Screen")]
    [SerializeField]
    private AudioSource gameOverSound = null;
    [SerializeField]
    //Sound of the gameOver
    private RectTransform endScreen = null;

    [Header("Win Screen")]
    [SerializeField]
    private AudioSource winSound = null;
    [SerializeField]
    private AudioSource popSound = null;
    [SerializeField]
    //Sound of the gameOver
    private RectTransform winScreen = null;
    [SerializeField]
    private List<RectTransform> forest = null;

    private int nbSeeds = 10;
    private int nbActualTree;
    private int nbGoalTree = 1;//
    private int level = 1;
    private float startTime;

    private static UIManager internalInstance;

    public static UIManager Instance
    {
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
        forest.Sort((i, j) => (int)Random.Range(0, 2) * 2 - 1);
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
        if (paused)
            return;
        float t = Time.time - startTime;
        float seconds = (limitTime - t);
        if (nbActualTree == nbGoalTree)
        {

            if (level == 1)
            {
                nbGoalTree += 4;
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
            SetLevel(level, nbGoalTree);
            SetTime(limitTime + limitTime - t);
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
            DisplayGameOverScreen();
        }

        var f2 = System.Array.Find(GameObject.FindGameObjectsWithTag("Tree"), o => o.GetComponent<ForestTree>().IsAlive());
        if (f2 == null && nbSeeds == 0)
            DisplayWinScreen(654);// DisplayGameOverScreen(); TODO chqnge
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

    public void DisplayGameOverScreen()
    {
        themeSound.Stop();
        gameOverSound.Play();
        paused = true;
        endScreen.GetChild(2).GetComponent<Text>().text = level.ToString();
        StartCoroutine(WaitForUserGameOver());
    }

    public void DisplayWinScreen(float playDuration)
    {
        themeSound.Stop();
        // winSound.Play(); TODO Maeva
        paused = true;
        StartCoroutine(WaitForUserWin(playDuration));
    }

    private IEnumerator WaitForUserGameOver()
    {
        LeanTween.alpha(endScreen, 0f, 0f);
        LeanTween.alpha((RectTransform)popup.transform, 0f, 0f);

        // Show step by step text
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

        // Wait for user input
        yield return new WaitUntil(() => InputManager.GetButtonDown(Button.A));
        LeanTween.alpha((RectTransform)curtain.transform, 1f, .2f).setOnComplete(() =>
        {
            LeanTween.cancelAll();
            SceneManager.LoadScene("StartScene");
        });
    }
    private IEnumerator WaitForUserWin(float playDuration)
    {
        LeanTween.alpha(winScreen, 0f, 0f);
        LeanTween.alpha((RectTransform)popup.transform, 0f, 0f);
        foreach (RectTransform rt in hazards)
            LeanTween.alpha(rt, 0f, .2f);

        // Show step by step text
        yield return null;
        winScreen.gameObject.SetActive(true);
        LeanTween.alpha(winScreen, 1f, .2f).setRecursive(false);
        yield return new WaitForSeconds(.6f);
        LeanTween.alphaText((RectTransform)winScreen.GetChild(1), 1f, 0f);
        yield return new WaitForSeconds(1.15f);
        LeanTween.alphaText((RectTransform)winScreen.GetChild(2), 1f, 0f);
        yield return new WaitForSeconds(.65f);
        LeanTween.alphaText((RectTransform)winScreen.GetChild(3), 1f, 0f);

        // Process score animation
        float speed = 7, time = 0, progress = 0;
        int index = 0;
        while (time < playDuration)
        {
            yield return new WaitForSeconds(.01f);
            progress = time / playDuration;
            int current = (int)(forest.Count * progress);
            for (int i = index; i < current; i++)
            {
                forest[i].GetComponent<Image>().color = Color.white;
                LeanTween.scale(forest[i], Vector3.one, 0.2f).setEaseOutBack();
                popSound.Play();
            }
            winScreen.GetChild(3).GetComponent<Text>().text = ((int)(time / 60)) + ":" + (time % 60).ToString("00");

            time += speed;
            index = current;
        }

        // Set final values
        LeanTween.scale((RectTransform)winScreen.GetChild(3), Vector3.one, 0.15f).setEaseOutBack();
        yield return new WaitForSeconds(.01f);

        for (int i = index; i < forest.Count; i++)
        {
            forest[i].GetComponent<Image>().color = Color.white;
            LeanTween.scale(forest[i], Vector3.one, 0.2f).setEaseOutBack();
        }
        winScreen.GetChild(3).GetComponent<Text>().text = ((int)(playDuration / 60)) + ":" + (playDuration % 60);

        // Show final text
        yield return new WaitForSeconds(.8f);
        Color color = new Color(168f / 255f, 168f / 255f, 168f / 255f, 1);
        LeanTween.value(0, 1, 1.5f).setLoopClamp().setOnUpdate((float val)
             => winScreen.GetChild(4).GetComponent<Text>().color = color * Mathf.Round(val));

        // Wait for user input
        yield return new WaitUntil(() => InputManager.GetButtonDown(Button.A));
        LeanTween.alpha((RectTransform)curtain.transform, 1f, .2f).setOnComplete(() =>
        {
            LeanTween.cancelAll();
            SceneManager.LoadScene("StartScene");
        });
    }
}
