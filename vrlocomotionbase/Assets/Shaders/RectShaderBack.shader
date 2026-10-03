Shader "Unlit/RectShaderBack"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "red" {}
        _Color ("Example color", Color) = (1, 0, 0, 0.33)
    }
    SubShader
    {
        Tags {"Queue" = "Overlay" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Front
        ZTest Off
        //Cull Off


        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                // draw borders with width
                float width = 0.025;

                if (i.uv.x < width || i.uv.x > 1 - width || i.uv.y < width || i.uv.y > 1 - width) {
                     col.r = _Color[0];
                    col.g = _Color[1];
                    col.b = _Color[2];
                }
                else {
                    col.r = _Color[0]*0.7;
                    col.g = _Color[1]*0.7;
                    col.b = _Color[2]*0.7;
                }

                col.a = 0.33;

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
