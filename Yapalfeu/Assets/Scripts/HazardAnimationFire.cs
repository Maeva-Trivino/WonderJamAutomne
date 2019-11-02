using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        if (mooving && !UIManager.Instance.HasWon)
        {
            ((RectTransform)transform).anchoredPosition += Vector2.right * speed * Time.deltaTime;

            List<ForestTree> burntTrees = new List<ForestTree>();

            foreach (ForestTree t in treesToBurn)
            {
                if ((((RectTransform)transform).anchoredPosition.x - 350) * 23f / 500 > t.gameObject.transform.position.x)
                {
                    t.SetOnFire();
                    burntTrees.Add(t);
                }
            }

            foreach (ForestTree t in burntTrees)
            {
                treesToBurn.Remove(t);
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

    public void Trigger(List<ForestTree> treesToBurn)
    {
        this.treesToBurn = treesToBurn;
        Reset();
        GetComponent<Image>().sprite = sprite;
        mooving = true;
    }
}
