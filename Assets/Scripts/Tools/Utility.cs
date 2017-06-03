using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class Utility {

  public static Color WithAlpha(this Color color, float a) {
    return new Color (color.r, color.g, color.b, a);
  }

  public static IEnumerator Clock(float l, Action<float> onUpdate) {
    for (float t = 0; t < l; t += Time.deltaTime) {
      onUpdate (t);
      yield return null;
    }

    onUpdate (l);
  }

}

