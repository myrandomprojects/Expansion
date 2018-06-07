#ifdef _WIN32

#include "matrix.h"

#endif

float3 pixelShader(const Vertex v, const WorldSettings* worldSettings)
{
	//float4 cameralight = reflect(worldSettings->lightDir, v.normal);
	//float bright = clamp(-cameralight.z, 0.1f, 1.f);

	float3 basecolor = (float3)(0.9, 0.94, 0.96);

	float ambient = clamp(-dot(worldSettings->lightDir, v.normal.xyz), 0.1f, 1.f);

	return basecolor * (ambient + 0.1f);
}

float edgeFunction(float4 a, float4 b, float4 c)
{
	return (c.x - a.x) * (b.y - a.y) - (c.y - a.y) * (b.x - a.x);
}

void rasterizeOne(const Triangle t, const int2 pos, Pixel* pix, const WorldSettings* worldSettings)
{
	float area = 1 / edgeFunction(t.edges[0].screenPos, t.edges[1].screenPos, t.edges[2].screenPos);

	float w[3];
	int c = 0;
	for (int i = 0; i < 3; i++)
	{
		w[i] = edgeFunction(t.edges[(i + 1) % 3].screenPos, t.edges[(i + 2) % 3].screenPos, convert_float4(pos.xyxy));
		//(pos.x - t.edges[i].screenPos.x)*t.edges[i].vals.x - (pos.y - t.edges[i].screenPos.y)*t.edges[i].vals.y;
		if (w[i] > 0)
			c++;
		else {
			c--;
			w[i] = -w[i];
		}

		w[i] *= area;
	}

	if (c != 3) //  && c != -3
		return;

	half z = w[0] * t.edges[0].screenPos.z + w[1] * t.edges[1].screenPos.z + w[2] * t.edges[2].screenPos.z;

	if (z >= pix->depth)
		return;

	//float c0[3] = { 255, 0, 0 };
	//float c1[3] = { 0, 255, 0 };
	//float c2[3] = { 0, 0, 255 };

	Vertex v = vadd(vadd(
		vmul(t.edges[0].vertex, w[0]),
		vmul(t.edges[1].vertex, w[1])),
		vmul(t.edges[2].vertex, w[2]));

	//float r = w[0] * c0[0] + w[1] * c1[0] + w[2] * c2[0];
	//float g = w[0] * c0[1] + w[1] * c1[1] + w[2] * c2[1];
	//float b = w[0] * c0[2] + w[1] * c1[2] + w[2] * c2[2];

	float3 color = pixelShader(v, worldSettings);

	pix->color = convert_uchar4((float4)(clamp(color.x * 255, 0.f, 255.f), clamp(color.y * 255, 0.f, 255.f), clamp(color.z * 255, 0.f, 255.f), 255.f));
	pix->depth = z;
}

kernel void rasterize(global RenderTexture* canvas, global WorldSettings* _worldSettings, global Triangle* t, global uint* tCount)
{
	const int2 pos = { get_global_id(0), get_global_id(1) };
	global Pixel* pix = getPixel(canvas, pos);

	Pixel _pix = *pix;
	WorldSettings worldSettings = *_worldSettings;
	for (uint i = 0; i < *tCount; i++)
		rasterizeOne(t[i], pos, &_pix, &worldSettings);

	*pix = _pix;
}
