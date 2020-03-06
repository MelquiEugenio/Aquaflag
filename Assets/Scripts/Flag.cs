public class Flag : Pieces
{
    public float yRotationSpeed = -2f;

    void Update()
    {
        transform.Rotate(0, yRotationSpeed, 0);
    }
}