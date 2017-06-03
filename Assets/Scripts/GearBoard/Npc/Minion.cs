using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GearBoard {
  public class Minion : BoardNpc {
    public ParticleSystem effect;

    public override void Build() {
      board = GetComponent<Board> ();
      board.Build (new Status () {
        maxHp = 100f,
        curHp = 100f,
      });
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
    }

    public override void StartNpc() {
      StartCoroutine (Npc ());
    }

    IEnumerator Npc() {
      while (board.status.isAlive) {
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
