using UnityEngine;

public class Rotator : MonoBehaviour
{
    // Saniyede kaç derece dönecek? (Y ekseninde 30 derece)
    public Vector3 rotateSpeed = new Vector3(0, 30, 0);

    void Update()
    {
        // Objeyi sürekli döndür
        transform.Rotate(rotateSpeed * Time.deltaTime);
    }
}