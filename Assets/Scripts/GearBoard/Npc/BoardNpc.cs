using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GearBoard {
  public class BoardNpc : MonoBehaviour {
    public Board board { protected set; get; }

    public virtual void Build() {
    }

    public virtual void StartNpc() {
    }

  }
}
