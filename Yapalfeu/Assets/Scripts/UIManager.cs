using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text nbSeedText = null,
        nbLevelText = null,
        nbTreeText = null,
        timerText = null;
    [SerializeField]
    private Image bucketImg = null;
    [SerializeField]
    private float limitTime = 120;
    [SerializeField]
    private Sprite emptyBucketSprite = null;
    [SerializeField]
    private Sprite filledBucketSprite = null;
    [SerializeField]
    private AudioSource themeSound;

    private int nbActualTree;    
    private int nbGoalTree = 1;//
    private int level = 1;
    private float startTime;

    public static UIManager instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        startTime = Time.time;
        SetLevel(level, nbGoalTree);
        bucketImg.color = new Color(1f, 1f, 1f, .5f);
        themeSound.Play();
    }

    // Update is called once per frame
    void Update()
    {
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
            // TODO : END OF GAME
        }
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
}
