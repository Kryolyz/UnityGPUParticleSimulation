
#define PARTICLE_TYPE_NONE 0
#define PARTICLE_TYPE_DIRT 1
#define PARTICLE_TYPE_STONE 2
#define PARTICLE_TYPE_CRYSTAL 3
#define PARTICLE_TYPE_WATER 4
#define PARTICLE_TYPE_AIR 5
#define PARTICLE_TYPE_GAS 6

uint TextureWidth;
uint ParticleBufferWidth;
uint GridSize;
float restitutionCoefficient;

struct Particle
{
    float2 p;
    float2 newP;

    float2 v;
    float2 newV;

    float2 a;
    
    float size;
    float mass;

    float temperature;
    int type;
    int collisions;
};

void clampParticle(inout float2 p, inout float2 v, float size, float restitutionCoefficient)
{
    float textureWidthf = TextureWidth;
    
    if (p.x + size > (textureWidthf - 1.0f))
    {
        p.x -= (p.x + size - (textureWidthf - 1.0f));
        v.x *= -1.0f * restitutionCoefficient;
    }
    else if (p.x - size < -0.5f)
    {
        p.x += - (p.x - size);
        v.x *= -1.0f * restitutionCoefficient;
    }

    if (p.y + size > (textureWidthf - 1.0f))
    {
        p.y -= (p.y + size - (textureWidthf - 1.0f));
        v.y *= -1.0f * restitutionCoefficient;
    }
    else if (p.y - size < -0.5f)
    {
        p.y += - (p.y - size);
        v.y *= -1.0f * restitutionCoefficient;
    }
}

uint IndexFromXY(uint2 id)
{
    return id.y * ParticleBufferWidth + id.x;
}

uint3 GridIndex(float2 p)
{
    int3 gridIndex;
    //clamp(int((f2Pos.x + 1.0) * 0.5 * float(i2ParticleGridSize.x)), 0, i2ParticleGridSize.x - 1);
    //gridIndex.x = clamp(int((p.x + 1.0) * 0.5 * float(TextureWidth / GridSize)), 0, TextureWidth / GridSize - 1);
    //gridIndex.y = clamp(int((p.y + 1.0) * 0.5 * float(TextureWidth / GridSize)), 0, TextureWidth / GridSize - 1);
    gridIndex.x = clamp(int((p.x + 1.f) / GridSize), 0, TextureWidth / GridSize - 1);
    gridIndex.y = clamp(int((p.y + 1.f) / GridSize), 0, TextureWidth / GridSize - 1);
    gridIndex.z = (gridIndex.y * TextureWidth / GridSize + gridIndex.x);
    return gridIndex;
}