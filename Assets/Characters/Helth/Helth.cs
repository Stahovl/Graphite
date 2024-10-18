using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helth : MonoBehaviour
{
    protected int _currentHelth;

    public bool IsAlive() {
        return _currentHelth > 0;
    }

    public virtual void AddHelth( int value ) {
        _currentHelth += value;
    }

    public int GetHelth()
    {
        return _currentHelth;
    }
}
