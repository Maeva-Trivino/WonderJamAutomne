using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HazardAnimationTempest : MonoBehaviour
{

    [SerializeField]
    private float speed = 1f;

    private bool mooving;

    [SerializeField]
    public Sprite sprite;

    public static HazardAnimationTempest instance;

    private List<ForestTree> treesToDrown;
    private List<ForestTree> burnableTrees;

    //Sound of the wave
    [SerializeField]
    private AudioSource waveSound;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        mooving = false;
    }

    // Update is called once per frame
    void Update()
    {
        System.Random number = new System.Random();

        if (mooving)
        {
            ((RectTransform)transform).anchoredPosition += Vector2.right * speed * Time.deltaTime;

            List<ForestTree> clone = new List<ForestTree>(treesToDrown.Count);
            foreach (ForestTree t in treesToDrown)
            {
                clone.Add(t);
            }

            foreach (ForestTree t in clone)
            {
                if ((((RectTransform)transform).anchoredPosition.x - 350) * 23f / 500 > t.gameObject.transform.position.x)
                {
                    t.Drown();
                    treesToDrown.Remove(t);
                }
            }

            foreach (ForestTree t in burnableTrees)
            {
                if (t.IsBurning() && (((RectTransform)transform).anchoredPosition.x - 350) * 23f / 500 > t.gameObject.transform.position.x)
                {
                    if (number.Next(1,101) > 50 )
                    {
                        t.StopBurning();
                    }
                    
                }
            }

            if (((RectTransform)transform).anchoredPosition.x > 1000f)
            {
                mooving = false;
            }
        }
        
    }

    private void Reset()
    {
        ((RectTransform)transform).anchoredPosition = Vector3.zero;
    }
    public void Trigger(List<ForestTree> treesToDrown, List<ForestTree> burnableTrees)
    {
        this.treesToDrown = treesToDrown;
        this.burnableTrees = burnableTrees;
        Reset();
        GetComponent<Image>().sprite = sprite;
        mooving = true;
        waveSound.Play();
    }
}
