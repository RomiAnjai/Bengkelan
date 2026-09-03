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
    public Transform posisiSpawn;

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
                Vector3 targetPosisiSpawn = posisiSpawn.position;
                if (SpawnObjek != null)
                {
                    Instantiate(SpawnObjek, targetPosisiSpawn, Quaternion.identity);
                    SequenceService.instance.MulaiServis(0);
                    currentState = GameState.MotorSedangServis;
                }
                break;

            case GameState.MotorSedangServis:
                break;

            case GameState.MotorSelesai:
                break;
        }
    }
}