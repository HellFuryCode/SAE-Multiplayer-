
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MashEscape : MonoBehaviour
{

    public PlayerCarry target; //who
    public Camera followCamera; //what
    public Transform followAnchor; //where

    public Image radialfill;
    public TextMeshProUGUI mashtext;
    public CanvasGroup group;

    public bool worldSpace = true;
    public Vector3 worldOffeset = new Vector3(0f, 1.8f, 0f);
    public float showFadeSpeed = 10f;

    public float jitterAmp = 8f;
    public float jitterspeed = 22f;
    public float ringSpingSpeed = 60f;

    private Vector3 baseAnchorPos;
    private RectTransform react;
    private bool shouldshow;

    public void Awake()
    {
        react = GetComponent<RectTransform>();
        if (group == null)
        {
            group = gameObject.AddComponent<CanvasGroup>();
            baseAnchorPos = react.anchoredPosition;
            SetVisible(false, instant: true);

            if (target)
            {
                target.OnGrabbed += HandleGrabbed;
                target.OnReasled += HandleRelease;
            }
        }
    }

    private void OnDestroy()
    {
        if (target)
        {
            target.OnGrabbed -= HandleGrabbed;
            target.OnReasled -= HandleRelease;
        }
    }

    private void HandleGrabbed() => SetVisible(true);
    private void HandleRelease() => SetVisible(false);

    private void SetVisible(bool v, bool instant = false)
    {
        shouldshow = v;
        if (instant)
        {
            group.alpha = v ? 1f : 0f;
            gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        float targetAlpha = shouldshow ? 1f : 0f;
        group.alpha = Mathf.MoveTowards(group.alpha, targetAlpha, showFadeSpeed * Time.deltaTime);
        if (group.alpha <= 0.001f && !shouldshow) return;
    

        if (target && radialfill)
        {
            radialfill.fillAmount = target.MashFill01;
        }

        if (mashtext)
        {
            float t = Time.time * jitterspeed;
            Vector3 jitter = new Vector2(
                (Mathf.PerlinNoise(t, 0.13f) - 0.05f),
                (Mathf.PerlinNoise(0.47f, t) - 0.5f)
            ) * jitterAmp;
            mashtext.rectTransform.anchoredPosition = baseAnchorPos + jitter;
        }

        if (worldSpace && followAnchor && followCamera)
        {
            transform.position = followAnchor.position + worldOffeset;
            transform.rotation = Quaternion.LookRotation(transform.position - followCamera.transform.position, Vector3.up);
        }
    } 
}
