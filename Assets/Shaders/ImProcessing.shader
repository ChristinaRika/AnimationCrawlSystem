Shader "Custom/ImProcessing"
{ 
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        radius ("Radius", Range(0,30)) = 15
        resolution ("Resolution", float) = 800  
        hstep("HorizontalStep", Range(0,1)) = 0.5
        vstep("VerticalStep", Range(0,1)) = 0.5  
        fadeLevel("Fade Level", Range(0,1)) = 1.0
        cartoon("cartoon", int) = 0 
        redStrength("Red Strength", Range(0,1)) = 1.0
        greenStrength("Green Strength", Range(0,1)) = 1.0
        blueStrength("Blue Strength", Range(0,1)) = 1.0
        _PixelSize ("PixelSize", Range(1, 100)) = 1
    }

    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="true" "RenderType"="Transparent"}
        ZWrite Off Blend SrcAlpha OneMinusSrcAlpha Cull Off
        Pass
        {    
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };    
            struct v2f
            {
                half2 texcoord  : TEXCOORD0;
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
            };

            sampler2D _MainTex;
            float radius;
            float resolution;
            float fadeLevel;
            int cartoon;
            float redStrength;
            float greenStrength;
            float blueStrength;
            int _PixelSize;

            float hstep;
            float vstep;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color;
                return OUT;
            }
            float4 _MainTex_TexelSize;

            float4 frag(v2f i) : COLOR
            {    
                float2 uv = i.texcoord.xy;
                float4 sum = float4(0.0, 0.0, 0.0, 0.0);
                float2 tc = uv;

                //pixel
                float2 interval = _PixelSize * _MainTex_TexelSize.xy;
                tc = uv / interval;    // belongs which pixel
                tc = tc - frac(tc);  // delete point to intilate pixel(!)
                tc *= interval;       //new uv

                //blur radius in pixels
                float blur = radius/resolution/4;     

                sum += tex2D(_MainTex, float2(tc.x - 4.0*blur*hstep, tc.y - 4.0*blur*vstep)) * 0.0162162162;
                sum += tex2D(_MainTex, float2(tc.x - 3.0*blur*hstep, tc.y - 3.0*blur*vstep)) * 0.0540540541;
                sum += tex2D(_MainTex, float2(tc.x - 2.0*blur*hstep, tc.y - 2.0*blur*vstep)) * 0.1216216216;
                sum += tex2D(_MainTex, float2(tc.x - 1.0*blur*hstep, tc.y - 1.0*blur*vstep)) * 0.1945945946;

                sum += tex2D(_MainTex, float2(tc.x, tc.y)) * 0.2270270270;

                sum += tex2D(_MainTex, float2(tc.x + 1.0*blur*hstep, tc.y + 1.0*blur*vstep)) * 0.1945945946;
                sum += tex2D(_MainTex, float2(tc.x + 2.0*blur*hstep, tc.y + 2.0*blur*vstep)) * 0.1216216216;
                sum += tex2D(_MainTex, float2(tc.x + 3.0*blur*hstep, tc.y + 3.0*blur*vstep)) * 0.0540540541;
                sum += tex2D(_MainTex, float2(tc.x + 4.0*blur*hstep, tc.y + 4.0*blur*vstep)) * 0.0162162162;

                //rgb
                sum.rgb = float3(sum.r * redStrength, sum.g * greenStrength, sum.b * blueStrength);
                // gray
                float c = (sum.r+sum.g+sum.b)/3.0;
                sum.r = sum.r * fadeLevel + c*(1-fadeLevel);
                sum.g = sum.g * fadeLevel + c*(1-fadeLevel);
                sum.b = sum.b * fadeLevel + c*(1-fadeLevel);

                // simple cartoon[if possible, use more methods]
                if(cartoon == 1){
                    if(sum.r < 0.2)sum.r = 0.2;
                    else if(sum.r > 0.2 && sum.r < 0.4)sum.r = 0.4;
                    else if(sum.r > 0.4 && sum.r < 0.6)sum.r = 0.6;
                    else if(sum.r > 0.6 && sum.r < 0.8)sum.r = 0.8;
                    else if(sum.r > 0.8)sum.r = 1.0;

                    if(sum.g < 0.2)sum.g = 0.2;
                    else if(sum.g > 0.2 && sum.g < 0.4)sum.g = 0.4;
                    else if(sum.g > 0.4 && sum.g < 0.6)sum.g = 0.6;
                    else if(sum.g > 0.6 && sum.g < 0.8)sum.g = 0.8;
                    else if(sum.g > 0.8)sum.g = 1.0;

                    if(sum.b < 0.2)sum.b = 0.2;
                    else if(sum.b > 0.2 && sum.b < 0.4)sum.b = 0.4;
                    else if(sum.b > 0.4 && sum.b < 0.6)sum.b = 0.6;
                    else if(sum.b > 0.6 && sum.b < 0.8)sum.b = 0.8;
                    else if(sum.b > 0.8)sum.b = 1.0;
                }

                return float4(sum.rgb, 1);
            }    
            ENDCG
        }
    }
    Fallback "Sprites/Default"    
}