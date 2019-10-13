using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private UIManager uIManager;
    [SerializeField]
    private VideoPlayer videoIntro;

    private CanvasGroup group;
    private bool isHidden;
    // Start is called before the first frame update
    void Start()
    {
        isHidden = false;
        group = GetComponent<CanvasGroup>();
        LeanTween.alpha((RectTransform)videoIntro.transform.parent, 0f, 0f).setRecursive(false);
        videoIntro.Play();
        videoIntro.Pause();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isHidden && InputManager.GetButtonDown(Button.A))
        {
            isHidden = true;
            videoIntro.Play();
            StartCoroutine(ShowIntro());
        }
    }

    public IEnumerator ShowIntro()
    {
        LeanTween.alpha((RectTransform)transform, 0f, .3f);

        float t = Time.time;
        yield return new WaitUntil(() => (InputManager.GetButtonDown(Button.A) && (Time.time - t) > 2) || (Time.time - t) > 25);

        LeanTween.alpha((RectTransform)videoIntro.transform.parent, 1f, .2f).setRecursive(false).setOnComplete(() =>
        {
            videoIntro.Stop();
            StartGame();
        });
    }

    public void MainMenu()
    {
        group.alpha = 1;
    }

    public void StartGame()
    {
        LeanTween.alpha((RectTransform)videoIntro.transform.parent, 0f, .2f).setRecursive(false);
        uIManager.gameObject.SetActive(true);
    }
}
