using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GearBoard {
  public class AutoGear : MonoBehaviour {

    Board board;

    void Start() {
      board = GetComponent<Board> ();
      board.Build ();
      board.kinematics.engine.SetState (Engine.State.Stop);

      var tgt = GameObject.FindObjectsOfType<DamageReceptor> ()
        .Where (r => r.owner == DamageReceptor.Owner.Player)
        .FirstOrDefault ();
      board.shooter.SetTarget (tgt);

      StartCoroutine (Npc ());
    }

    IEnumerator Npc() {
      board.kinematics.engine.SetState (Engine.State.TopForward);

      while (true) {
        board.kinematics.rotater.SetRotate (true, 315f);
        yield return new WaitForSeconds(5f);

        board.kinematics.rotater.SetRotate (true, 270f);
        yield return new WaitForSeconds(5f);

        board.kinematics.rotater.SetRotate (false, 0f);
        yield return new WaitForSeconds(5f);

        board.kinematics.rotater.SetRotate (true, 90f);
        yield return new WaitForSeconds(5f);

        board.kinematics.rotater.SetRotate (true, 135f);
        yield return new WaitForSeconds(5f);

        board.kinematics.rotater.SetRotate (false, 0f);
        yield return new WaitForSeconds(5f);
      }
    }

  }
}
