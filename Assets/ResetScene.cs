using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;


public class ResetScene : MonoBehaviour
{

    public GameObject toggleState;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (toggleState.activeSelf)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

    }

    public void OnTouchStarted(HandTrackingInputEventData eventData)
    {
        // Handle the touch started event
        Debug.Log("Touch Started!");
    }

    public void OnFocusEnter()
    {
        Debug.Log("111");

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
