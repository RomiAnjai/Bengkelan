using UnityEngine;
using System.Collections;

public enum GameState
{
    MotorBelumDatang,
    MotorMasukBengkel,
    MotorSedangServis,
    MotorSelesai
}

public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript instance;
    public GameState currentState;
    public GameObject SpawnObjek;
    public Transform posisiSpawn;
    public Transform posisiParkir;
    public float jedaSebelumJalan = 2f;
    public float waktuPerjalanan = 3f;
    public TutorialHighlight[] tutorLight;

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
                if (SpawnObjek != null && posisiSpawn != null && posisiParkir != null)
                {
                    GameObject motorBaru = Instantiate(SpawnObjek, posisiSpawn.position, posisiSpawn.rotation);
                    currentState = GameState.MotorMasukBengkel;
                    StartCoroutine(ProsesMotorParkir(motorBaru));
                }
                break;
            case GameState.MotorMasukBengkel:
                break;
            case GameState.MotorSedangServis:
                break;
            case GameState.MotorSelesai:
                ObjectiveHUD.instance.SetBalance(60000);
                break;
        }
    }

    private IEnumerator ProsesMotorParkir(GameObject motor)
    {
        yield return new WaitForSeconds(jedaSebelumJalan);

        float waktuBerjalan = 0f;
        Vector3 posisiAwal = motor.transform.position;
        Quaternion rotasiAwal = motor.transform.rotation;

        while (waktuBerjalan < waktuPerjalanan)
        {
            waktuBerjalan += Time.deltaTime;
            float persentase = waktuBerjalan / waktuPerjalanan;
            float smoothT = Mathf.SmoothStep(0f, 1f, persentase);

            motor.transform.position = Vector3.Lerp(posisiAwal, posisiParkir.position, smoothT);
            motor.transform.rotation = Quaternion.Lerp(rotasiAwal, posisiParkir.rotation, smoothT);

            yield return null;
        }

        motor.transform.position = posisiParkir.position;
        motor.transform.rotation = posisiParkir.rotation;

        SequenceService.instance.MulaiServis(0);
        currentState = GameState.MotorSedangServis;
    }

    public void MulaiServis()
    {
        InteraksiMotor.instance.FokusKeMotor();

        MekanikController mc = FindAnyObjectByType<MekanikController>();
        if (mc != null)
        {
            mc.KunciKursor(true);
        }
    }
}