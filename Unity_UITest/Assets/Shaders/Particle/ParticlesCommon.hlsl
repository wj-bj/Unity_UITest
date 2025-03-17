#ifndef PARTICLES_COMMON_INCLUDED
#define PARTICLES_COMMON_INCLUDED


#if defined (_ENABLE_FOG_TRANS) & (_ENABLE_FOG)
#define FOG_COORD(idx1) float fogcoord : TEXCOORD##idx1;
#else
#define FOG_COORD(idx1)
#endif


//FOG----------------------------
uniform half4 _FogParameter; //(x:FogEnd,y:FogStart,z:FogBottom,w:FogTop)
uniform half4 _FogColor;


//////////////////////fog/////////////////////////////////////////////

float Transfer_Fog(float z, float3 posWorld)
{
    float clipZ_01 = UNITY_Z_0_FAR_FROM_CLIPSPACE(z);
    float fogDis  = saturate(clipZ_01 * _FogParameter.x + _FogParameter.y);
    float fogHeight = clamp((_FogParameter.z - posWorld.y) / (_FogParameter.z - _FogParameter.w), 0.0, 0.99);
    return  max(fogHeight, fogDis);
}

half3 Mix_Fog(half3 col, float fogCoord)
{
    return   lerp( col.rgb,_FogColor.rgb,saturate((1 - fogCoord) * _FogColor.a));
     
}
half3 Mix_Fog_Add(half3 col, float fogCoord)
{
    
    half fogWeight = saturate((1 - fogCoord) * _FogColor.a);
    return   lerp(col.rgb,0,fogWeight);
     
}
half Mix_Fog_Alpha(half a, float fogCoord)
{
    return   (lerp(a,0,saturate((1 - fogCoord) * _FogColor.a)));
     
}
 

#if defined (_ENABLE_FOG_TRANS) & (_ENABLE_FOG)
    #define TRANSFER_FOG(z,posWorld, OUT) OUT.fogcoord = Transfer_Fog(z , posWorld.xyz);
    #define  MIX_FOG(a,IN) a.rgb = Mix_Fog(a.rgb,IN.fogcoord);
    #define  MIX_FOG_ALPHA(a,IN) a = Mix_Fog_Alpha(a,IN.fogcoord);
    #define  MIX_FOG_ADD(a,IN) a.rgb = Mix_Fog_Add(a,IN.fogcoord);
#else
    #define TRANSFER_FOG(z,posWorld, OUT)
    #define  MIX_FOG(a,IN)
    #define  MIX_FOG_ALPHA(a,IN)
    #define  MIX_FOG_ADD(a,IN) 
#endif 

half3 ParticlesOutColor(half3 color)
{
    return color;
    // color = clamp(color,0.0,1.5);
    float3 a = (1.2).xxx;
    float3 x = (0.3).xxx;
   
    color = ( color > a ? pow( color , x ) : color <= a && color >= a ? color : color );
    //color = saturate(color);
    return color;
}
//////////////////////////////////////////////////////////////////////////////

















#endif //PARTICLES_COMMON_INCLUDED