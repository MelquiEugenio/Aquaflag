using System.Collections;
using UnityEngine;

public class Ship : Pieces
{
    public override ArrayList PossibleMoves(string[,] board, string fieldName)
    {
        ArrayList possibleMoves = new ArrayList();
        ArrayList impossibleMovesForShip = new ArrayList
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

        void FindNeighbors(string[,] theBoard, string theFieldName)
        {
            for (int i = 0; i < theBoard.GetLength(0); i++)
            {
                for (int j = 0; j < theBoard.GetLength(1); j++)
                {
                    if (theBoard[i, j] == theFieldName)
                    {
                        if (theBoard[i, j + 1] != null && !impossibleMovesForShip.Contains(theBoard[i, j + 1]) && !possibleMoves.Contains(theBoard[i, j + 1]))
                            possibleMoves.Add(theBoard[i, j + 1]);

                        if (theBoard[i + 1, j + 1] != null && !impossibleMovesForShip.Contains(theBoard[i + 1, j + 1]) && !possibleMoves.Contains(theBoard[i + 1, j + 1]))
                            possibleMoves.Add(theBoard[i + 1, j + 1]);

                        if (theBoard[i + 1, j] != null && !impossibleMovesForShip.Contains(theBoard[i + 1, j]) && !possibleMoves.Contains(theBoard[i + 1, j]))
                            possibleMoves.Add(theBoard[i + 1, j]);

                        if (theBoard[i, j - 1] != null && !impossibleMovesForShip.Contains(theBoard[i, j - 1]) && !possibleMoves.Contains(theBoard[i, j - 1]))
                            possibleMoves.Add(theBoard[i, j - 1]);

                        if (theBoard[i - 1, j - 1] != null && !impossibleMovesForShip.Contains(theBoard[i - 1, j - 1]) && !possibleMoves.Contains(theBoard[i - 1, j - 1]))
                            possibleMoves.Add(theBoard[i - 1, j - 1]);

                        if (theBoard[i - 1, j] != null && !impossibleMovesForShip.Contains(theBoard[i - 1, j]) && !possibleMoves.Contains(theBoard[i - 1, j]))
                            possibleMoves.Add(theBoard[i - 1, j]);

                        return;
                    }
                }
            }
        }

        FindNeighbors(board, fieldName);

        int possibleMovesCount = possibleMoves.Count; //if we use 'possibleMoves.Count' to set the loop limit, it'll loop forever cause its an ArrayList

        // find and add the second possible fields for each last added moves (cause Ship can move 2 fields per turn)
        for (int i = 0; i < possibleMovesCount; i++)
        {
            FindNeighbors(board, (string) possibleMoves[i]);
        }

        // remove the immediate selected field
        possibleMoves.Remove(fieldName);

        return possibleMoves;
    }

    public override bool OnCollision(RaycastHit hittenFieldInfo, GameObject piece)
    {
        // with an enemy piece, destroy it
        if (hittenFieldInfo.transform.childCount > 0 &&
            hittenFieldInfo.transform.GetChild(0).GetComponent<Pieces>().isGreen != piece.GetComponent<Pieces>().isGreen)
            Destroy(hittenFieldInfo.transform.GetChild(0).gameObject);

        // with a team ship, invalid move
        if (hittenFieldInfo.transform.childCount > 0 && hittenFieldInfo.transform.GetChild(0).tag == "Ship" &&
            hittenFieldInfo.transform.GetChild(0).GetComponent<Pieces>().isGreen == piece.GetComponent<Pieces>().isGreen)
        {
            Debug.Log("Invalid Move");
            return false;
        }

        // with a team captain
        if (hittenFieldInfo.transform.childCount > 0 && hittenFieldInfo.transform.GetChild(0).tag == "Captain" &&
            hittenFieldInfo.transform.GetChild(0).GetComponent<Pieces>().isGreen == piece.GetComponent<Pieces>().isGreen)
        {
            // while carrying enemy captain, kick enemy
            if (piece.transform.childCount > 0 && piece.transform.GetChild(0).tag == "Captain")
            {
                piece.transform.GetChild(0).SetParent(piece.transform.parent);
            }

            // while carrying a flag
            if (piece.transform.childCount > 0 && piece.transform.GetChild(0).tag == "Item")
            {
                // set flag to be child of captain
                piece.transform.GetChild(0).SetParent(hittenFieldInfo.transform.GetChild(0));

                // center position of childs on captain
                for (int i = 0; i < hittenFieldInfo.transform.GetChild(0).childCount; i++)
                {
                    hittenFieldInfo.transform.GetChild(0).GetChild(i).position = hittenFieldInfo.transform.GetChild(0).position;
                }
            }

            // rescue it from drowning (set as child of the Ship)
            hittenFieldInfo.transform.GetChild(0).SetParent(piece.transform);
            // center position
            piece.transform.GetChild(0).position = piece.transform.position;
        }

        // with a team flag
        if (hittenFieldInfo.transform.childCount > 0 && hittenFieldInfo.transform.GetChild(0).tag == "Item" &&
            hittenFieldInfo.transform.GetChild(0).GetComponent<Pieces>().isGreen == piece.GetComponent<Pieces>().isGreen)
        {
            // while carrying a captain
            if (piece.transform.childCount > 0 && piece.transform.GetChild(0).tag == "Captain")
            {
                // set flag as child of captain
                hittenFieldInfo.transform.GetChild(0).SetParent(piece.transform.GetChild(0));

                // center position of childs on captain
                for (int i = 0; i < piece.transform.GetChild(0).childCount; i++)
                {
                    piece.transform.GetChild(0).GetChild(i).position = piece.transform.GetChild(0).position;
                }
            }
            else
            {
                // set flag to be child of the ship
                hittenFieldInfo.transform.GetChild(0).SetParent(piece.transform);
                // center position
                piece.transform.GetChild(0).position = piece.transform.position;
            }
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
