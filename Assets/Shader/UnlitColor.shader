Shader "Custom/BlackUnlit" {
	Properties {
        _MainColor ("MainColor", Color) = (0, 0, 0, 0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Unlit

        half4 LightingUnlit (SurfaceOutput s, half3 lightDir, half atten) {
          fixed4 c;
          c.rgb = s.Albedo;
          c.a = s.Alpha;
          
          return c;
      }

        float4 _MainColor;

		struct Input {
            float4 color : COLOR;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = _MainColor;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
}
