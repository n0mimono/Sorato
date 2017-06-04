using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiTarget : MonoBehaviour {
  public Image image;
  public Image[] subImages;
  public Image dmgImage;
  public Image altImage;
  public Text text;

  bool isInScreen;
  bool isActive;

  public void Build() {
    gameObject.SetActive (true);
  }

  public void UpdateTargetPosition(ScreenPoint point) {
    var scrPos = point.pos;
    var isInScreen = point.isForward &&
                      scrPos.x > 0 &&
                      scrPos.x < Screen.width &&
                      scrPos.y > 0 &&
                      scrPos.y < Screen.height;
    SetInScreen (isInScreen);

    if (isInScreen) {
      transform.position = point.pos;
    } else {
      transform.position = new Vector3 (
        scrPos.x < Screen.width * 0.5f ? 0f : Screen.width,
        Mathf.Clamp(scrPos.y, 0f, Screen.height)
      );
    }
  }

  public void SetInScreen(bool isInScreen) {
    this.isInScreen = isInScreen;
    UpdateState ();
  }

  public void SetActiveTarget(bool isActive) {
    this.isActive = isActive;
    UpdateState ();
  }

  public void UpdateTargetDist(float dist) {
    text.text = dist.ToString ("F1") + " m";
  }

  void UpdateState() {
    image.enabled = isInScreen;
    altImage.enabled = !isInScreen;

    image.color = image.color.WithAlpha (isActive ? 1f : 0.4f);
    altImage.color = altImage.color.WithAlpha (isActive ? 1f : 0.4f);

    text.enabled = isInScreen && isActive;
    foreach (var i in subImages) {
      i.enabled = isInScreen && isActive;
    }

    bool flip = !isInScreen && transform.position.x < Screen.width * 0.5f;
    transform.localEulerAngles = new Vector3 (0f, flip ? 180f : 0f, 0f);
  }

  public void Stop() {
    gameObject.SetActive (false);
  }

}
