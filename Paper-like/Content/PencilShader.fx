#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif


struct VertexShaderInput
{
    // Geometry Data (From Buffer A)
    float4 LocalPos : POSITION0;
    float2 Position : POSITION1;
    float2 TexCoord : TEXCOORD0;

    // Instance Data (From Buffer B)
    // Must match the VertexDeclaration we defined in C# (BlendWeight 0-3)
    float4 InstanceColor : COLOR1;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float2 TexCoord : TEXCOORD0;
    float4 Color : COLOR0;
};

VertexShaderOutput MainVS(VertexShaderInput input)
{
    VertexShaderOutput output;

    // Reconstruct the matrix from the 4 rows
    output.Color = input.InstanceColor;
    output.Position = float4(input.LocalPos.xy + input.Position, 0, 1);

    output.TexCoord = input.TexCoord;

    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    
    float2 coords = (input.TexCoord - float2(1, 1) * 0.5f) * 2;
    float rSqr = coords.x * coords.x + coords.y * coords.y;
    if (rSqr < 1 && input.Color.a > 0.1f)
    {
        //input.Color.a = (0.01f / rSqr);
        input.Color.a = (1 - sqrt(sqrt(rSqr)) + 0.03f / rSqr - 0.03f * 0.2f) * 0.2f;
        if (input.Color.a > 1)
        {
            input.Color.rgb *= input.Color.a;

        }
        return input.Color; // Render red for test
    }
    discard;
    return float4(1, 0, 1, 1);

}

technique Instancing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};