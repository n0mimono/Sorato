using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiFlash : MonoBehaviour {
  Image image;

  Color orgColor;
  Color fullColor;

  bool isActive;
  float elapse;

  void Start() {
    image = GetComponent<Image> ();

    orgColor = image.color;
    fullColor = new Color (
      orgColor.r + (1f - orgColor.r) * 0.5f,
      orgColor.g + (1f - orgColor.g) * 0.5f,
      orgColor.b + (1f - orgColor.b) * 0.5f
    );
  }

  void Update() {
    if (isActive) {
      elapse += Time.deltaTime;
      var t = Mathf.PingPong (elapse, 1f);
      image.color = Color.Lerp (orgColor, fullColor, t);
    }
  }

  public void SetActive(bool isActive) {
    if (!this.isActive && isActive) {
      elapse = 0.5f;
    } else if (this.isActive && !isActive) {
      elapse = 0f;
      image.color = orgColor;
    }

    this.isActive = isActive;
  }

}
