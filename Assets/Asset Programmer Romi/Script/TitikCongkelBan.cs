using UnityEngine;

public class TitikCongkelBan : MonoBehaviour
{
    void OnMouseDown()
    {
        if (StationInteraction.instance != null)
        {
            StationInteraction.instance.TitikDitekan(this.gameObject);
        }
    }
}