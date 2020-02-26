using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : Pieces
{
    public float yRotationSpeed = 10f;

    void Update()
    {
        transform.Rotate(0, yRotationSpeed, 0);
    }
}