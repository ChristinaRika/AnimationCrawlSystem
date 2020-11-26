Shader "Custom/ImProcessing"
{ 
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        radius ("Radius", Range(0,30)) = 15
        resolution ("Resolution", float) = 800  
        hstep("HorizontalStep", Range(0,1)) = 0.5
        vstep("VerticalStep", Range(0,1)) = 0.5  
        fadeLevel("Fade Level", Range(0,1)) = 0
        cartoon("cartoon", Int) = 0 
        redStrength("Red Strength", Range(0,1)) = 1.0
        greenStrength("Green Strength", Range(0,1)) = 1.0
        blueStrength("Blue Strength", Range(0,1)) = 1.0
        _PixelSize ("PixelSize", Range(1, 100)) = 1   
        _Edge("Edge", Int) = 0
        _Intensity("_Intensity", Range(0, 1)) = 0
        _BackgroundColor("_BackgroundColor", Color) = (1,1,1,1)
        _EdgeColor("_EdgeColor", Color) = (0,0,0,1)
    }

    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="true" "RenderType"="Transparent"}
        ZWrite Off Blend SrcAlpha OneMinusSrcAlpha Cull Off ZTest Always
        Pass
        {    
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_fog
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };    
            struct v2f
            {
                half2 texcoord[9]  : TEXCOORD0;
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                UNITY_FOG_COORDS(1)
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
            float _Intensity;
            int _Edge;

            float hstep;
            float vstep;

            fixed4 _EdgeColor;
            fixed4 _BackgroundColor;

            float4 _MainTex_TexelSize;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                
                half2 uv = IN.texcoord;
                half2 size = _MainTex_TexelSize.xy;

                OUT.texcoord[0] = uv + size * half2(-1, -1);
                OUT.texcoord[1] = uv + size * half2(0, -1);
                OUT.texcoord[2] = uv + size * half2(1, -1);
                OUT.texcoord[3] = uv + size * half2(-1, 0);
                OUT.texcoord[4] = uv + size * half2(0, 0);
                OUT.texcoord[5] = uv + size * half2(1, 0);
                OUT.texcoord[6] = uv + size * half2(-1, 1);
                OUT.texcoord[7] = uv + size * half2(0, 1);
                OUT.texcoord[8] = uv + size * half2(1, 1);

                UNITY_TRANSFER_FOG(OUT,OUT.vertex);

                return OUT;
            }
            fixed luminance(fixed4 color) {
                return  0.2125 * color.r + 0.7154 * color.g + 0.0721 * color.b; 
            }
            half sobel(v2f i) 
            {
                const half Gx[9] = {
                    - 1,0,1,
                    - 2,0,2,
                    - 1,0,1
                };
                const half Gy[9] = {
                    -1,-2,-1,
                    0 , 0, 0,
                    1 , 2, 1
                };

                //set horizontal and vertical gradient
                half graX = 0;
                half graY = 0;
                
                for (int it = 0; it < 9; it++) 
                {
                    graX += Gx[it] * luminance(tex2D(_MainTex, i.texcoord[it]));
                    graY += Gy[it] * luminance(tex2D(_MainTex, i.texcoord[it]));
                }
                
                return 1 - abs(graX) - abs(graY);
            }
            

            float4 frag(v2f i) : SV_TARGET
            {    
                float2 uv = i.texcoord[4];
                fixed4 sum = float4(0.0, 0.0, 0.0, 0.0);
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
                //gray
                sum = lerp(sum, luminance(sum), fadeLevel);

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

                if(_Edge){
                    half gra = sobel(i);
                    fixed4 withEdgeColor = lerp( _EdgeColor, sum, gra);
                    fixed4 onlyEdgeColor = lerp( _EdgeColor, _BackgroundColor, gra);
                    sum = lerp(withEdgeColor, onlyEdgeColor, _Intensity);
                    UNITY_APPLY_FOG(i.fogCoord, sum);
                }
                
                return fixed4(sum.rgb, 1);
            }    
            ENDCG
        }
    }
    Fallback "Sprites/Default"    
}