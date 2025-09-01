using UnityEngine;

public class CatapultController : MonoBehaviour
{
    public HingeJoint hinge;
    public float motorVelocity = 500f;
    public float motorForce = 2000f;

    public Transform projectileSpawnPoint;
    public GameObject projectilePrefab;
    private GameObject currentProjectile;

    private bool isFiring = false;

    void Start()
    {
        
        SetMotor(false); // Initially off
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isFiring)
        {
            isFiring = true;
            Fire();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            SetMotor(false);
            isFiring = false;
        }
    }

    void LoadProjectile()
    {
        currentProjectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        //currentProjectile.transform.SetParent(projectileSpawnPoint);
      //  currentProjectile.GetComponent<Rigidbody>().isKinematic = true;
    }

    void Fire()
    {

        LoadProjectile();
        // Release projectile before arm swings
        // currentProjectile.transform.SetParent(null);
        // currentProjectile.GetComponent<Rigidbody>().isKinematic = false;

        SetMotor(true);
    }

    void SetMotor(bool enable)
    {
        JointMotor motor = hinge.motor;
        motor.force = motorForce;
        motor.targetVelocity = enable ? motorVelocity : 0;
        hinge.motor = motor;
        hinge.useMotor = enable;
    }
}