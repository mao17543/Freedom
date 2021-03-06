// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

#ifndef CURVED_RENDERING_COMMON
#define CURVED_RENDERING_COMMON

#include "UnityCG.cginc"
float4 _CurvedParam;
float _DecalHeightYParam;

float4 CalculateDecalCurvedViewPos(float4 vertex) {
	float4 vWorldPos = mul(unity_ObjectToWorld, float4(vertex.xyz, 1.0));
	vWorldPos.y = _DecalHeightYParam;
	float4 vPos = mul(UNITY_MATRIX_V, vWorldPos); //mul(UNITY_MATRIX_MV, vertex);
	float zOffX = vPos.x * _CurvedParam.y;
	float zOffY = vPos.y;
	vPos += float4(0, -_CurvedParam.x, 0, 0)*(zOffY*zOffY + zOffX * zOffX);

	return mul(UNITY_MATRIX_P, vPos);
}

float4 CalculateDecalCurvedViewPosDepth(float4 vertex) {
	float4 vWorldPos = mul(unity_ObjectToWorld, float4(vertex.xyz, 1.0));
	//avoid depth Z and decal Z fighting
	vWorldPos.y = _DecalHeightYParam - 0.001f;
	float4 vPos = mul(UNITY_MATRIX_V, vWorldPos); //mul(UNITY_MATRIX_MV, vertex);
	float zOffX = vPos.x * _CurvedParam.y;
	float zOffY = vPos.y;
	vPos += float4(0, -_CurvedParam.x, 0, 0)*(zOffY*zOffY + zOffX * zOffX);

	return mul(UNITY_MATRIX_P, vPos);
}

float4 CalculateCurvedViewPos(float4 vertex) {
	float4 vPos = mul(UNITY_MATRIX_V, mul(unity_ObjectToWorld, float4(vertex.xyz, 1.0))); //mul(UNITY_MATRIX_MV, vertex);
	float zOffX = vPos.x * _CurvedParam.y;
	float zOffY = vPos.y;
	vPos += float4(0, -_CurvedParam.x, 0, 0)*(zOffY*zOffY + zOffX * zOffX);

	return mul(UNITY_MATRIX_P, vPos);
}

float4 CalculateCurvedViewPosFromWorld(float3 vertex) {
	float4 vPos = mul(UNITY_MATRIX_V, float4(vertex.xyz, 1.0)); //mul(UNITY_MATRIX_MV, vertex);
	float zOffX = vPos.x * _CurvedParam.y;
	float zOffY = vPos.y;
	vPos += float4(0, -_CurvedParam.x, 0, 0)*(zOffY*zOffY + zOffX * zOffX);

	return mul(UNITY_MATRIX_P, vPos);
}

float4 CalculateCurvedViewPosWithOffset(float4 vertex, float3 offset) {
	float4 vWorldPos = mul(unity_ObjectToWorld, float4(vertex.xyz, 1.0));
	vWorldPos.xyz = vWorldPos.xyz + offset;
	float4 vPos = mul(UNITY_MATRIX_V, vWorldPos); //mul(UNITY_MATRIX_MV, vertex);
	float zOffX = vPos.x * _CurvedParam.y;
	float zOffY = vPos.y;
	vPos += float4(0, -_CurvedParam.x, 0, 0)*(zOffY*zOffY + zOffX * zOffX);

	return mul(UNITY_MATRIX_P, vPos);
}

float4 CalculateFacingCameraCurvedViewPos(float4 vertex) {

	float3 vpos = mul((float3x3)unity_ObjectToWorld, vertex.xyz);
	float4 worldCoord = float4(0, 0, 0, 1);
	float4 viewPos = mul(UNITY_MATRIX_MV, worldCoord) + float4(vpos, 0);

	float zOffX = viewPos.x * _CurvedParam.y;
	float zOffY = viewPos.y;
	viewPos += float4(0, -_CurvedParam.x, 0, 0)*(zOffY*zOffY + zOffX * zOffX);

	return mul(UNITY_MATRIX_P, viewPos);
}

float4 CalculateTestViewPos(float4 vertex) {
	float4 vPos = UnityObjectToClipPos(float4(vertex.xyz, 1.0)); //mul(UNITY_MATRIX_MV, vertex);

	return vPos;
}
#endif