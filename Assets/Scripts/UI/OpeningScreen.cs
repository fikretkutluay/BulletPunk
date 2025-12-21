using UnityEngine;

public class AnimationUIHandler : MonoBehaviour
{
    public GameObject playerBars;
    public GameObject controlButtons;
    // This function will be called by the Animation Event
    public void EnablePanel()
    {
        if (playerBars && controlButtons != null)
        {
            controlButtons.SetActive(true);
            playerBars.SetActive(true);
            Destroy(gameObject);
        }
    }
}