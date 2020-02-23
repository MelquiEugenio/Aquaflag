using System.Collections;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    private static GameObject field;
    private Color fieldColor;

    void Start()
    {
        field = GameObject.Find("a1");
        fieldColor = field.GetComponent<MeshRenderer>().material.color;
    }

    void Update()
    {
        //Reset color
        field.GetComponent<MeshRenderer>().material.color = fieldColor;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //If hitting field
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            //New field
            field = hitInfo.collider.transform.gameObject;
            //Set color
            fieldColor = field.GetComponent<MeshRenderer>().material.color;
            //Highlight
            field.GetComponent<MeshRenderer>().material.color = Color.white;
        }
    }
}
