// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

/// <minValue>0</minValue>
/// <maxValue>1</maxValue>
/// <defaultValue>0</defaultValue>
float Progress : register(C0);

/// <minValue>0</minValue>
/// <maxValue>3</maxValue>
/// <defaultValue>1</defaultValue>
float Mode : register(C1);

/// <minValue>-1,-1</minValue>
/// <maxValue>1,1</maxValue>
/// <defaultValue>1,0</defaultValue>
float2 SlideVector : register(C2);

sampler2D Texture1 : register(s0);

sampler2D Texture2 : register(s1);

float4 Fade(float2 uv, float progress)
{
    float4 c1 = tex2D(Texture2, uv);
    float4 c2 = tex2D(Texture1, uv);
    return lerp(c1, c2, progress);
}

float4 LeastBright(float2 uv, float progress)
{
    int c = 4;
    int c2 = 3;
    float oc = (c - 1) / 2;
    float oc2 = (c2 - 1) / 2;
    float offset = 0.01 * progress;

    float leastBright = 1;
    float4 leastBrightColor;
    for (int y = 0; y < c; y++)
    {
        for (int x = 0; x < c2; x++)
        {
            float2 newUV = uv + (float2(x, y) - float2(oc2, oc)) * offset;
            float4 color = tex2D(Texture2, newUV);
            float brightness = dot(color.rgb, float3(1, 1.1, 0.9));
            if (brightness < leastBright)
            {
                leastBright = brightness;
                leastBrightColor = color;
            }
        }
    }

    float4 impl = tex2D(Texture1, uv);

    return lerp(leastBrightColor, impl, progress);
}

float4 Pixelate(float2 uv, float progress)
{
    float pixels;
    float segment_progress;
    if (progress < 0.5)
    {
        segment_progress = 1 - progress * 2;
    }
    else
    {
        segment_progress = (progress - 0.5) * 2;
    }

    pixels = 5 + 1000 * segment_progress * segment_progress;
    float2 newUV = round(uv * pixels) / pixels;

    float4 c1 = tex2D(Texture2, newUV);
    float4 c2 = tex2D(Texture1, newUV);

    float lerp_progress = saturate((progress - 0.4) / 0.2);
    return lerp(c1, c2, lerp_progress);
}

float4 Slide(float2 uv, float progress)
{
    uv += SlideVector * progress;
    if (any(saturate(uv) - uv))
    {
        uv = frac(uv);
        return tex2D(Texture1, uv);
    }
    else
    {
        return tex2D(Texture2, uv);
    }
}

float4 main(float2 uv : TEXCOORD) : COlOR
{
    if (Mode > 0)
        return Fade(uv, Progress);
    else if (Mode >= 1)
        return LeastBright(uv, Progress);
    else if (Mode >= 2)
        return Pixelate(uv, Progress);
    else
        return Slide(uv, Progress);
}
