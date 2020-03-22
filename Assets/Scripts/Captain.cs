using System.Collections;
using UnityEngine;

public class Captain : Pieces
{
    public int stepsLimitCarryingEnemyFlag = 3;

    public override ArrayList PossibleMoves(string[,] board, string fieldName, GameObject piece)
    {
        ArrayList possibleMoves = new ArrayList();

        void AddNeighbors()
        {
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i, j] == fieldName)
                    {
                        if (board[i, j + 1] != null)
                            possibleMoves.Add(board[i, j + 1]);

                        if (board[i + 1, j + 1] != null)
                            possibleMoves.Add(board[i + 1, j + 1]);

                        if (board[i + 1, j] != null)
                            possibleMoves.Add(board[i + 1, j]);

                        if (board[i, j - 1] != null)
                            possibleMoves.Add(board[i, j - 1]);

                        if (board[i - 1, j - 1] != null)
                            possibleMoves.Add(board[i - 1, j - 1]);

                        if (board[i - 1, j] != null)
                            possibleMoves.Add(board[i - 1, j]);

                        return;
                    }
                }
            }
        }

        AddNeighbors();

        //Remove captains collision
        if (piece.GetComponent<Pieces>().isGreen)
        {
            // if not destroyed
            if (GameObject.Find("RedCaptain") != null)
            {
                possibleMoves.Remove(GameObject.Find("RedCaptain").transform.parent.name);
                // if on board
                possibleMoves.Remove(GameObject.Find("RedCaptain").transform.parent.parent.name);
            }
        }
        else
        {
            if (GameObject.Find("GreenCaptain") != null)
            {
                possibleMoves.Remove(GameObject.Find("GreenCaptain").transform.parent.name);
                possibleMoves.Remove(GameObject.Find("GreenCaptain").transform.parent.parent.name);
            }
        }

        return possibleMoves;
    }

    public override bool OnCollision(RaycastHit hittenFieldInfo, GameObject piece)
    {
        // with an item, collect it (set as child)
        if (hittenFieldInfo.transform.childCount > 0 && hittenFieldInfo.transform.GetChild(0).tag == "Item")
        {
            // set item to be child of the captain
            hittenFieldInfo.transform.GetChild(0).SetParent(piece.transform);

            // center position of childs on captain
            for (int i = 0; i < piece.transform.childCount; i++)
            {
                piece.transform.GetChild(i).position = piece.transform.position;
            }
        }

        // while carrying enemy flag, steps limit
        if (piece.transform.childCount > 0)
        {
            // first child
            if (piece.GetComponent<Pieces>().isGreen != piece.transform.GetChild(0).GetComponent<Pieces>().isGreen)
            {
                CarryingEnemyFlag(0);
            } 
            else if (piece.transform.childCount == 2) // -> second is an enemy flag
            {
                CarryingEnemyFlag(1);
            }

            void CarryingEnemyFlag(int childNumber)
            {
                int limit = piece.transform.parent.tag == "Ship" ? 0 : -1;

                if (stepsLimitCarryingEnemyFlag == limit)
                {
                    if (piece.transform.parent.tag == "Ship")
                        piece.transform.GetChild(childNumber).SetParent(piece.transform.parent.parent);
                    else
                        piece.transform.GetChild(childNumber).SetParent(piece.transform.parent);

                    // reset steps
                    stepsLimitCarryingEnemyFlag = 3;
                }
                else
                {
                    stepsLimitCarryingEnemyFlag--;
                    Debug.Log("Captain can move enemy flag more: " + (stepsLimitCarryingEnemyFlag + 1) + " steps");
                }
            }
        }

        // with a ship (boarding)
        if (hittenFieldInfo.transform.childCount > 0 && hittenFieldInfo.transform.GetChild(0).tag == "Ship")
        {
            // while a flag is on board, catch it (set as child)
            if (hittenFieldInfo.transform.GetChild(0).childCount > 0 && hittenFieldInfo.transform.GetChild(0).GetChild(0).tag == "Item")
            {
                // parent flag
                hittenFieldInfo.transform.GetChild(0).GetChild(0).SetParent(piece.transform);

                // center position of childs on captain
                for (int i = 0; i < piece.transform.childCount; i++)
                    piece.transform.GetChild(i).position = piece.transform.position;
            }

            // get on board (be parented)
            piece.transform.SetParent(hittenFieldInfo.transform.GetChild(0));
            // center position
            piece.transform.position = hittenFieldInfo.transform.GetChild(0).position;

            Debug.Log("Moved: " + piece.name + " to: " + hittenFieldInfo.transform.name);

            return true;
        }

        //---------------

        // Move to field
        piece.transform.position = hittenFieldInfo.transform.position;
        // Set piece to be child of the clicked field
        piece.transform.SetParent(hittenFieldInfo.transform);

        Debug.Log("Moved: " + piece.name + " to: " + hittenFieldInfo.transform.name);

        return true;
    }
}
