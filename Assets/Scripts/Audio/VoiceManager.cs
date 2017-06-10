using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class VoiceManager : MonoBehaviour {
  public AudioSource source;

  [Serializable]
  public class Voice {
    public List<AudioClip> clips;
    public float time;
  }
  public List<Voice> voices;

  public List<AudioClip> adlibs;
  float elapse;

  public void Play(int no) {
    var voice = voices [no];
    if (voice == null) {
      Debug.LogWarning (no);
    }
    if (Time.time - voice.time < 2f) {
      return;
    }
    voice.time = Time.time;

    source.clip = voice.clips.Random();
    source.Play ();
    elapse = 0f;
  }

  void Update() {
    elapse += Time.deltaTime;
    if (elapse > 10f) {
      source.clip = adlibs.Random();
      source.Play ();
      elapse = 0f;
    }
  }

}

public static class Voice {
  static VoiceManager instance {
    get {
      return GameObject.FindObjectOfType<VoiceManager>();
    }
  }

  public static void Play(VO vo) {
    instance.Play ((int)vo);
  }

}

public enum VO {
  Attack,
  Hit,
  Dead,
  Start,
  Win,
  Lose,
  BackToTitle,
  Damage
}
