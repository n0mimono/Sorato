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

  public static T Random<T>(this List<T> list) {
    var index = Mathf.FloorToInt (UnityEngine.Random.value * list.Count);
    return list [index];
  }

  public static IEnumerator LazyStop(this AudioSource source) { 
    var orgVolume = source.volume;

    for (int i = 0; i < 60; i++) {
      source.volume = orgVolume * (60 - i) / 60f;
      yield return null;
    }

    source.Stop ();
    source.volume = orgVolume;
  }
}
