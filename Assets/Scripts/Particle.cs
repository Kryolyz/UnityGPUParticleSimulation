using UnityEngine;

enum ParticleTypes {
    None,
    dirt,
    stone,
    crystal,
    water,
    air,
    gas
}

public struct Particle
{
    public Vector2 p;
    public Vector2 oldP;
    public Vector2 tempP;

    public Vector2 a;
    public float size;
    public float mass;

    public float temperature;
    public int type;
    public int collisions;
}
