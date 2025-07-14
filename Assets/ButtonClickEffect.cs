using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonClickEffect : MonoBehaviour
{
    public Material normalMaterial;


    public GameObject PanelOptions;

    public Animator Right;

    private RectTransform rect;

    private Image buttonImage;
    private Button Button;

    public float scaleFactor;
    public float highlightSpeed;


    private bool isClicked = false; 

    private float effectTimer = 0f;

    private Vector3 originalScale;

    public GameObject loadingScreenCanvas;
    public Slider loadingSlider;
    public float fakeLoadDuration = 3f;

    

    void Start()
    {
        buttonImage = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        Button = GetComponent<Button>();

        originalScale = rect.transform.localScale;

        if (buttonImage != null && normalMaterial != null)
        {
            buttonImage.material = normalMaterial;
        }

        if (rect.TryGetComponent<EventTrigger>(out var buttonTrigger)) 
        {
            buttonTrigger = Button.gameObject.AddComponent<EventTrigger>();
        }

        var entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        entryEnter.callback.AddListener((data) => OnButtonHoverEnter());
        buttonTrigger.triggers.Add(entryEnter);

        var entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        entryExit.callback.AddListener((data) => OnButtonHoverExxit());
        buttonTrigger.triggers.Add(entryExit);


        //AudioManager.Instance.PlayMusic("ThemeStart");
    }

    void Update()
    {
        

        if (isClicked)
        {
            effectTimer -= Time.deltaTime;
            if (effectTimer <= 0f)
            {
                ResetToNormalMaterial();
            }
        }
    }

    public void OnButtonClickPlay()
    {
        Right.SetTrigger("PlayRight");
    }

    public void OnPanelAnimationComplete()
    {
        StartLoading();
    }

    public void OnButtonClickQuit()
    {
        Application.Quit();
    }

    public void OnButtonClickOPtions()
    {
        PanelOptions.SetActive(true);
    }

    public void OnButtonClickCross()
    {
        PanelOptions.SetActive(false);
    }

    public void OnButtonHoverEnter()
    {
        StopAllCoroutines();
        StartCoroutine(ScaleButton(originalScale * scaleFactor));
    }

    public void OnButtonHoverExxit()
    {
        StopAllCoroutines();
        StartCoroutine(ScaleButton(originalScale));
    }

    private void ResetToNormalMaterial()
    {
        // Retour au matériau normal
        if (buttonImage != null && normalMaterial != null)
        {
            buttonImage.material = normalMaterial;
            rect.localScale = Vector3.one;
        }
        isClicked = false;
    }

    private IEnumerator ScaleButton(Vector3 targetScale)
    {
        Vector3 currentScale = rect.transform.localScale;

        while (Vector3.Distance(currentScale, targetScale) > 0.01f) 
        {
            currentScale = Vector3.Lerp(currentScale, targetScale, highlightSpeed * Time.deltaTime);
            rect.transform.localScale = currentScale;
            yield return null;
        }

        rect.transform.localScale = targetScale;

    }

    public void StartLoading()
    {
        loadingScreenCanvas.SetActive(true);
        StartCoroutine(FakeLoading());
    }

    private IEnumerator FakeLoading()
    {
        float elapsedTime = 0f;

        loadingSlider.value = 0f;

        while (elapsedTime < fakeLoadDuration)
        {
            elapsedTime += Time.deltaTime;

            loadingSlider.value = elapsedTime / fakeLoadDuration;

            yield return null;
        }
        loadingSlider.value = 1f;

        LoadNextScene();
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene("New Scene");
    }
}
