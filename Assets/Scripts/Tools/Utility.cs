using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class Utility {
  public const float deltaTime = 1f / 30f;

  public static Color WithAlpha(this Color color, float a) {
    return new Color (color.r, color.g, color.b, a);
  }

  public static IEnumerator Clock(float l, Action<float> onUpdate) {
    for (float t = 0; t < l; t += Utility.deltaTime) {
      onUpdate (t);
      yield return null;
    }

    onUpdate (l);
  }

  public static IEnumerator OnComplete(this IEnumerator routine, Action onComplete) {
    while (routine.MoveNext()) {
      yield return routine.Current;
    }

    onComplete ();
  }

}
