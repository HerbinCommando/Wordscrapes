using UnityEngine;

public class UIBackgrounds : MonoBehaviour
{
    public void Shuffle()
    {
        for (int i = 0; i < transform.childCount; ++i)
            transform.GetChild(i).gameObject.SetActive(false);

        transform.GetChild(Random.Range(0, transform.childCount)).gameObject.SetActive(true);
    }
}
