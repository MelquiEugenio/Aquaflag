using System.Collections;
using UnityEngine;

public abstract class Pieces : MonoBehaviour
{
    public bool isGreen;

    public virtual ArrayList PossibleMoves(string[,] board, string fieldName, GameObject piece)
    {
        Debug.Log("Piece is an Item");
        return null;
    }

    public virtual bool OnCollision(RaycastHit hittenFieldInfo, GameObject piece)
    {
        Debug.Log("Piece is an Item");
        return false;
    }
}
