using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            ((RectTransform)transform).anchoredPosition += Vector2.right * speed;

            List<ForestTree> clone = new List<ForestTree>(treesToWind.Count);
            foreach (ForestTree t in treesToWind)
            {
                clone.Add(t);
            }

            foreach (ForestTree t in clone)
            {
                if ((((RectTransform)transform).anchoredPosition.x - 350) * 23f / 500f > t.gameObject.transform.position.x)
                {
                    if (t.HasSeed())
                    {
                        t.RemoveSeed();
                        treesToWind.Remove(t);
                    }
                    else  if (t.IsPlant()){
                        t.Drown();
                        treesToWind.Remove(t);
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
    public void Trigger(List<ForestTree> treesToWind)
    {
        this.treesToWind = treesToWind;
        Reset();
        GetComponent<Image>().sprite = sprite;
        mooving = true;
        windSound.Play();
    }


}
