using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorBehaviour : MonoBehaviour
{
    
    public Texture2D[] cursors;

    void Start() {
        Cursor.SetCursor(cursors[0], Vector2.zero, CursorMode.Auto);
    }

    void Update() {
        if(Input.GetMouseButton(0)) {
            Cursor.SetCursor(cursors[1], Vector2.zero, CursorMode.Auto);
        } else {
            Cursor.SetCursor(cursors[0], Vector2.zero, CursorMode.Auto);
        }
    }
}
