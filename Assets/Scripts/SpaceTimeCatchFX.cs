using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpaceTimeCatchFX : MonoBehaviour
{
    [Header("Glitch")]
    [SerializeField] private GameObject[] glitchBars;

    [SerializeField] private float glitchDuration = 1.2f;
    [SerializeField] private float glitchInterval = 0.05f;

    [Header("Black Fade")]
    [SerializeField] private Image blackFade;
    [SerializeField] private float fadeDuration = 0.8f;

    private Coroutine effectCoroutine;

    private void Awake()
    {
        ResetEffect();
    }

    public void PlayEffect()
    {
        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
        }

        effectCoroutine = StartCoroutine(PlayEffectCoroutine());
    }

    public void ResetEffect()
    {
        // Apagamos todas las barras
        foreach (GameObject bar in glitchBars)
        {
            if (bar != null)
            {
                bar.SetActive(false);
            }
        }

        // Dejamos el negro totalmente transparente
        if (blackFade != null)
        {
            Color colour = blackFade.color;
            colour.a = 0f;
            blackFade.color = colour;
        }
    }

    private IEnumerator PlayEffectCoroutine()
    {
        float elapsed = 0f;

        // GLITCH
        while (elapsed < glitchDuration)
        {
            float progress = elapsed / glitchDuration;

            // Cuanto más nos acercamos al final,
            // más probable es que aparezcan barras
            float probability = Mathf.Lerp(
                0.2f,
                0.8f,
                progress
            );

            foreach (GameObject bar in glitchBars)
            {
                if (bar == null)
                    continue;

                bool visible = Random.value < probability;

                bar.SetActive(visible);
            }

            yield return new WaitForSecondsRealtime(
                glitchInterval
            );

            elapsed += glitchInterval;
        }

        // Quitamos las barras
        foreach (GameObject bar in glitchBars)
        {
            if (bar != null)
            {
                bar.SetActive(false);
            }
        }

        // FADE A NEGRO
        float fadeElapsed = 0f;

        while (fadeElapsed < fadeDuration)
        {
            fadeElapsed += Time.unscaledDeltaTime;

            float alpha =
                Mathf.Clamp01(fadeElapsed / fadeDuration);

            Color colour = blackFade.color;
            colour.a = alpha;
            blackFade.color = colour;

            yield return null;
        }

        // Nos aseguramos de acabar completamente en negro
        Color finalColour = blackFade.color;
        finalColour.a = 1f;
        blackFade.color = finalColour;

        effectCoroutine = null;
    }

    [ContextMenu("TEST EFFECT")]
    private void TestEffect()
    {
        if (!Application.isPlaying)
        {
            Debug.Log(
                "Enter Play Mode to test the effect."
            );

            return;
        }

        PlayEffect();
    }
}
