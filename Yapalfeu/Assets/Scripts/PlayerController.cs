using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Translation speed of the player
    public float speed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(
            speed * new Vector2(
                Input.GetAxis("Horizontal"),
                Input.GetAxis("Vertical")
            )
        );
    }
}
