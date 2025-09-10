Shader "Unlit/Hidden URP DepthMask"
{
      SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry-1" "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Off
        }
    }
}
