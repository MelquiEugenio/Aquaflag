using System.Collections;
using UnityEngine;

public abstract class Pieces : MonoBehaviour
{
    public bool isGreen;

    public virtual ArrayList PossibleMoves(string[,] board, string field)
    {
        //hopefully never called
        return null;
    }

    public virtual bool OnCollision(RaycastHit hittenFieldInfo, GameObject piece)
    {
        return false;
    }
}
