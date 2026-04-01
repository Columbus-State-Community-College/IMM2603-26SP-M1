using UnityEngine;

public class ChangingSignMessage : MonoBehaviour
{
    public TextMesh SignMessage;
    string hexColor = "#A8EA8C";
    
    void Start()
    {
        int messageValue = Random.Range(1, 5);
        Debug.Log("The number this time is: " + messageValue);

        if(messageValue == 1)
        {
            if (ColorUtility.TryParseHtmlString(hexColor, out Color newColor))
            {
                SignMessage.color = newColor;
            }

            SignMessage.text = "My Property! - Granny";
        }

        if(messageValue == 2)
        {
            hexColor = "#FF0000";
            if (ColorUtility.TryParseHtmlString(hexColor, out Color newColor))
            {
                SignMessage.color = newColor;
            }

            SignMessage.text = "Granny Stinks! - Enemy";
        }

        else if (messageValue == 3 || messageValue == 4 || messageValue == 5) 
        {
            hexColor = "#FFFFFF";
            if (ColorUtility.TryParseHtmlString(hexColor, out Color newColor))
            {
                SignMessage.color = newColor;
            }

            SignMessage.text = "Lunchtime? Yay! - ???";
        }
    }
}
