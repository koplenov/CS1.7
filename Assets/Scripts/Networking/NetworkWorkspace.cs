using System;
using UnityEngine;

#region Network

[Serializable]
public class OldNetPlayer
{
    // TODO: add timestamp for non local network
    public OldNetPlayer()
    {
    }

    public OldNetPlayer(string nick, Vector3 position, Vector3 rotation)
    {
        this.nick = nick;
        this.position = new SVector(position);
        this.rotation = new SVector(rotation);
    }

    public OldNetPlayer(string nick, Transform transform)
    {
        this.nick = nick;
        this.position = new SVector(transform.position);
        this.rotation = new SVector(transform.rotation.eulerAngles);
    }

    [SerializeField] public string nick;
    [SerializeField] public SVector position;
    [SerializeField] public SVector rotation;

    public Transform Transform
    {
        set
        {
            this.position = new SVector(value.position);
            this.rotation = new SVector(value.rotation.eulerAngles);
        }
    }
}

[Serializable]
public class NetPlayer
{
    // TODO: add timestamp for non local network
    public NetPlayer(string nick)
    {
        this.nick = nick;
        this.Position = new Vector3();
        this.Rotation = new Vector3();
    }

    public NetPlayer(string nick, Vector3 position, Vector3 rotation)
    {
        this.nick = nick;
        this.Position = position;
        this.Rotation = rotation;
    }

    public NetPlayer(string nick, Transform transform)
    {
        this.nick = nick;
        this.Position = transform.position;
        this.Rotation = transform.rotation.eulerAngles;
    }

    public Vector3 Rotation
    {
        get => rotation.Vector3;
        set
        {
            rotation.x = value.x;
            rotation.y = value.y;
            rotation.z = value.z;
        }
    }
    public Vector3 Position
    {
        get => position.Vector3;
        set
        {
            position.x = value.x;
            position.y = value.y;
            position.z = value.z;
        }
    }
    public Transform Transform
    {
        set
        {
            Position = value.position;
            Rotation = value.rotation.eulerAngles;
        }
    }

    [SerializeField] public string nick = String.Empty;
    [SerializeField] public SVector position = new SVector(Vector3.zero);
    [SerializeField] public SVector rotation = new SVector(Vector3.zero);
}

[Serializable]
public class SVector
{
    public float x;
    public float y;
    public float z;

    public SVector(Vector3 position)
    {
        x = position.x;
        y = position.y;
        z = position.x;
    }

    public Vector3 Vector3
    {
        get => new Vector3(x, y, z);
        set
        {
            x = value.x;
            y = value.y;
            z = value.z;
        }
    }
}

#endregion