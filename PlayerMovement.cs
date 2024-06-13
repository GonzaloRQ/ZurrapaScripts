using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    public Rigidbody rb;
    public Camera cam;
    public static float velocidad, sensibilidadH, t;
    private float rotateHorizontal;
    public GameObject target, ejeTarget, targetInverse, initialTarget, player;
    public SphereCollider sphereCollider;
    public float maxGiro = 90.0f;
    private float currentGiro = 0.0f;
    public GameObject settings, partyLight;
    public Animator animator;
    public ParticleSystem eat;
    public float rotateAngle;
    public float rotateQuantity = 0f;
    public SpecialFoodManager spm;
    // Start is called before the first frame update
    void Start()
    {
        partyLight.SetActive(false);
        velocidad = 40f;
        sensibilidadH = 1f;
        sphereCollider.radius = 0f;
        sphereCollider.isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {

        rotateHorizontal += Input.GetAxis("Mouse X") * sensibilidadH;

        if (!settings.activeSelf & !ScoreManager.GetLost())
        {
            PlayerOrientation();
        }
        if(!spm.spiced)
        {
            if (ScoreManager.GetScore() >= 250 && ScoreManager.GetScore() < 500)
            {
                velocidad = 60f;
            }
            if (ScoreManager.GetScore() >= 500 && ScoreManager.GetScore() < 750)
            {
                velocidad = 65f;
            }
            if (ScoreManager.GetScore() >= 750 && ScoreManager.GetScore() < 1000)
            {
                velocidad = 70f;
            }
            else if (ScoreManager.GetScore() >= 1000 && ScoreManager.GetScore() < 1500)
            {
                velocidad = 75f;
            }
            else if (ScoreManager.GetScore() >= 1500 && ScoreManager.GetScore() < 2000)
            {
                velocidad = 150f;
                rb.mass = 15f;
            }
            else if (ScoreManager.GetScore() >= 2000 && ScoreManager.GetScore() < 2500)
            {
                velocidad = 200f;
                rb.mass = 25f;
            }
            else if (ScoreManager.GetScore() >= 2500 && ScoreManager.GetScore() < 3000)
            {
                velocidad = 500f;
                rb.mass = 40f;
            }
            else if (ScoreManager.GetScore() >= 3000)
            {
                sphereCollider.radius = 0.80f;
                sphereCollider.isTrigger = false;
                velocidad = 900f;
                rb.mass = 90f;
            }
        }
       
       
    }

    public float GetSpeed()
    {
        return velocidad;
    }

    public void SetSpeed(float otherSpeed)
    {
        velocidad = otherSpeed;
    }

    private void FixedUpdate()
    {
        if (!settings.activeSelf & !ScoreManager.GetLost())
        {
            Movement();
        }
    }

    private void LateUpdate()
    {
        if (!settings.activeSelf & !ScoreManager.GetLost())
        {
            CameraMove();
        }
    }

    void PlayerOrientation()
    {
        Vector3 mousePosition = Input.mousePosition;
        if (mousePosition.x < Screen.width * 0.4f || mousePosition.x > Screen.width * 0.6f)
        {
            rotateAngle = (mousePosition.x < Screen.width * 0.5f) ? -700f : 700f;
            if (rotateQuantity != rotateAngle)
            {
                if (rotateAngle < 0)
                {
                    rotateQuantity -= 1.3f;
                }
                else if (rotateAngle > 0)
                {
                    rotateQuantity += 1.3f;
                }
            }
            ejeTarget.transform.eulerAngles = new Vector3(-2f, rotateQuantity, 0f);
            transform.LookAt(targetInverse.transform.position);
        }
    }

    void Movement()
    {
        rb.AddForce(transform.forward * velocidad);
    }

    public void CameraMove()
    {
        cam.transform.position = Vector3.Lerp(cam.transform.position, target.transform.position, Mathf.SmoothStep(1.0f, 0.2f, t));
        cam.transform.LookAt(targetInverse.transform.position);
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.CompareTag("Low")) || (other.gameObject.CompareTag("Mid")) || (other.gameObject.CompareTag("High")))
        {
            eat.Play();
        }
    }
}
