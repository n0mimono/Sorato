using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GearBoard {
  public class Character : MonoBehaviour {
    public AnimationClip[] animations;

    Animator animator;
    string[] animStrs;

    void Awake() {
      animator = GetComponent<Animator> ();
      animStrs = animations.Select (a => a.name).ToArray ();
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
      if (animStrs.Contains (str)) {
        animator.CrossFade (str, 0);
      } else {
        animator.CrossFade ("default", 0);
      }
    }

  }
}
