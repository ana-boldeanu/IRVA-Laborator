
using UnityEngine;

public class PokemonController : MonoBehaviour
{
    public bool isSelected = false;
    public int mood = 0;
    public int level = 1;
    public int experience = 30;
    public Vector3 localHeadPosition = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in this.transform)
        {
            if (child.tag == "Head Model")
                localHeadPosition = child.localPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
