using UnityEngine;

public class StationInteraction : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Ban"))
        {
            GameManagerScript.instance.currentState = GameState.MotorSelesai;
            Destroy(other.gameObject);
        }
    }
}