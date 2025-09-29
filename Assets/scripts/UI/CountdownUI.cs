using UnityEngine;
using System.Collections;
using TMPro;
using System;

public class CountdownUI : MonoBehaviour
{
    public TMP_Text label;
    public float beatSeconds = 1f;
    public float popScale = 1.3f;  //shrink after big start
    public float fadeinFrac = 0.25f;
    public float fadeoutFrac = 0.15f;

    CanvasGroup group;
    Vector3 baseScale;

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
        if (!label) label = GetComponent<TMP_Text>();
        baseScale = transform.localScale;
        gameObject.SetActive(false);
        group.alpha = 0f;
    }

    public IEnumerator PlayCountDown(System.Collections.Generic.IList<string> sequence)
    {
        if (sequence == null || sequence.Count == 0) yield break;

        gameObject.SetActive(true);

        for (int i = 0; i < sequence.Count; i++)
        {
            string text = sequence[i];
            label.text = text;

            transform.localScale = baseScale * popScale;
            group.alpha = 0f;

            float t = 0f;
            float fadeInTime = beatSeconds * Mathf.Clamp01(fadeinFrac);
            float fadeOutTime = beatSeconds * Mathf.Clamp01(fadeoutFrac);
            float steadyTime = Mathf.Max(0f, beatSeconds - fadeInTime - fadeOutTime);

            while (t < fadeInTime)
            {
                t += Time.unscaledDeltaTime;
                float k = Mathf.Clamp01(t / fadeInTime);
                group.alpha = k;
                transform.localScale = Vector3.Lerp(baseScale * popScale, baseScale, k);
                yield return null;
            }


            t = 0f;
            group.alpha = 1f;
            transform.localScale = baseScale;
            while (t < steadyTime)
            {
                t += Time.unscaledDeltaTime;
                yield return null;
            }

            t = 0f;
            while (t < fadeOutTime)
            {
                t += Time.unscaledDeltaTime;
                float k = 1f - Mathf.Clamp01(t / fadeOutTime);
                group.alpha = k;
                yield return null;
            }
        }
        group.alpha = 0f;
        // gameObject.SetActive(false);

    }


}

// How To Make A HORROR Game In Unity | Basic Health System | Horror Series Part 024
//date accessed 2025/9/17
//created by: User1 Productions
//created on: 2022
//url: https://www.youtube.com/watch?v=qRgKB8l9GIg&list=PLlcgaDpDEvw05IgKGZo9FYA8Fo38RtAqH&index=24
//Online Video
//youtube
