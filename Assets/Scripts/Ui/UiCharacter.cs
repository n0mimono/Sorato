using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiCharacter : MonoBehaviour {
  public Sprite[] sprites;

  CharaFace cur;
  Sprite curSprite;

  Image image;
  float elapse;

  void Awake() {
    image = GetComponent<Image> ();
  }

  public void SetFace(CharaFace next) {
    var nextSprite = sprites [(int)next];

    if (cur == next) {
      if (cur == CharaFace.Normal) {
        // noop
      } else {
        elapse = 0f;
      }
    } else {
      if (elapse > 0.5f) {
        cur = next;
        curSprite = nextSprite;
        image.sprite = curSprite;
        elapse = 0f;
      }
    }
  }

  void Update() {
    elapse += Time.deltaTime;

    if (elapse > 2f) {
      SetFace (CharaFace.Normal);
    }
  }

}

public enum CharaFace {
  Normal,
  Cool,
  Angry,
  Suprise,
  Smile,
  Ouch,
}
