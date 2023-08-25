

float4 RAINBOW(uint index, float freq)
{
    float r = 0.5 + 0.5 * sin(freq * index + 0);
    float g = 0.5 + 0.5 * sin(freq * index + 2 * 3.14159 / 3);
    float b = 0.5 + 0.5 * sin(freq * index + 4 * 3.14159 / 3);
    float3 color = float3(r, g, b);
    return float4(color, 1.0);
}
