using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Orientation : MonoBehaviour
{
    private Dictionary<Collider, List<Vector3>> collisionNormals = new Dictionary<Collider, List<Vector3>>();
    protected Rigidbody _rb;
    protected Vector3 collisionNormal;
    /// <summary>
    /// Кривизна поверхности(нормаль поверхности)
    /// </summary>
    protected Quaternion targetRotation; 



    protected Vector3 _gravity;
    protected Vector3 _gravity_normal;
    protected float _gravity_magnitude;
    public Vector3 Gravity
    {
        get { return _gravity; }
        set {
            _gravity = value;
            _gravity_normal = value.normalized;
            _gravity_magnitude = value.magnitude;
        }
    }
    public bool RotateInAir = true;
    public float SlipAngle = 0.5f;
    public float LerpNormalGravity = 0.75f;

    private void Awake()
    {
        _rb = gameObject.GetComponent<Rigidbody>();
        _rb.useGravity = false;
        if (RotateInAir) FreezRotation(true);
    }

    protected void FreezRotation(bool value)
    {
        _rb.freezeRotation = value;
        //_rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; // Замораживает вращение по X и Y

    }

    void FixedUpdate()
    {
        _rb.linearVelocity += _gravity * Time.deltaTime;

        UpdateCollisionNormal();

        if (collisionNormal != Vector3.zero) FreezRotation(true);
        else if (!RotateInAir) FreezRotation(false);
        RotateObjectUp();

        OnUpdate();
    }

    public virtual void OnUpdate()
    {
    }

    private void RotateObjectUp()
    {
        Vector3 targetUp = collisionNormal != Vector3.zero ? collisionNormal : -_gravity_normal;


        targetRotation = Quaternion.FromToRotation(transform.up, targetUp) * transform.rotation;


        if (collisionNormal != Vector3.zero || RotateInAir)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.FromToRotation(transform.up, Vector3.Lerp(targetRotation * Vector3.up, -_gravity_normal, LerpNormalGravity).normalized) * transform.rotation, 
                Time.deltaTime * 4.0f
            );
        }
    }

    public bool isGrounded()
    {
        return collisionNormal != Vector3.zero;
    }





    private void OnCollisionEnter(Collision collision)
    {
        if (!collisionNormals.ContainsKey(collision.collider))
        {
            collisionNormals[collision.collider] = new List<Vector3>();
        }

        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, -_gravity_normal) > SlipAngle)
            {
                collisionNormals[collision.collider].Add(contact.normal);
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!collisionNormals.ContainsKey(collision.collider))
        {
            collisionNormals[collision.collider] = new List<Vector3>();
        }
        else
        {
            collisionNormals[collision.collider].Clear();
        }

        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, -_gravity_normal) > SlipAngle)
            {
                collisionNormals[collision.collider].Add(contact.normal);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collisionNormals.ContainsKey(collision.collider))
        {
            collisionNormals.Remove(collision.collider);
        }

        if (collisionNormals.Count == 0)
        {
            collisionNormal = Vector3.zero;
        }
    }

    private void UpdateCollisionNormal()
    {
        List<Vector3> allSupportingNormals = new List<Vector3>();

        foreach (var normals in collisionNormals.Values)
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
            collisionNormal = (averageNormal / allSupportingNormals.Count).normalized;
        }
        else
        {
            collisionNormal = Vector3.zero;
        }
    }
}
