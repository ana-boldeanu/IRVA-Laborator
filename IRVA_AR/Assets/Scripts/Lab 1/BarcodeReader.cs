using TMPro;
using UnityEngine;
using Vuforia;

public class BarcodeReader : MonoBehaviour
{
    public TextMeshProUGUI resultQR;

    BarcodeBehaviour mBarcodeBehaviour;

    void Start()
    {
        mBarcodeBehaviour = GetComponent<BarcodeBehaviour>();
    }

    void Update()
    {
        if (mBarcodeBehaviour != null && mBarcodeBehaviour.InstanceData != null)
        {
            Debug.Log(mBarcodeBehaviour.InstanceData.Text);
            resultQR.text = mBarcodeBehaviour.InstanceData.Text;
        }
    }
}