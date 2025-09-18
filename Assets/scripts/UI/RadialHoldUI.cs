using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using TMPro;
using Unity.VisualScripting;
using System.Data.Common;

public class RadialHoldUI : MonoBehaviour
{
    public Image[] playerRings = new Image[2];
    public float holdSeconds = 5f;

    private Coroutine[] _running = new Coroutine[2];
    private Action<int>[] _onDone = new Action<int>[2];

    private void Awake()
    {
        for (int i = 0; i < playerRings.Length; i++)
        {
            if (playerRings[i])
            {
                playerRings[i].fillAmount = 0f;
                playerRings[i].gameObject.SetActive(false);
            }
        }
    }

    public void Begin(int playerIndex, Action<int> onDone)
    {
        if (playerIndex < 0 || playerIndex >= _running.Length) return;

        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
        if (!enabled) enabled = true;

        Cancel(playerIndex);

        _onDone[playerIndex] = onDone;
        _running[playerIndex] = StartCoroutine(FillRing(playerIndex));
    }

    public void Cancel(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= _running.Length) return;
        if (_running[playerIndex] != null) StopCoroutine(_running[playerIndex]);
        _running[playerIndex] = null;

        if (playerRings[playerIndex])
        {
            playerRings[playerIndex].fillAmount = 0f;
            playerRings[playerIndex].gameObject.SetActive(false);
        }
        _onDone[playerIndex] = null;
    }

    private IEnumerator FillRing(int idx)
    {
        var ring = playerRings[idx];
        if (!ring) yield break;

        ring.fillAmount = 0f;
        ring.gameObject.SetActive(true);

        float t = 0f;
        while (t < holdSeconds)
        {
            t += Time.deltaTime;
            ring.fillAmount = Mathf.Clamp01(t / holdSeconds);
            yield return null;
        }

        ring.fillAmount = 1f;
        ring.gameObject.SetActive(false);
        _running[idx] = null;

        var done = _onDone[idx];
        _onDone[idx] = null;
        done?.Invoke(idx);
    }
}
