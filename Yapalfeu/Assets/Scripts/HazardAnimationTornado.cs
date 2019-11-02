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
    private AudioSource windSound = null;

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
            ((RectTransform)transform).anchoredPosition += Vector2.right * speed * Time.deltaTime;

            List<ForestTree> removedTrees = new List<ForestTree>();

            foreach (ForestTree t in treesToWind)
            {
                if ((((RectTransform)transform).anchoredPosition.x - 350) * 23f / 500f > t.gameObject.transform.position.x)
                {
                    if (t.HasSeed())
                    {
                        t.RemoveSeed();
                        removedTrees.Add(t);
                    }
                    else if (t.IsPlant()){
                        t.Drown();
                        removedTrees.Add(t);
                    }
                }
            }

            foreach (ForestTree t in removedTrees)
            {
                treesToWind.Remove(t);
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
        if(UIManager.Instance.HasWon)
            return;
        this.treesToWind = treesToWind;
        Reset();
        GetComponent<Image>().sprite = sprite;
        mooving = true;
        windSound.Play();
    }


}
