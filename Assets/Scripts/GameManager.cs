using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameObject piece, lastPiece, lastField;
    private ArrayList possibleMoves = new ArrayList();
    private ArrayList stdFieldColors = new ArrayList();
    private ArrayList lastMoves = new ArrayList();
    private ArrayList movesLog = new ArrayList();
    private bool isGreenTurn = true;
    private int greenCountdown = 15;
    private int redCountdown = 15;
    private bool endGame = false;
    private readonly string[,] board = {{ null,null, null, null, null, null, null, null, null, null, null, null, null }, // added an extra line each side
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
        if (!endGame)
            MouseOverBoard();
        else
            GameObject.Find("UndoButton").GetComponent<Button>().interactable = false;

        // Handling back pressed on Android
        if (Application.platform == RuntimePlatform.Android)
        {
            // Check if Back was pressed this frame
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("Returning");
                SceneManager.LoadScene(0);
            }
        }
    }

    void MouseOverBoard()
    {
        //If ray casted from screen to the direction of mouse position (or touch) is hitting a field on the board
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, 25.0f, LayerMask.GetMask("Board")))
        {
            SelectOnTurn(hitInfo, isGreenTurn);
            MovePiece(hitInfo);
        }

        void SelectOnTurn(RaycastHit hittenFieldInfo, bool isGreenTurn)
        {
            // If field has not an Item
            if (hittenFieldInfo.transform.childCount > 0 && hittenFieldInfo.transform.GetChild(0).tag != "Item")
            {
                if (isGreenTurn && hittenFieldInfo.transform.GetChild(0).GetComponent<Pieces>().isGreen) // If it's green turn and the piece is green
                    SelectPiece(hittenFieldInfo, false);
                else if (isGreenTurn && hittenFieldInfo.transform.GetChild(0).childCount > 0 &&
                    hittenFieldInfo.transform.GetChild(0).GetChild(0).GetComponent<Pieces>().isGreen &&
                    hittenFieldInfo.transform.GetChild(0).GetChild(0).tag != "Item") // If it's green turn and the sub-piece is green (GreenCaptain on RedShip), and it's not an Item
                    SelectPiece(hittenFieldInfo, true);
                else if (!isGreenTurn && !hittenFieldInfo.transform.GetChild(0).GetComponent<Pieces>().isGreen) // If it's red turn and the piece is red
                    SelectPiece(hittenFieldInfo, false);
                else if (!isGreenTurn && hittenFieldInfo.transform.GetChild(0).childCount > 0 &&
                    !hittenFieldInfo.transform.GetChild(0).GetChild(0).GetComponent<Pieces>().isGreen &&
                    hittenFieldInfo.transform.GetChild(0).GetChild(0).tag != "Item") // If it's red turn and the sub-piece is red (RedCaptain on GreenShip), and it's not an Item
                    SelectPiece(hittenFieldInfo, true);
            }
        }

        void SelectPiece(RaycastHit hittenFieldInfo, bool isGrandChild)
        {
            // On left Click
            if (Input.GetMouseButtonDown(0))
            {
                // Do not select again if over a possible move (let same color pieces to collide)
                if (possibleMoves.Contains(hittenFieldInfo.transform.name))
                    return;

                // Select Piece
                piece = isGrandChild ? hittenFieldInfo.transform.GetChild(0).GetChild(0).gameObject : hittenFieldInfo.transform.GetChild(0).gameObject;

                if (DeselectPieceIfClickedAgain())
                    return;

                Debug.Log("Selected: " + piece.name + " at: " + hittenFieldInfo.transform.name);

                // Save last field
                lastField = hittenFieldInfo.transform.gameObject;

                SetAndHighlightPossibleMoves(piece, hittenFieldInfo, Color.magenta);
            }

            bool DeselectPieceIfClickedAgain()
            {
                if (piece == lastPiece)
                {
                    UndoAll();
                    Debug.Log("Deselected");

                    return true;
                }
                else
                {
                    lastPiece = piece;

                    return false;
                }
            }
        }

        void SetAndHighlightPossibleMoves(GameObject piece, RaycastHit hittenFieldInfo, Color color)
        {
            // Repaint last fields
            for (int i = 0; i < possibleMoves.Count; i++)
                GameObject.Find((string)possibleMoves[i]).GetComponent<MeshRenderer>().material.color = (Color)stdFieldColors[i];

            // Clear arrays
            possibleMoves.Clear();
            stdFieldColors.Clear();

            // Set possible moves
            possibleMoves = piece.GetComponent<Pieces>().PossibleMoves(board, hittenFieldInfo.transform.name, piece);

            for (int i = 0; i < possibleMoves.Count; i++)
            {
                // Save standart color of each field
                stdFieldColors.Add(GameObject.Find((string)possibleMoves[i]).GetComponent<MeshRenderer>().material.color);
                // Highlight
                GameObject.Find((string)possibleMoves[i]).GetComponent<MeshRenderer>().material.color = color;
            }
        }

        void MovePiece(RaycastHit hittenFieldInfo)
        {
            // If field is a possible move
            if (possibleMoves.Contains(hittenFieldInfo.transform.name))
            {
                // On left click
                if (Input.GetMouseButtonDown(0))
                {
                    // Check collision events
                    if (piece.GetComponent<Pieces>().OnCollision(hittenFieldInfo, piece))
                    {
                        GameRules();

                        LogMoves();

                        // Change turn
                        isGreenTurn = isGreenTurn ? false : true;
                        //Debug.Log("Green Turn: " + isGreenTurn);

                        UndoAll();
                    }
                    else
                        UndoAll();
                }
            }
            else if (piece != null) // not a possible move, while piece selected
            {
                // excluding the immediate field where the selected piece is, any field is an invalid move
                if (hittenFieldInfo.transform.name != piece.transform.parent.name &&
                    hittenFieldInfo.transform.name != piece.transform.parent.parent.name)
                {
                    //On left click
                    if (Input.GetMouseButtonDown(0))
                    {
                        UndoAll();
                        Debug.Log("Invalid Move");
                    }
                }
            }

            void LogMoves()
            {
                // if the piece moved is a ship and the ship didn't move
                if (piece.tag == "Ship" && hittenFieldInfo.transform.GetChild(0).tag != "Ship")
                {
                    // add the piece which has moved (the captain)
                    movesLog.Insert(0, hittenFieldInfo.transform.GetChild(0).gameObject);
                    // add the last field (where the piece came from)
                    movesLog.Insert(1, lastField);

                    Debug.Log("Called");
                }
                else
                {
                    movesLog.Insert(0, lastPiece);
                    movesLog.Insert(1, lastField);
                }

                foreach (GameObject i in movesLog)
                    Debug.Log(i.name);

                // let return button interactivity
                GameObject.Find("UndoButton").GetComponent<Button>().interactable = true;
            }
        }

        void GameRules()
        {
            Draw();
            Loss();
            Win();

            void Win()
            {
                // WIN Condition: To conquer the enemy capital with the flag or to destroy the enemy captain/flag
                if (GameObject.Find("f12").transform.childCount > 0 &&
                    GameObject.Find("f12").transform.GetChild(0).name == "GreenCaptain" &&
                    GameObject.Find("f12").transform.GetChild(0).childCount > 0 &&
                    GameObject.Find("f12").transform.GetChild(0).GetChild(0).name == "GreenFlag" ||
                    GameObject.Find("RedCaptain").tag == "Destroyed" ||
                    GameObject.Find("RedFlag").tag == "Destroyed")
                {
                    Debug.Log("GREEN PLAYER WON!");
                    endGame = true;
                }
                else if (GameObject.Find("f2").transform.childCount > 0 &&
                         GameObject.Find("f2").transform.GetChild(0).name == "RedCaptain" &&
                         GameObject.Find("f2").transform.GetChild(0).childCount > 0 &&
                         GameObject.Find("f2").transform.GetChild(0).GetChild(0).name == "RedFlag" ||
                         GameObject.Find("GreenCaptain").tag == "Destroyed" ||
                         GameObject.Find("GreenFlag").tag == "Destroyed")
                {
                    Debug.Log("RED PLAYER WON!");
                    endGame = true;
                }
            }

            void Loss()
            {
                // LOSS Condition: 15 (fifteen) turns without moving the captain
                if (piece.transform.name == "GreenCaptain" ||
                    piece.transform.childCount > 0 && piece.transform.GetChild(0).name == "GreenCaptain")
                    greenCountdown = 15;
                else if (piece.transform.tag == "RedCaptain" ||
                    piece.transform.childCount > 0 && piece.transform.GetChild(0).name == "RedCaptain")
                    redCountdown = 15;
                else if (piece.transform.tag == "Ship" && piece.transform.GetComponent<Pieces>().isGreen)
                    greenCountdown--;
                else if (piece.transform.tag == "Ship" && !piece.transform.GetComponent<Pieces>().isGreen)
                    redCountdown--;

                if (redCountdown == 0)
                {
                    Debug.Log("GREEN PLAYER WON!");
                    endGame = true;
                }
                else if (greenCountdown == 0)
                {
                    Debug.Log("RED PLAYER WON!");
                    endGame = true;
                }
            }

            void Draw()
            {
                //DRAW Condition: 3 (three) turns with sequentially repeated moves by both players

                // 0 1  2 3  4 5  6 7  8 9
                // 1-2, x-x, 1-2, x-x, 1-2

                lastMoves.Add(piece.name + " to: " + piece.transform.parent.name);

                if (lastMoves.Count == 10)
                {
                    if (lastMoves[0].ToString() == lastMoves[4].ToString() && lastMoves[4].ToString() == lastMoves[8].ToString() &&
                       lastMoves[1].ToString() == lastMoves[5].ToString() && lastMoves[5].ToString() == lastMoves[9].ToString())
                    {
                        Debug.Log("DRAW!");
                        endGame = true;
                    }
                    else
                        lastMoves.RemoveAt(0);
                }
            }
        }
    }

    void UndoAll()
    {
        // Repaint last fields
        for (int i = 0; i < possibleMoves.Count; i++)
            GameObject.Find(possibleMoves[i].ToString()).GetComponent<MeshRenderer>().material.color = (Color)stdFieldColors[i];

        // Unselect Piece
        piece = null;
        lastPiece = null;

        // Clear arrays
        possibleMoves.Clear();
        stdFieldColors.Clear();
    }


    // Game Scene UI

    public void UndoLastMove()
    {
        GameObject piece, field;
        piece = (GameObject) movesLog[0];
        field = (GameObject) movesLog[1];

        // return move
        if (field.transform.childCount > 0 && field.transform.GetChild(0).tag == "Ship")
        {
            piece.transform.position = field.transform.GetChild(0).position;
            piece.transform.SetParent(field.transform.GetChild(0));
            movesLog.RemoveAt(0);
            movesLog.RemoveAt(0);
        }
        else if (field.transform.childCount > 0 && field.transform.GetChild(0).tag == "Captain")
        {
            //more to do
            // parent captain
            field.transform.GetChild(0).SetParent(piece.transform);
            // Move to field
            piece.transform.position = field.transform.position;
            // Set piece to be child of the clicked field
            piece.transform.SetParent(field.transform);
        }


        piece.transform.position = field.transform.position;
        // re-parent
        piece.transform.SetParent(field.transform);
        // remove moves returned
        movesLog.RemoveAt(0);
        movesLog.RemoveAt(0);

        // return turn
        isGreenTurn = piece.transform.GetComponent<Pieces>().isGreen ? true : false;

        if (movesLog.Count == 0)
            GameObject.Find("UndoButton").GetComponent<Button>().interactable = false;

        UndoAll();
    }

    public void RestartGameScene()
    {
        Debug.Log("Restarting");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMainMenu()
    {
        Debug.Log("Returning");
        SceneManager.LoadScene(0);
    }
}

