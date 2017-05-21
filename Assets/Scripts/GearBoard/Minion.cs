using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GearBoard {
  public class Minion : MonoBehaviour {
    public ParticleSystem effect;

    Board board;

    void Start() {
      board = GetComponent<Board> ();
      board.Build ();
      board.kinematics.engine.SetState (Engine.State.Stop);

      // re-build
      board.shooter.Build(
        new Shooter.Status () {
          gauge = 0,
          max = 50,
          recoverPerSec = 50f,
        }
      );

      var tgt = GameObject.FindObjectsOfType<DamageReceptor> ()
        .Where (r => r.owner == DamageReceptor.Owner.Player)
        .FirstOrDefault ();
      board.shooter.SetTarget (tgt);

      effect.Stop ();
      StartCoroutine (Npc ());
    }

    IEnumerator Npc() {
      while (true) {
        yield return new WaitForSeconds (20f);

        effect.Play ();
        yield return new WaitForSeconds (3f);

        for (int i = 0; i < 20f; i++) {
          board.shooter.WillShoot ();

          yield return new WaitForSeconds (0.5f);
        }

        effect.Stop ();
      }

    }

  }
}
