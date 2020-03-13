using System.Collections;
using UnityEngine;

public class Captain : Pieces
{
    public int stepsLimitCarryingEnemyFlag = 3;

    public override ArrayList PossibleMoves(string[,] board, string fieldName)
    {
        ArrayList possibleMoves = new ArrayList();

        void FindNeighbors()
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

        FindNeighbors();

        return possibleMoves;
    }

    public override bool OnCollision(RaycastHit hittenFieldInfo, GameObject piece)
    {
        // with another captain, invalid move
        if (hittenFieldInfo.transform.childCount > 0 && hittenFieldInfo.transform.GetChild(0).tag == "Captain")
        {
            Debug.Log("Invalid Move");
            return false;
        }

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

        // carrying enemy flag, steps limit
        if (piece.transform.childCount > 0)
        {
            // and its an enemy flag on first child
            if (piece.GetComponent<Pieces>().isGreen != piece.transform.GetChild(0).GetComponent<Pieces>().isGreen)
            {
                CarryingEnemyFlag(0);
            } 
            else if (piece.transform.childCount == 2) // if there is a second child -> its an enemy flag
            {
                CarryingEnemyFlag(1);
            }

            void CarryingEnemyFlag(int childNumber)
            {
                if (stepsLimitCarryingEnemyFlag == 0)
                {
                    // set the piece to be child of the current field, in order to become the first child
                    piece.transform.SetParent(hittenFieldInfo.transform);
                    // set item to be child of the current field (leave item)
                    piece.transform.GetChild( childNumber ).SetParent(hittenFieldInfo.transform);
                    // move the item to the field (center position)
                    hittenFieldInfo.transform.GetChild(1).position = hittenFieldInfo.transform.position;
                    // reset steps
                    stepsLimitCarryingEnemyFlag = 3;
                }
                else
                {
                    stepsLimitCarryingEnemyFlag--;
                    Debug.Log(stepsLimitCarryingEnemyFlag);
                }
            }
        }

        // with a ship 
        if (hittenFieldInfo.transform.childCount > 0 && hittenFieldInfo.transform.GetChild(0).tag == "Ship")
        {
            // while another cap is on board, invalid move
            if (hittenFieldInfo.transform.GetChild(0).childCount > 0 && hittenFieldInfo.transform.GetChild(0).GetChild(0).tag == "Captain")
            {
                Debug.Log("Invalid Move");
                return false;
            }

            // while a flag is on board, catch it (set as child)
            if (hittenFieldInfo.transform.GetChild(0).childCount > 0 && hittenFieldInfo.transform.GetChild(0).GetChild(0).tag == "Item")
            {
                hittenFieldInfo.transform.GetChild(0).GetChild(0).SetParent(piece.transform);

                // center position of childs on captain
                for (int i = 0; i < piece.transform.childCount; i++)
                {
                    piece.transform.GetChild(i).position = piece.transform.position;
                }
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
