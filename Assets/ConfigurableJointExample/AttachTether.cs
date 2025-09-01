using UnityEngine;

public class AttachTether : MonoBehaviour
{
    public Rigidbody connectedBody;

    void Start()
    {
        ConfigurableJoint joint = gameObject.AddComponent<ConfigurableJoint>();
        joint.connectedBody = connectedBody;

        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = Vector3.zero;
        joint.connectedAnchor = new Vector3(0, 0, 0);

        // Allow movement on all axes, but restrict with limits
        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Limited;
        joint.zMotion = ConfigurableJointMotion.Limited;

        SoftJointLimit limit = new SoftJointLimit();
        limit.limit = 5f; // Max distance from connected body

        joint.linearLimit = limit;

        // Optional springy effect
        JointDrive drive = new JointDrive
        {
            positionSpring = 100f,
            positionDamper = 5f,
            maximumForce = Mathf.Infinity
        };

        joint.xDrive = drive;
        joint.yDrive = drive;
        joint.zDrive = drive;
    }
}
    