using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GearBoard {
  public class Character : MonoBehaviour {
    public AnimationClip[] animations;

    Animator animator;

    void Awake() {
      animator = GetComponent<Animator> ();
    }

    public void SetSpeed(int spd) {
      animator.SetInteger ("Speed", spd);
    }

    public void InvokeFire() {
      animator.SetTrigger ("Fire");
    }

    public void InvokeDamage() {
      animator.SetTrigger ("Damage");
    }

    public void OnCallChangeFace (string str) {      
      foreach (var animation in animations) {
        if (str == animation.name) {
          Debug.Log (str);
          animator.CrossFade (str, 0);
          break;
        }
      }
    }

  }
}
