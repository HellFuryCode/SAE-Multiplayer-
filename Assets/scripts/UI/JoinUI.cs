using UnityEngine;
using TMPro;
using System.Collections;


public class JoinUI : MonoBehaviour
{
    public System.Action OnHidden;

    public TMP_Text titleText;
    public TMP_Text p1Text;
    public TMP_Text p2Text;


    public bool waitForBothPlayers = false;  // if true: hide after both joined; else hide after first join
    public float autoHideDelay = 2f;         // seconds to wait after condition met
    public float fadeSeconds = 0.35f;

    private CanvasGroup _group;
    private bool _p1Joined, _p2Joined;
    private Coroutine _hideCo;

    private void Awake()
    {
        _group = GetComponent<CanvasGroup>();
        _group.alpha = 1f;
        _group.interactable = true;
        _group.blocksRaycasts = true;
    }

    private void OnEnable()
    {
        // reset texts every time we show this panel
        _p1Joined = _p2Joined = false;
        if (titleText) titleText.text = "Press to Join";
        if (p1Text) p1Text.text = "Player 1 — press <b>F</b> (WASD) or Controller <b>Start</b>";
        if (p2Text) p2Text.text = "Player 2 — press <b>Enter</b> (Arrows) or Controller <b>Start</b>";

        // subscribe to JoinSyste
        var joinSys = FindFirstObjectByType<JoinSystem>();
        if (joinSys != null)
        {
            joinSys.OnLocalPlayerJoined += HandlePlayerJoined;
        }
    }

    private void OnDisable()
    {
        var joinSys = FindFirstObjectByType<JoinSystem>();
        if (joinSys != null)
        {
            joinSys.OnLocalPlayerJoined -= HandlePlayerJoined;
        }
    }

    private void HandlePlayerJoined(int slotIndex)
    {
        // slotIndex: 0 => Player1, 1 => Player2
        if (slotIndex == 0)
        {
            _p1Joined = true;
            if (p1Text) p1Text.text = "<b>Player 1 joined!</b>";
        }
        else if (slotIndex == 1)
        {
            _p2Joined = true;
            if (p2Text) p2Text.text = "<b>Player 2 joined!</b>";
        }

        if (titleText)
        {
            if (waitForBothPlayers)
            {
                int count = (_p1Joined ? 1 : 0) + (_p2Joined ? 1 : 0);
                titleText.text = (count == 2) ? "All set!" : "Waiting for second player…";
            }
            else
            {
                titleText.text = "Ready!";
            }
        }

        // start/refresh auto-hide when condition met
        if ((!waitForBothPlayers && (_p1Joined || _p2Joined)) ||
            (waitForBothPlayers && _p1Joined && _p2Joined))
        {
            if (_hideCo != null) StopCoroutine(_hideCo);
            _hideCo = StartCoroutine(HideAfterDelay());
        }
    }

    private IEnumerator HideAfterDelay()
    {
        // small hold
        float t = 0f;
        while (t < autoHideDelay)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        // fade out fancy
        t = 0f;
        float start = _group.alpha;
        while (t < fadeSeconds)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / fadeSeconds);
            _group.alpha = Mathf.Lerp(start, 0f, k);
            yield return null;
        }


    }

    public void HideNow()
    {
        if (_hideCo != null)
        {
            StopCoroutine(_hideCo = null);
        }
        _group.alpha = 0f;
        _group.interactable = false;
        _group.blocksRaycasts = false;
        gameObject.SetActive(false);
        OnHidden?.Invoke();
         Debug.Log("[JoinUI] HideNow()"); ///bitvh
    }
}


