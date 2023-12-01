Shader "Custom/QuadWarpUI"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_OverlayTex ("Overlay Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "PreviewType" ="Plane" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _OverlayTex;
			float4x4 _Homography;

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 local_pos : TEXCOORD1;
			};

			v2f vert(float3 vertex : POSITION )
			{
				v2f o;
				o.pos = UnityObjectToClipPos (vertex);
				o.local_pos = vertex;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float3 uvq = mul(_Homography, float3(i.local_pos.xy,1));
				fixed4 overlay = tex2D(_OverlayTex, i.local_pos.xy);
				fixed4 m = tex2D(_MainTex, uvq.xy/uvq.z);
				
				fixed4 result = m * (1 - overlay.a) + overlay * overlay.a;
				return result;
				//return tex2D(_MainTex, uvq.xy/uvq.z);
			}
			ENDCG
		}
	}
}
