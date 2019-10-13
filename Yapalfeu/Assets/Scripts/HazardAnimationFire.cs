using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardAnimationFire: MonoBehaviour
{

    [SerializeField]
    private float speed = 1f;

    private bool mooving;

    [SerializeField]
    public Sprite sprite;

    public static HazardAnimationFire instance;

    private List<ForestTree> treesToBurn;

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

            List<ForestTree> clone = new List<ForestTree>(treesToBurn.Count);
            foreach (ForestTree t in treesToBurn)
            {
                clone.Add(t);
            }

            foreach (ForestTree t in clone)
            {
                if (transform.position.x > t.gameObject.transform.position.x)
                {
                    t.SetOnFire();
                    treesToBurn.Remove(t);
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
    public void Trigger(List<ForestTree> treesToBurn)
    {
        this.treesToBurn = treesToBurn;
        Reset();
        GetComponent<SpriteRenderer>().sprite = sprite;
        mooving = true;
    }


}
