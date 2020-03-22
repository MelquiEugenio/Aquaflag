using System.Collections;

using UnityEngine;

public class Ship : Pieces
{
    public int stepsLimitCarryingEnemyFlag = 3;

    private ArrayList impossibleMovesForShip = new ArrayList
    {
        "f2",
        "d2",
        "f4",
        "h2",
        "c3",
        "e5",
        "f6",
        "g5",
        "i3",
        "b3",
        "f7",
        "j3",
        "c6",
        "e8",
        "f8",
        "g8",
        "i6",
        "d8",
        "f10",
        "h8",
        "f12"
    };

    public override ArrayList PossibleMoves(string[,] board, string fieldName, GameObject piece)
    {
        ArrayList possibleMoves = new ArrayList();
        
        void AddShipNeighbors(string field)
        {
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i, j] == field)
                    {
                        if (board[i, j + 1] != null && !impossibleMovesForShip.Contains(board[i, j + 1]) && !possibleMoves.Contains(board[i, j + 1]))
                            possibleMoves.Add(board[i, j + 1]);

                        if (board[i + 1, j + 1] != null && !impossibleMovesForShip.Contains(board[i + 1, j + 1]) && !possibleMoves.Contains(board[i + 1, j + 1]))
                            possibleMoves.Add(board[i + 1, j + 1]);

                        if (board[i + 1, j] != null && !impossibleMovesForShip.Contains(board[i + 1, j]) && !possibleMoves.Contains(board[i + 1, j]))
                            possibleMoves.Add(board[i + 1, j]);

                        if (board[i, j - 1] != null && !impossibleMovesForShip.Contains(board[i, j - 1]) && !possibleMoves.Contains(board[i, j - 1]))
                            possibleMoves.Add(board[i, j - 1]);

                        if (board[i - 1, j - 1] != null && !impossibleMovesForShip.Contains(board[i - 1, j - 1]) && !possibleMoves.Contains(board[i - 1, j - 1]))
                            possibleMoves.Add(board[i - 1, j - 1]);

                        if (board[i - 1, j] != null && !impossibleMovesForShip.Contains(board[i - 1, j]) && !possibleMoves.Contains(board[i - 1, j]))
                            possibleMoves.Add(board[i - 1, j]);

                        return;
                    }
                }
            }
        }

        AddShipNeighbors(fieldName);

        // add the neighbors of the neighbors (cause the Ship can move 2 fields per turn)
        int possibleMovesCount = possibleMoves.Count; //if we use 'possibleMoves.Count' to set the loop limit, it'll loop forever cause the ArrayList is growing
        for (int i = 0; i < possibleMovesCount; i++)
            AddShipNeighbors((string)possibleMoves[i]);

        // add the Captain possible moves if carrying the team Captain
        if (piece.transform.childCount > 0 && piece.transform.GetChild(0).tag == "Captain" &&
            piece.GetComponent<Pieces>().isGreen == piece.transform.GetChild(0).GetComponent<Pieces>().isGreen)
        {
            ArrayList captainPossibleMoves = piece.transform.GetChild(0).GetComponent<Pieces>().PossibleMoves(board, fieldName, piece.transform.GetChild(0).gameObject);

            // remove duplicates
            foreach (string field in captainPossibleMoves)
            {
                if (!possibleMoves.Contains(field))
                    possibleMoves.Add(field);
            }
        }

        // remove team ships collision
        if (piece.GetComponent<Pieces>().isGreen)
        {
            if (GameObject.Find("GreenShip1") != null) // if not destroyed
                possibleMoves.Remove(GameObject.Find("GreenShip1").transform.parent.name);

            if (GameObject.Find("GreenShip2") != null)
                possibleMoves.Remove(GameObject.Find("GreenShip2").transform.parent.name);
        }
        else
        {
            if (GameObject.Find("RedShip1") != null)
                possibleMoves.Remove(GameObject.Find("RedShip1").transform.parent.name);

            if (GameObject.Find("RedShip2") != null)
                possibleMoves.Remove(GameObject.Find("RedShip2").transform.parent.name);
        }

        // remove the immediate selected field
        possibleMoves.Remove(fieldName);

        return possibleMoves;
    }

    public override bool OnCollision(RaycastHit hittenFieldInfo, GameObject piece)
    {
        // while landing a team captain, change to Captain OnCollision rules only
        if (impossibleMovesForShip.Contains(hittenFieldInfo.transform.name))
            return piece.transform.GetChild(0).GetComponent<Pieces>().OnCollision(hittenFieldInfo, piece.transform.GetChild(0).gameObject);

        // with an enemy piece, not an item, destroy it
        if (hittenFieldInfo.transform.childCount > 0 &&
            hittenFieldInfo.transform.GetChild(0).GetComponent<Pieces>().isGreen != piece.GetComponent<Pieces>().isGreen &&
            hittenFieldInfo.transform.GetChild(0).tag != "Item")
        {
            // while enemy is carrying something
            if (hittenFieldInfo.transform.GetChild(0).childCount > 0)
            {
                // that is a team piece/item
                if (hittenFieldInfo.transform.GetChild(0).GetChild(0).GetComponent<Pieces>().isGreen == piece.GetComponent<Pieces>().isGreen)
                {
                    // if I'm carrying an enemy, kick enemy
                    if (piece.transform.childCount > 0 && piece.transform.GetChild(0).tag == "Captain" &&
                        piece.transform.GetChild(0).GetComponent<Pieces>().isGreen != piece.GetComponent<Pieces>().isGreen)
                        piece.transform.GetChild(0).SetParent(piece.transform.parent);

                    // move thing to ship
                    hittenFieldInfo.transform.GetChild(0).GetChild(0).position = piece.transform.position;
                    // parent
                    hittenFieldInfo.transform.GetChild(0).GetChild(0).SetParent(piece.transform);
                }
                else
                    hittenFieldInfo.transform.GetChild(0).GetChild(0).tag = "Destroyed";
            }

            hittenFieldInfo.transform.GetChild(0).tag = "Destroyed";
            Destroy(hittenFieldInfo.transform.GetChild(0).gameObject);
        }

        // with a flag
        if (hittenFieldInfo.transform.childCount > 0 && hittenFieldInfo.transform.GetChild(0).tag == "Item")
        {
            // while carrying a captain
            if (piece.transform.childCount > 0 && piece.transform.GetChild(0).tag == "Captain")
            {
                // set flag as child of captain
                hittenFieldInfo.transform.GetChild(0).SetParent(piece.transform.GetChild(0));

                // center position of childs on captain
                for (int i = 0; i < piece.transform.GetChild(0).childCount; i++)
                    piece.transform.GetChild(0).GetChild(i).position = piece.transform.GetChild(0).position;
            }
            else
            {
                // set the flag to be child of the ship
                hittenFieldInfo.transform.GetChild(0).SetParent(piece.transform);
                // center position
                piece.transform.GetChild(0).position = piece.transform.position;
            }
        }

        // while carrying an enemy flag, steps limit
        if (piece.transform.childCount > 0 &&
            piece.transform.GetChild(0).tag == "Item" &&
            piece.transform.GetChild(0).GetComponent<Pieces>().isGreen != piece.GetComponent<Pieces>().isGreen)
        {
            if (stepsLimitCarryingEnemyFlag == -1)
            {
                // set item to be child of the current field (leave item)
                piece.transform.GetChild(0).SetParent(piece.transform.parent);
                // reset steps
                stepsLimitCarryingEnemyFlag = 3;
            }
            else
            {
                stepsLimitCarryingEnemyFlag--;
                Debug.Log("Ship can move enemy flag more: " + (stepsLimitCarryingEnemyFlag + 1) + " steps");
            }
        }

        // with a team captain,
        if (hittenFieldInfo.transform.childCount > 0 && hittenFieldInfo.transform.GetChild(0).tag == "Captain" &&
            hittenFieldInfo.transform.GetChild(0).GetComponent<Pieces>().isGreen == piece.GetComponent<Pieces>().isGreen)
        {
            // while carrying enemy captain, kick enemy
            if (piece.transform.childCount > 0 && piece.transform.GetChild(0).tag == "Captain")
                piece.transform.GetChild(0).SetParent(piece.transform.parent);

            // while carrying a flag
            if (piece.transform.childCount > 0 && piece.transform.GetChild(0).tag == "Item")
            {
                // set flag to be child of captain
                piece.transform.GetChild(0).SetParent(hittenFieldInfo.transform.GetChild(0));

                // center position of childs on captain
                for (int i = 0; i < hittenFieldInfo.transform.GetChild(0).childCount; i++)
                    hittenFieldInfo.transform.GetChild(0).GetChild(i).position = hittenFieldInfo.transform.GetChild(0).position;
            }

            // rescue it from drowning (set as child of the Ship)
            hittenFieldInfo.transform.GetChild(0).SetParent(piece.transform);
            // center position
            piece.transform.GetChild(0).position = piece.transform.position;
        }

        //---------------

        // Move to field
        piece.transform.position = hittenFieldInfo.transform.position;
        // Set piece to be child of the clicked field
        piece.transform.SetParent(hittenFieldInfo.transform);

        Debug.Log("Moved: " + piece.name + " to: " + hittenFieldInfo.transform.name);

        // while carrying a captain also check captain collisions
        if (piece.transform.childCount > 0 && piece.transform.GetChild(0).tag == "Captain")
            return piece.transform.GetChild(0).GetComponent<Pieces>().OnCollision(hittenFieldInfo, piece.transform.GetChild(0).gameObject);

        return true;
    }
}
