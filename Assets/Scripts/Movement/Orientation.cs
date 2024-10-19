using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class Orientation : MonoBehaviour
{
    public Quaternion TargetRotation;

    [SerializeField] private float _slipAngle = 0.5f;
    [SerializeField] private float _lerpNormalGravity = 0.0f;
    [SerializeField] private bool _rotateInAir = true;

    private Dictionary<Collider, List<Vector3>> _collisionNormals = new Dictionary<Collider, List<Vector3>>();

    protected Rigidbody Rb;

    protected Vector3 CollisionNormal;
    protected Vector3 _gravity;
    protected Vector3 _gravity_normal;
    protected float _gravity_magnitude;

    public Vector3 Gravity
    {
        get { return _gravity; }
        set
        {
            _gravity = value;
            _gravity_normal = value.normalized;
            _gravity_magnitude = value.magnitude;
        }
    }
 

    private void Awake()
    {
        Rb = gameObject.GetComponent<Rigidbody>();
        Rb.useGravity = false;
        if (_rotateInAir) FreezRotation(true);
    }

    protected void FreezRotation(bool value)
    {
        Rb.freezeRotation = value;
        //_rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; // Замораживает вращение по X и Y

    }

    private void FixedUpdate()
    {
        Rb.linearVelocity += _gravity * Time.deltaTime;

        UpdateCollisionNormal();

        if (CollisionNormal != Vector3.zero) FreezRotation(true);
        else if (!_rotateInAir) FreezRotation(false);
        RotateObjectUp();

        OnUpdate();
    }

    public virtual void OnUpdate()
    {
    }

    private void RotateObjectUp()
    {
        Vector3 targetUp = CollisionNormal != Vector3.zero ? CollisionNormal : -_gravity_normal;


        TargetRotation = Quaternion.FromToRotation(transform.up, targetUp) * transform.rotation;


        if (CollisionNormal != Vector3.zero || _rotateInAir)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.FromToRotation(transform.up, Vector3.Lerp(TargetRotation * Vector3.up, -_gravity_normal, _lerpNormalGravity).normalized) * transform.rotation,
                Time.deltaTime * 4.0f
            );
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_collisionNormals.ContainsKey(collision.collider))
        {
            _collisionNormals[collision.collider] = new List<Vector3>();
        }

        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, -_gravity_normal) > _slipAngle)
            {
                _collisionNormals[collision.collider].Add(contact.normal);
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!_collisionNormals.ContainsKey(collision.collider))
        {
            _collisionNormals[collision.collider] = new List<Vector3>();
        }
        else
        {
            _collisionNormals[collision.collider].Clear();
        }

        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, -_gravity_normal) > _slipAngle)
            {
                _collisionNormals[collision.collider].Add(contact.normal);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (_collisionNormals.ContainsKey(collision.collider))
        {
            _collisionNormals.Remove(collision.collider);
        }

        if (_collisionNormals.Count == 0)
        {
            CollisionNormal = Vector3.zero;
        }
    }

    private void UpdateCollisionNormal()
    {
        List<Vector3> allSupportingNormals = new List<Vector3>();

        foreach (var normals in _collisionNormals.Values)
        {
            allSupportingNormals.AddRange(normals);
        }

        if (allSupportingNormals.Count > 0)
        {
            Vector3 averageNormal = Vector3.zero;
            foreach (var normal in allSupportingNormals)
            {
                averageNormal += normal;
            }
            CollisionNormal = (averageNormal / allSupportingNormals.Count).normalized;
        }
        else
        {
            CollisionNormal = Vector3.zero;
        }
    }

    public bool IsGrounded()
    {
        return CollisionNormal != Vector3.zero;
    }
}

