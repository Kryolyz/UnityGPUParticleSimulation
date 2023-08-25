
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
    float2 oldP;
    float2 tempP;

    float2 a;
    
    float size;
    float mass;

    float temperature;
    float4 color;
    int type;
    int collisions;
};

void clampParticle(inout float2 p, inout float2 oldP, float size)
{
    float textureWidthf = TextureWidth;
    
    if (p.x + size > (textureWidthf - 1.0f) || p.x < size)
    {
        p.x += (1 + restitutionCoefficient) * (oldP.x - p.x);
    }

    if (p.y + size > (textureWidthf - 1.0f) || p.y < size)
    {
        p.y += (1 + restitutionCoefficient) * (oldP.y - p.y);
    }
}

uint IndexFromXY(uint2 id)
{
    return id.y * ParticleBufferWidth + id.x;
}

uint3 GridIndex(float2 p)
{
    int3 gridIndex;
    gridIndex.x = clamp(int((p.x + 1.f) / GridSize), 0, TextureWidth / GridSize - 1);
    gridIndex.y = clamp(int((p.y + 1.f) / GridSize), 0, TextureWidth / GridSize - 1);
    gridIndex.z = (gridIndex.y * TextureWidth / GridSize + gridIndex.x);
    return gridIndex;
}