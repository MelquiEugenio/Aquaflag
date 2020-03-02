using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameObject field, piece;
    private Color stdFieldColor;

    void Start()
    {
        field = GameObject.Find("a1");
        stdFieldColor = field.GetComponent<MeshRenderer>().material.color;
    }

    //loops every frame
    void Update()
    {
        MouseOver();
    }

    private void MouseOver()
    {
        //Repaint last saved field
        field.GetComponent<MeshRenderer>().material.color = stdFieldColor;

        //Make a ray from main camera to the board
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //If the ray is hitting a field on the board
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 25.0f, LayerMask.GetMask("Board")))
        {
            HighlightField(hitInfo.transform.gameObject, Color.white);
            SelectPiece(hitInfo);
            MovePiece(hitInfo);
        }

        void HighlightField(GameObject hittenField, Color color)
        {
            //Save field
            field = hittenField;
            //Save field Color
            stdFieldColor = field.GetComponent<MeshRenderer>().material.color;
            //Highlight (paint in the color)
            field.GetComponent<MeshRenderer>().material.color = color;
        }

        void SelectPiece(RaycastHit fieldHittenInfo)
        {
            //If hitten field has a Child (a Piece)
            if (fieldHittenInfo.transform.childCount > 0)
            {
                //On left Click
                if (Input.GetMouseButtonDown(0))
                {
                    //Select Piece
                    piece = fieldHittenInfo.transform.GetChild(0).gameObject;

                    Debug.Log("Selected: " + piece.name + " at: " + fieldHittenInfo.transform.name);
                }
            }
        }

        void MovePiece(RaycastHit fieldHittenInfo)
        {
            //If there's a piece selected
            if(piece != null)
            {
                //If hitten field has no piece
                if (fieldHittenInfo.transform.childCount == 0)
                {
                    //On left click
                    if (Input.GetMouseButtonDown(0))
                    {
                        //Move to field
                        piece.transform.position = fieldHittenInfo.transform.position;
                        //Set piece to be child of the clicked field
                        piece.transform.SetParent(fieldHittenInfo.transform);

                        Debug.Log("Moved: " + piece.name + " to: " + fieldHittenInfo.transform.name);

                        //Unselect piece
                        piece = null;
                    }
                }
            }
        }
    }
}