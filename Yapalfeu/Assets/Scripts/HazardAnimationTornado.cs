using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardAnimationTornado : MonoBehaviour
{

    [SerializeField]
    private float speed = 1f;

    private bool mooving;

    [SerializeField]
    public Sprite sprite;

    public static HazardAnimationTornado instance;

    private List<ForestTree> treesToWind;

    //Sound of the wind
    [SerializeField]
    private AudioSource windSound;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        mooving = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (mooving)
        {
            transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);

            List<ForestTree> clone = new List<ForestTree>(treesToWind.Count);
            foreach (ForestTree t in treesToWind)
            {
                clone.Add(t);
            }

            foreach (ForestTree t in clone)
            {
                if (transform.position.x > t.gameObject.transform.position.x)
                {
                    t.RemoveSeed();
                    treesToWind.Remove(t);

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
    public void Trigger(List<ForestTree> treesToWind)
    {
        this.treesToWind = treesToWind;
        Reset();
        GetComponent<SpriteRenderer>().sprite = sprite;
        mooving = true;
        windSound.Play();
    }


}
