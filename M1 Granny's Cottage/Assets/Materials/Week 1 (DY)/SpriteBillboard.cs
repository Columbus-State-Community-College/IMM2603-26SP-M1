using UnityEngine;
//Tutorial credit: Paper Mouse Games https://youtu.be/FjJJ_I9zqJo?si=RMMxTl-6_O2mIV9Z
public class SpriteBillboard : MonoBehaviour
{

    [SerializeField] bool freezXZAxis = true;
    // Update is called once per frame
    void Update()
    {
        if (freezXZAxis)
        {
             transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
        }
        else
        {
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}
