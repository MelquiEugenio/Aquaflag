using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameObject piece;
    private bool isGreenTurn = true;
    private ArrayList possibleMoves = new ArrayList();
    private ArrayList fieldColors = new ArrayList();
    private string[,] board = {{ null,null, null, null, null, null, null, null, null, null, null, null, null }, //the board has an extra line each side
                               { null, null, null, null, null, "g1", "f1", null, null, null, null, null, null },
                               { null, null, null, "i1", "h1", "g2", "f2", "e1", null, null, null, null, null },
                               { null, null, "j1", "i2", "h2", "g3", "f3", "e2", null, null, null, null, null },
                               { null, "k1", "j2", "i3", "h3", "g4", "f4", "e3", "d1", null, null, null, null },
                               { null, "k2", "j3", "i4", "h4", "g5", "f5", "e4", "d2", "c1", null, null, null },
                               { null, null, "j4", "i5", "h5", "g6", "f6", "e5", "d3", "c2", null, null, null },
                               { null, null, "j5", "i6", "h6", "g7", "f7", "e6", "d4", "c3", "b1", null, null },
                               { null, null, null, "i7", "h7", "g8", "f8", "e7", "d5", "c4", "b2", null, null },
                               { null, null, null, "i8", "h8", "g9", "f9", "e8", "d6", "c5", "b3", "a1", null },
                               { null, null, null, null, "h9", "g10", "f10", "e9", "d7", "c6", "b4", "a2", null },
                               { null, null, null, null, null, "g11", "f11", "e10", "d8", "c7", "b5", null, null },
                               { null, null, null, null, null, "g12", "f12", "e11", "d9", "c8", null, null, null },
                               { null, null, null, null, null, null, "f13", "e12", null, null, null, null, null },
                               { null, null, null, null, null, null, null, null, null, null, null, null, null },};

    void Update()
    {
        MouseOver();
        //highlight pieces allowed to move
    }

    private void MouseOver()
    {
        //If ray casted from screen to the direction of mouse position (or touch) is hitting a field on the board
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, 25.0f, LayerMask.GetMask("Board")))
        {
            SelectOnTurn(hitInfo, isGreenTurn);
            MovePiece(hitInfo);
        }

        void SelectOnTurn(RaycastHit hittenFieldInfo, bool isGreenTurn)
        {
            //If hitten field has a Child (a Piece) and its not an Item
            if (hittenFieldInfo.transform.childCount > 0 && hittenFieldInfo.transform.GetChild(0).tag != "Item")
            {
                //If its green turn and piece is green
                if (isGreenTurn && hittenFieldInfo.transform.GetChild(0).gameObject.GetComponent<Pieces>().isGreen)
                {
                    SelectPiece(hittenFieldInfo);

                }//If its red turn and piece is red
                else if (!isGreenTurn && !hittenFieldInfo.transform.GetChild(0).gameObject.GetComponent<Pieces>().isGreen)
                {
                    SelectPiece(hittenFieldInfo);
                }
            }
        }

        void SelectPiece(RaycastHit hittenFieldInfo)
        {
            //On left Click
            if (Input.GetMouseButtonDown(0))
            {
                //Select Piece
                piece = hittenFieldInfo.transform.GetChild(0).gameObject;

                HighlightPossibleMoves(piece, hittenFieldInfo, Color.magenta);

                Debug.Log("Selected: " + piece.name + " at: " + hittenFieldInfo.transform.name);
            }
        }

        void HighlightPossibleMoves(GameObject piece, RaycastHit hittenFieldInfo, Color color)
        {
            //Repaint last fields
            for (int i = 0; i < possibleMoves.Count; i++)
                GameObject.Find((string) possibleMoves[i]).GetComponent<MeshRenderer>().material.color = (Color) fieldColors[i];

            //Clear fields
            possibleMoves.Clear();
            fieldColors.Clear();

            //Set possibleMoves
            possibleMoves = piece.GetComponent<Pieces>().PossibleMoves(board, hittenFieldInfo.transform.name);

            for (int i = 0; i < possibleMoves.Count; i++)
            {
                //save standart color of each field
                fieldColors.Add(GameObject.Find((string) possibleMoves[i]).GetComponent<MeshRenderer>().material.color);
                //highlight
                GameObject.Find((string) possibleMoves[i]).GetComponent<MeshRenderer>().material.color = color;
            }
        }

        void MovePiece(RaycastHit hittenFieldInfo)
        {
            //If there's a piece selected
            if(piece != null)
            {
                //If hitten field has no piece
                if (hittenFieldInfo.transform.childCount == 0)
                {
                    //if hitten field is a possible move
                    if (possibleMoves.Contains(hittenFieldInfo.transform.name))
                    {
                        //On left click
                        if (Input.GetMouseButtonDown(0))
                        {
                            //Move to field
                            piece.transform.position = hittenFieldInfo.transform.position;
                            //Set piece to be child of the clicked field
                            piece.transform.SetParent(hittenFieldInfo.transform);

                            Debug.Log("Moved: " + piece.name + " to: " + hittenFieldInfo.transform.name);

                            //Unselect piece
                            piece = null;
                            //Change turn
                            isGreenTurn = isGreenTurn ? false : true;

                            UnHighlightPossibleMoves();

                            //Debug.Log("Green Turn: "+ isGreenTurn);

                        }
                    }
                }
            }
        }

        void UnHighlightPossibleMoves()
        {
            //Repaint last fields
            for (int i = 0; i < possibleMoves.Count; i++)
                GameObject.Find(possibleMoves[i].ToString()).GetComponent<MeshRenderer>().material.color = (Color) fieldColors[i];
        }
    }
}