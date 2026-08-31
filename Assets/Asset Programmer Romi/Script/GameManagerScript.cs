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

    private Rigidbody rbBan;

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
                    AmbilDataBan();
                    currentState = GameState.MotorSedangServis;
                }
                break;

            case GameState.MotorSedangServis:
                if (rbBan != null && Input.GetKeyDown(KeyCode.E))
                {
                    rbBan.isKinematic = false;
                    rbBan.useGravity = true;
                }
                break;

            case GameState.MotorSelesai:
                break;
        }
    }

    void AmbilDataBan()
    {
        GameObject ban = GameObject.FindWithTag("Ban");
        if (ban != null)
        {
            rbBan = ban.GetComponent<Rigidbody>();
        }
    }
}