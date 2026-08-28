using UnityEngine;

public enum GameState
{
    MotorBelumDatang,
    MotorSedangServis,
    MotorSelesai
}

public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript instance;
    public GameState currentState;
    public GameObject SpawnObjek;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentState = GameState.MotorBelumDatang;
    }

    void Update()
    {
        switch (currentState)
        {
            case GameState.MotorBelumDatang:
                Vector3 position = new Vector3(0f, 0f, 0f);
                Instantiate(SpawnObjek, position, Quaternion.identity);
                currentState = GameState.MotorSedangServis;
                break;

            case GameState.MotorSedangServis:
                break;

            case GameState.MotorSelesai:
                break;
        }
    }
}