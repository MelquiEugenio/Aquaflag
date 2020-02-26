using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameObject field;
    private Color stdFieldColor;

    void Start()
    {
        field = GameObject.Find("a1");
        stdFieldColor = field.GetComponent<MeshRenderer>().material.color;
    }

    void Update()
    {
        HighlightMouseOverField();

        //if its my turn
    }

    private void HighlightMouseOverField()
    {
        //Repaint last field
        field.GetComponent<MeshRenderer>().material.color = stdFieldColor;

        //Make a ray from main camera to the board
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //If the ray is hitting an obj
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 25.0f))
        {
            //If the obj is a Piece
            if (hitInfo.transform.parent.name != "Board")
            {
                //Send the field which this piece is over to highlight
                HighlightField(hitInfo.transform.parent.gameObject);
            }
            else {
                //Send the field to highlight
                HighlightField(hitInfo.transform.gameObject);
            }
        }

        void HighlightField(GameObject hitField)
        {
            //Set field
            field = hitField;
            //Save its standart color
            stdFieldColor = field.GetComponent<MeshRenderer>().material.color;
            //Highlight (paint white)
            field.GetComponent<MeshRenderer>().material.color = Color.white;
        }
    }
}
