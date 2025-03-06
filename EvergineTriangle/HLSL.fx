struct VSInput
{
    float4 position : POSITION;
};

struct PSInput
{
    float4 position : SV_POSITION;
};

PSInput VSMain(VSInput input)
{
    PSInput output;
    output.position = input.position;
    
    return output;
}

float4 PSMain(PSInput input) : SV_Target
{
    return float4(1.0, 1.0, 1.0, 1.0);
}
