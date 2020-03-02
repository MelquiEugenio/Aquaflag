using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameObject field, piece;
    private Color fieldColor;
    private bool isGreenTurn = true;

    private void Start()
    {
        field = GameObject.Find("a1");
        fieldColor = field.GetComponent<MeshRenderer>().material.color;
    }

    //loops every frame
    void Update()
    {
        MouseOver();
    }

    private void MouseOver()
    {
        //Repaint last saved field
        field.GetComponent<MeshRenderer>().material.color = fieldColor;

        //If ray at the mouse position is hitting a field on the board
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, 25.0f, LayerMask.GetMask("Board")))
        {
            HighlightField(hitInfo, Color.white);
            SelectOnTurn(hitInfo, isGreenTurn);
            MovePiece(hitInfo);
        }

        void HighlightField(RaycastHit hittenFieldInfo, Color color)
        {
            //Save field
            field = hittenFieldInfo.transform.gameObject;
            //Save field Color
            fieldColor = field.GetComponent<MeshRenderer>().material.color;
            //Highlight (paint in the color)
            field.GetComponent<MeshRenderer>().material.color = color;
        }

        void SelectOnTurn(RaycastHit hittenFieldInfo, bool isGreenTurn)
        {
            //If hitten field has a Child (a Piece)
            if (hittenFieldInfo.transform.childCount > 0)
            {
                //If its green turn and piece is green
                if (isGreenTurn && hittenFieldInfo.transform.GetChild(0).gameObject.tag == "Green")
                {
                    SelectPiece(hittenFieldInfo);

                }//If its red turn and piece is red
                else if (!isGreenTurn && hittenFieldInfo.transform.GetChild(0).gameObject.tag != "Green")
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

                Debug.Log("Selected: " + piece.name + " at: " + hittenFieldInfo.transform.name);
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

                        Debug.Log("Green Turn: "+ isGreenTurn);
                    }
                }
            }
        }
    }
}