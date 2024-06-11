Shader "Custom/ClippedRenderTexture"
{
    Properties  
    {  
        _MainTex ("Texture", 2D) = "white" {}  
        _OffsetX ("OffsetX", Range(-1, 1)) = 0  
        _OffsetY ("OffsetY", Range(-1, 1)) = 0  
        _ScaleX ("ScaleX", Range(0, 2)) = 1  
        _ScaleY ("ScaleY", Range(0, 2)) = 1
        _Rotation ("Rotation", Float) = 0 
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
            float _OffsetX, _OffsetY, _ScaleX, _ScaleY, _Rotation;  
  
            v2f vert (appdata v)  
            {  
                v2f o;  
                o.vertex = UnityObjectToClipPos(v.vertex);

                // Apply scaling and offset first
                float2 uv = v.uv;
                uv.x = uv.x * _ScaleX + _OffsetX;
                uv.y = uv.y * _ScaleY + _OffsetY;

                // Convert to -1 to 1 range
                uv = uv * 2.0 - 1.0;

                // Rotate UV coordinates
                float sinTheta = sin(_Rotation);
                float cosTheta = cos(_Rotation);
                float2x2 rotationMatrix = float2x2(cosTheta, -sinTheta, sinTheta, cosTheta);
                uv = mul(rotationMatrix, uv);

                // Convert back to 0 to 1 range
                uv = uv * 0.5 + 0.5;

                o.uv = uv;
                return o;  
            }  
  
            fixed4 frag (v2f i) : SV_Target  
            {  
                fixed4 col = tex2D(_MainTex, i.uv);  
                return col;  
            }  
            ENDCG  
        }  
    }  
}