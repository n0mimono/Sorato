using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UiResult : MonoBehaviour {
  public Image baseImage;

  [Header("Character")]
  public Image charaImage;
  public Sprite sprWin;
  public Sprite sprLose;
  public RectTransform charaOrigin;

  [Header("Back Bands")]
  public GameObject bandRoot;
  public Image bandBase;

  [Header("Stamp")]
  public Image stampImage;
  public Image stampEffect;

  [Header("Next")]
  public Button resultBtn;

  void Start() {
    gameObject.SetActive (false);
    resultBtn.gameObject.SetActive(false);
  }

  public IEnumerator Result(bool win, Action onComplete) {
    charaImage.sprite = win ? sprWin : sprLose;

    var bands = bandRoot.GetComponentsInChildren<Image> ();
    foreach (var b in bands) {
      b.fillAmount = 0f;
    }
    bandBase.fillAmount = 0f;

    var stampRect = stampImage.GetComponent<RectTransform> ();
    var effectRect = stampEffect.GetComponent<RectTransform> ();
    var stampColor = stampImage.color;
    var effectColor = stampEffect.color;
    stampImage.color = stampColor.WithAlpha (0f);
    stampEffect.color = effectColor.WithAlpha (0f);

    var charaRect = charaImage.GetComponent<RectTransform> ();
    var charaDelta = charaRect.anchoredPosition - charaOrigin.anchoredPosition;
    charaRect.anchoredPosition = charaOrigin.anchoredPosition;

    gameObject.SetActive (true);

    var baseColor = baseImage.color;
    yield return Utility.Clock (1f, t => {
      baseImage.color = baseColor.WithAlpha(t * 0.5f);
    });
    yield return Utility.Clock (0.3f, t => {
      charaRect.anchoredPosition = charaOrigin.anchoredPosition + charaDelta * t / 0.3f;
    });
    yield return Utility.Clock (0.8f, t => {
      foreach (var b in bands) {
        b.fillAmount = t / 0.8f;
      }
    });
    yield return Utility.Clock (0.4f, t => {
      bandBase.fillAmount = t / 0.4f;
    });
    yield return Utility.Clock (0.5f, t => {
      stampImage.color = stampColor.WithAlpha(t * 4f);
      stampRect.localScale = Vector3.one * (1f + (0.5f - t) * 10f);
    });
    yield return Utility.Clock (0.5f, t => {
      stampEffect.color = effectColor.WithAlpha((0.5f - t) * 8f);
      effectRect.localScale = Vector3.one * (1f + t * 2f * 0.2f);
    });

    yield return null;
    resultBtn.gameObject.SetActive(true);

    var goNext = false;
    resultBtn.onClick.AddListener (() => {
      goNext = true;
    });

    yield return new WaitUntil (() => goNext);
    gameObject.SetActive (false);

    onComplete ();
  }

}
