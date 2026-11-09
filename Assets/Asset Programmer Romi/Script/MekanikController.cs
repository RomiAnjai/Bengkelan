using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MekanikController : MonoBehaviour
{
    public float speed = 5f;
    public float mouseSensitivity = 200f;
    public Transform playerCamera;

    public bool bisaGerak = true;
    public bool bisaRotasiKamera = true;

    [Header("Audio Langkah Kaki")]
    public AudioSource audioLangkah;
    public AudioClip sfxFootstep;
    public float jedaLangkah = 0.5f;

    [Header("Efek View Bobbing Natural")]
    public float kecepatanBobbing = 14f;
    public float intensitasBobbing = 0.15f;
    public float faktorKetidakseimbangan = 0.3f;

    private float timerLangkah = 0f;
    private float timerBobbing = 0f;
    private Vector3 posisiAwalKamera;
    
    private float xRotation = 0f;
    private CharacterController controller;
    private Vector3 velocity;
    private float gravity = -9.81f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (playerCamera != null)
        {
            posisiAwalKamera = playerCamera.localPosition;
        }
    }

    void Update()
    {
        UpdateStatusGerak();
        MulaiGerak();
    }

    void UpdateStatusGerak()
    {
        bool fokusMotor = InteraksiMotor.instance != null && InteraksiMotor.instance.sedangFokus;
        bool fokusMeja = StationInteraction.instance != null && StationInteraction.instance.sedangFokusMeja;
        bool hpBuka = GameManagerScript.instance != null && GameManagerScript.instance.hpSedangBuka;

        if (fokusMotor || fokusMeja || hpBuka)
        {
            bisaGerak = false;
            bisaRotasiKamera = false;
        }
        else
        {
            bisaGerak = true;
            bisaRotasiKamera = true;
        }
    }

    public void KunciKursor(bool dikunci)
    {
    }

    public void MulaiGerak()
    {
        if (bisaGerak)
        {
            if (bisaRotasiKamera)
            {
                float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
                float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

                xRotation -= mouseY;
                xRotation = Mathf.Clamp(xRotation, -90f, 90f);

                playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
                transform.Rotate(Vector3.up * mouseX);
            }

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;

            if (controller.isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            velocity.y += gravity * Time.deltaTime;

            Vector3 pergerakanAkhir = (move * speed) + velocity;
            controller.Move(pergerakanAkhir * Time.deltaTime);

            if (controller.isGrounded && move.magnitude > 0.1f)
            {
                timerLangkah -= Time.deltaTime;
                if (timerLangkah <= 0f)
                {
                    if (audioLangkah != null && sfxFootstep != null)
                    {
                        audioLangkah.PlayOneShot(sfxFootstep);
                    }
                    timerLangkah = jedaLangkah;
                }

                timerBobbing += Time.deltaTime * kecepatanBobbing;
                
                float ketidakseimbangan = 1f + (Mathf.Sin(timerBobbing * 0.5f) * faktorKetidakseimbangan);
                float posisiYBaru = posisiAwalKamera.y + (Mathf.Sin(timerBobbing) * intensitasBobbing * ketidakseimbangan);
                float posisiXBaru = posisiAwalKamera.x + (Mathf.Cos(timerBobbing * 0.5f) * intensitasBobbing * 0.7f);
                
                playerCamera.localPosition = new Vector3(posisiXBaru, posisiYBaru, playerCamera.localPosition.z);
            }
            else
            {
                if (move.magnitude < 0.1f)
                {
                    timerLangkah = 0f;
                }
                
                timerBobbing = 0f;
                playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, posisiAwalKamera, Time.deltaTime * (kecepatanBobbing / 2f));
            }
        }
        else
        {
            if (controller.isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
    }
}