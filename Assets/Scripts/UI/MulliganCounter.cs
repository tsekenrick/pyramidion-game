using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MulliganCounter : MonoBehaviour
{
    void Update() {
        GetComponent<TextMeshPro>().enabled = Board.instance.curPhase == Phase.Mulligan;
    }
}
