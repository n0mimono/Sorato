using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiResult : MonoBehaviour {
  public Image charaImage;
  public Sprite sprWin;
  public Sprite sprLose;
  public RectTransform charaOrigin;

  public GameObject bandRoot;

  public Image stampImage;
  public Image stampEffect;

  public Button resultBtn;

  Color stampColor;
  Color effectColor;
  Image[] bands;

  void Start() {
    gameObject.SetActive (false);
    resultBtn.gameObject.SetActive(false);

    bands = bandRoot.GetComponentsInChildren<Image> ();
    foreach (var b in bands) {
      b.fillAmount = 0f;
    }

    stampColor = stampImage.color;
    effectColor = stampEffect.color;
    stampImage.color = stampColor.WithAlpha (0f);
    stampEffect.color = effectColor.WithAlpha (0f);
  }

  public IEnumerator Result(bool win) {
    gameObject.SetActive (true);
    charaImage.sprite = win ? sprWin : sprLose;

    var charaRect = charaImage.GetComponent<RectTransform> ();
    var charaDelta = charaRect.anchoredPosition - charaOrigin.anchoredPosition;
    charaRect.anchoredPosition = charaOrigin.anchoredPosition;
    yield return Utility.Clock (0.3f, t => {
      charaRect.anchoredPosition = charaOrigin.anchoredPosition + charaDelta * t / 0.3f;
    });

    yield return Utility.Clock (0.8f, t => {
      foreach (var b in bands) {
        b.fillAmount = t / 0.8f;
      }
    });

    var stampRect = stampImage.GetComponent<RectTransform> ();
    yield return Utility.Clock (0.5f, t => {
      stampImage.color = stampColor.WithAlpha(t * 4f);
      stampRect.localScale = Vector3.one * (1f + (0.5f - t) * 10f);
    });
    var effectRect = stampEffect.GetComponent<RectTransform> ();
    yield return Utility.Clock (0.5f, t => {
      stampEffect.color = effectColor.WithAlpha((0.5f - t) * 8f);
      effectRect.localScale = Vector3.one * (1f + t * 2f * 0.2f);
    });

    yield return null;
    resultBtn.gameObject.SetActive(true);

    bool goNext = false;
    resultBtn.onClick.AddListener (() => {
      goNext = true;
    });

    yield return new WaitUntil (() => goNext);
    gameObject.SetActive (false);
  }

}
