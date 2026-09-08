using UnityEngine;

public class GerakanKameraMotor : MonoBehaviour
{
    public static GerakanKameraMotor instance;
    public Transform pos1;
    public Transform pos2;
    public Transform cameraMotor;
    public GameObject cameraObjek;
    public float speed = 5f;
    public float mouseSensitivity = 200f;
    public bool canMove = false;

    private float xRotation = 0f;
    private float yRotation = 0f;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (cameraMotor != null)
        {
            xRotation = cameraMotor.localEulerAngles.x;
            yRotation = cameraMotor.localEulerAngles.y;
        }
    }

    void Update()
    {
        if (cameraObjek != null && cameraObjek.activeSelf)
        {
            canMove = true;
        } 
        else
        {
            canMove = false;
        }

        if (canMove == true)
        {
            MulaiGerak();
        }
    }

    public void MulaiGerak()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        yRotation += mouseX;

        cameraMotor.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

        float gerakHorizontal = Input.GetAxis("Horizontal");
        float gerakVertical = Input.GetAxis("Vertical");

        Vector3 gerak = (cameraMotor.right * gerakHorizontal) + (cameraMotor.forward * gerakVertical);
        cameraMotor.position += gerak * speed * Time.deltaTime;

        float minX = Mathf.Min(pos1.position.x, pos2.position.x);
        float maxX = Mathf.Max(pos1.position.x, pos2.position.x);
        float minY = Mathf.Min(pos1.position.y, pos2.position.y);
        float maxY = Mathf.Max(pos1.position.y, pos2.position.y);
        float minZ = Mathf.Min(pos1.position.z, pos2.position.z);
        float maxZ = Mathf.Max(pos1.position.z, pos2.position.z);

        Vector3 posisiDibatasi = cameraMotor.position;
        posisiDibatasi.x = Mathf.Clamp(posisiDibatasi.x, minX, maxX);
        posisiDibatasi.y = Mathf.Clamp(posisiDibatasi.y, minY, maxY);
        posisiDibatasi.z = Mathf.Clamp(posisiDibatasi.z, minZ, maxZ);

        cameraMotor.position = posisiDibatasi;
    }
}