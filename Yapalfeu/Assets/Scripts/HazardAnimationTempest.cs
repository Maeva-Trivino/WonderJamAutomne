using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
            
            List<ForestTree> clone = new List<ForestTree>(treesToDrown.Count);
            foreach (ForestTree t in treesToDrown)
            {
                clone.Add(t);
            }

            foreach (ForestTree t in clone)
            {
                if (transform.position.x > t.gameObject.transform.position.x)
                {
                    t.Drown();
                    treesToDrown.Remove(t);
                }
            }

            foreach (ForestTree t in burnableTrees)
            {
                if (t.IsBurning() && transform.position.x > t.gameObject.transform.position.x)
                {
                    if (number.Next(1,101) > 50 )
                    {
                        t.StopBurning();
                    }
                    
                }
            }
            
            if (transform.position.x > 23.5)
                {
                mooving = false;
            }
        }
        
    }

    private void Reset()
    {
        transform.position = new Vector3(-22.5f, -1.5f, 0f);
    }
    public void Trigger(List<ForestTree> treesToDrown, List<ForestTree> burnableTrees)
    {
        this.treesToDrown = treesToDrown;
        this.burnableTrees = burnableTrees;
        Reset();
        GetComponent<SpriteRenderer>().sprite = sprite;
        mooving = true;
        waveSound.Play();
    }
}
