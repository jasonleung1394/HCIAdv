using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAction : MonoBehaviour
{
    // Start is called before the first frame update
    public Button syncUnityToRosBtn;

    void Start()
    {
        syncUnityToRosBtn.onClick.AddListener(SyncUnityToRos);
    }

    // Update is called once per frame
    void Update() { }

    void SyncUnityToRos()
    {
        JointStatePublisher jointStatePublisher = GetComponent<JointStatePublisher>();

        jointStatePublisher.PublishJointState();
    }
}
