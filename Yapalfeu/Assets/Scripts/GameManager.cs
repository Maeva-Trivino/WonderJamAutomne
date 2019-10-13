using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private VideoPlayer videoIntro;

    private CanvasGroup group;
    private bool isHidden;
    private bool isCreditOpen;
    // Start is called before the first frame update
    void Start()
    {
        isHidden = false;
        isCreditOpen = false;
        group = GetComponent<CanvasGroup>();
        LeanTween.alpha((RectTransform)videoIntro.transform.parent, 0f, 0f).setRecursive(false);
        videoIntro.Play();
        videoIntro.Pause();

        LeanTween.alpha((RectTransform)videoIntro.transform.parent, 0f, .2f);
    }

    // Update is called once per frame
    void Update()
    {
        GetComponentsInChildren<Text>()[1].text = "APPUYEZ SUR "+InputManager.GetButtonName(Button.A)+"\nPOUR COMMENCER";
        if (!isHidden && !isCreditOpen && InputManager.GetButtonDown(Button.A))
        {
            isHidden = true;
            videoIntro.Play();
            StartCoroutine(ShowIntro());
        }
        else if(!isHidden && !isCreditOpen && InputManager.GetButtonDown(Button.X))
        {
            isCreditOpen = true;
            transform.GetChild(0).GetComponent<Text>().enabled = false;
            transform.GetChild(1).GetComponent<Text>().enabled = false;
            transform.GetChild(2).GetComponent<Text>().enabled = false;
            transform.GetChild(3).GetComponent<Text>().enabled = true;
            transform.GetChild(4).GetComponent<Text>().enabled = true;
        }
        else if (!isHidden && isCreditOpen && InputManager.GetButtonDown(Button.X))
        {
            isCreditOpen = false;
            transform.GetChild(0).GetComponent<Text>().enabled = true;
            transform.GetChild(1).GetComponent<Text>().enabled = true;
            transform.GetChild(2).GetComponent<Text>().enabled = true;
            transform.GetChild(3).GetComponent<Text>().enabled = false;
            transform.GetChild(4).GetComponent<Text>().enabled = false;
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
        LeanTween.alpha((RectTransform)videoIntro.transform.parent, 0f, .2f).setRecursive(false).setOnComplete(() => SceneManager.LoadScene("MainScene"));
    }
}
