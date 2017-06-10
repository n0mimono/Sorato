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
  }
  public List<Voice> voices;

  float elapse = 10f;
  int latest = -1;

  public void Play(int no) {
    if (no == latest && elapse < 3f) {
      return;
    }
    if (no != latest && elapse < 1f) {
      return;
    }

    var voice = voices [no];
    if (voice == null) {
      Debug.LogWarning (no);
    }

    source.clip = voice.clips.Random();
    source.Play ();

    elapse = 0f;
    latest = no;
  }

  void Update() {
    elapse += Time.deltaTime;
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
