using UnityEngine;

public class TitikCongkelBan : MonoBehaviour
{
    void OnMouseOver()
    {
        if (StationInteraction.instance != null)
        {
            if (Input.GetMouseButton(0))
            {
                StationInteraction.instance.ProsesTahanTitik(this.gameObject);
            }
            else
            {
                StationInteraction.instance.ResetProgressCongkel();
            }
        }
    }

    void OnMouseExit()
    {
        if (StationInteraction.instance != null)
        {
            StationInteraction.instance.ResetProgressCongkel();
        }
    }

    void OnMouseUp()
    {
        if (StationInteraction.instance != null)
        {
            StationInteraction.instance.ResetProgressCongkel();
        }
    }
}