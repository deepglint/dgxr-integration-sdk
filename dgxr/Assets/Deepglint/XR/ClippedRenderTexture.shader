Shader "Custom/ClippedRenderTexture"
{
    Properties  
    {  
        _MainTex ("Texture", 2D) = "white" {}  
        _OffsetX ("OffsetX", Range(-1, 1)) = 0  
        _OffsetY ("OffsetY", Range(-1, 1)) = 0  
        _ScaleX ("ScaleX", Range(0, 2)) = 1  
        _ScaleY ("ScaleY", Range(0, 2)) = 1  
    }  
    SubShader  
    {  
        // No culling or depth  
        Cull Off ZWrite Off ZTest Always  
  
        Pass  
        {  
            CGPROGRAM  
            #pragma vertex vert  
            #pragma fragment frag  
  
            #include "UnityCG.cginc"  
  
            struct appdata  
            {  
                float4 vertex : POSITION;  
                float2 uv : TEXCOORD0;  
            };  
  
            struct v2f  
            {  
                float2 uv : TEXCOORD0;  
                float4 vertex : SV_POSITION;  
            };  
  
            sampler2D _MainTex;  
            float _OffsetX, _OffsetY, _ScaleX, _ScaleY;  
  
            v2f vert (appdata v)  
            {  
                v2f o;  
                o.vertex = UnityObjectToClipPos(v.vertex);  
                o.uv = v.uv;  
                return o;  
            }  
  
            fixed4 frag (v2f i) : SV_Target  
            {  
                // Apply offset and scale to UVs  
                float2 uv = i.uv;  
                uv.x = uv.x * _ScaleX + _OffsetX;  
                uv.y = uv.y * _ScaleY + _OffsetY;  
                  
                // Sample the texture  
                fixed4 col = tex2D(_MainTex, uv);  
                return col;  
            }  
            ENDCG  
        }  
    }  
}
