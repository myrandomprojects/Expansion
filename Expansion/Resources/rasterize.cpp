#ifdef _WIN32

#include "types.h"

#endif

#define COLOR(r,g,b) (float3)(r,g,b)

float fmin3(float a, float b, float c)
{
	return (a < b && a < c ? a : (b < c ? b : c));
}

float fmax3(float a, float b, float c)
{
	return (a > b && a > c ? a : (b > c ? b : c));
}





typedef struct {
	float2 samples[17];
	int count;
} UVSamples;


float3 sampleTexture(read_only image2d_t tex, const float2 uv)
{
	const sampler_t texsampler = CLK_NORMALIZED_COORDS_TRUE | CLK_ADDRESS_REPEAT | CLK_FILTER_NEAREST;

	float3 color = convert_float3(read_imageui(tex, texsampler, uv).zyx) * (1.f / 255);

	return color;
}

float3 sampleTextureWithFilter(read_only image2d_t tex, const UVSamples* samples)
{
	float3 color = sampleTexture(tex, samples->samples[0]) * 3.5f;
	for (int i = 1; i < samples->count; i++)
		color += sampleTexture(tex, samples->samples[i]);
	return color * (1 / (samples->count + 2.5f));
}

float4 reflect(float4 d, float4 n) { return d - n * (dot(d, n) * 2); }
/*
float3 pixelShader(const Vertex v, const WorldSettings* worldSettings, read_only image2d_t basetex, read_only image2d_t normaltex)
{
	//float4 cameralight = reflect(worldSettings->lightDir, v.normal);
	//float bright = clamp(-cameralight.z, 0.1f, 1.f);

	float3 basecolor = sampleTexture(basetex, v.uv); //(float3)(0.9, 0.94, 0.96);

	float ambient = clamp(-dot(worldSettings->lightDir, v.normal.xyz), 0.1f, 1.f);

	return basecolor * (ambient + 0.1f);
}





		float left = fmin3(v[0].vals[0], v[1].vals[0], v[2].vals[0]);
		if (pos.x < left)
			continue;
		float top = fmin3(v[0].vals[1], v[1].vals[1], v[2].vals[1]);
		if (pos.y < top)
			continue;
		float right = fmax3(v[0].vals[0], v[1].vals[0], v[2].vals[0]);
		if (pos.x > right)
			continue;
		float bottom = fmax3(v[0].vals[1], v[1].vals[1], v[2].vals[1]);
		if (pos.y > bottom)
			continue;





*/






void vSwap(VertexOut* a, VertexOut* b)
{
	VertexOut t = *a;
	*a = *b;
	*b = t;
}

half edgeFunction(half4 a, half4 b, half4 c)
{
	return (c.x - a.x) * (b.y - a.y) - (c.y - a.y) * (b.x - a.x);
}

int TryFind(VertexOut* vOut, UVSamples* uvSamples, Pixel* pix, Triangle tri)
{
	const half2 pos = { get_global_id(0), get_global_id(1) };

	if (dot(tri.v[0].loc, tri.v[0].loc) == 0)
		return 0;
	//if (pos.x < pack.pmin.x || pos.y < pack.pmin.y || pos.x > pack.pmax.x || pos.y > pack.pmax.y)
	//	continue;

	//for (uint i = 0; i < pack.tCount; i++)
	{
		VertexOut* v = tri.v;

		half w[3];

		w[0] = -edgeFunction(v[1].loc, v[2].loc, pos.xyxy);
		w[1] = -edgeFunction(v[2].loc, v[0].loc, pos.xyxy);
		w[2] = -edgeFunction(v[0].loc, v[1].loc, pos.xyxy);

		if (w[0] < 0 || w[1] < 0 || w[2] < 0)
		{
			if (v[3].loc.z == 99999)
				return 0;

			v++;
			vSwap(v + 1, v + 2);

			// Setup.bat -exclude=Linux -exclude=IOS -exclude=Mac -exclude=HTML5 -exclude=Android -exclude=TVOS -exclude=Win32
			w[0] = -edgeFunction(v[1].loc, v[2].loc, pos.xyxy);
			w[1] = -edgeFunction(v[2].loc, v[0].loc, pos.xyxy);
			w[2] = -edgeFunction(v[0].loc, v[1].loc, pos.xyxy);

			if (w[0] < 0 || w[1] < 0 || w[2] < 0)
				return 0;
		}

		half areaInv = 1 / -edgeFunction(v[0].loc, v[1].loc, v[2].loc);
		w[0] *= areaInv;
		w[1] *= areaInv;
		w[2] *= areaInv;

		float zCorr = 1 / (w[0] * v[0].loc.w + w[1] * v[1].loc.w + w[2] * v[2].loc.w);
		half z = (w[0] * v[0].vals[2] + w[1] * v[1].vals[2] + w[2] * v[2].vals[2]);

		if (z >= pix->depth)
			return 0;

		pix->depth = z;

		//zCorr = 1;

		z *= zCorr;

		vOut->f16 = v[0].f16*w[0] + v[1].f16*w[1] + v[2].f16*w[2];

		half4 l = vOut->loc;
		vOut->f16 *= zCorr;
		vOut->loc = l;

		uvSamples->samples[0] = convert_float2(vOut->uv);
		uvSamples->count = 1;

		float values[] = { -0.375, -0.125, 0.125, 0.375 };
		for (int di = 0; di < ARRAYSIZE(values); di++)
			for (int dj = 0; dj < ARRAYSIZE(values); dj++)
			{
				half2 tPos = pos + (half2)(values[di], values[dj]);

				w[0] = areaInv * -edgeFunction(v[1].loc, v[2].loc, tPos.xyxy);
				w[1] = areaInv * -edgeFunction(v[2].loc, v[0].loc, tPos.xyxy);
				w[2] = areaInv * -edgeFunction(v[0].loc, v[1].loc, tPos.xyxy);
				if (w[0] < 0 || w[1] < 0 || w[2] < 0)
					continue;

				uvSamples->samples[uvSamples->count++] = convert_float2((v[0].uv*w[0] + v[1].uv*w[1] + v[2].uv*w[2])) * zCorr;
			}
		//*/

		return 1;
	}
}

int FindNearestVertex(VertexOut* vOut, UVSamples* uvSamples, Pixel* pix, global const Triangle* triangles, int count, global const Batch* batches)
{
	int vFound = 0;

	const int2 pt = (int2)(get_global_id(0) / 16, get_global_id(1) / 16);
	const int2 screenSize = (int2)((get_global_size(0)+15) / 16, (get_global_size(1) + 15) / 16);
	const int binId = pt.y * screenSize.x + pt.x;
	
	local Batch batch;

	for (uint pI = 0; pI < count; pI++)
	{
		int primId = pI % 255;

		if (primId == 0)
		{
			barrier(CLK_LOCAL_MEM_FENCE);
			event_t event = async_work_group_copy(
				(local char*)&batch,
				(global const char*)(batches + (count + 254) / 255 * binId + pI / 255),
				sizeof(Batch),
				0
			);
			wait_group_events(1, &event);
		}

		if (pI % 255 == 0)
		{
			if ((batch.bytes[31] & 128) == 0)
			{
				pI += 254;
				continue;
			}
		}

		if (batch.bytes[primId / 8] & (1 << primId % 8))
			vFound += TryFind(vOut, uvSamples, pix, triangles[pI]);

		//TriPack pack = tri[pI];
		//Triangle tri = triangles[pI];

	}

	return vFound;
}

#define sample(t) sampleTextureWithFilter(t, &s)
#define sampleNormal(t) (sampleTextureWithFilter(t, &s)*2-(float3)1)

#define MAT_VARIABLES()			   \
	float3 BaseColor = (float3)1;  \
	float  Metallic = 0.5;         \
	float  Roughness = 0.5;        \
	float3 Normal = (float3)(0, 0, 1);

//WorldSettings worldSettings = *_worldSettings;

#define MAT_HEADER() \
	const int2 pos = { get_global_id(0), get_global_id(1) }; \
	Pixel pix = *getPixel(canvas, pos); \
	Vertex v;   \
	UVSamples s; s.count = 0; \
				\
	if (FindNearestVertex(&v,  &s, &pix, tri, worldSettings->triangleCount, batches)) \
	{ \
		MAT_VARIABLES()

#define MAT_FOOTER() \
		Normal = normal(v)*Normal.z + tangent(v)*Normal.x + binormal(v)*Normal.y; \
		float ambient = clamp(-dot(worldSettings->lightDir, Normal)*0.6f+0.4f, 0.1f, 1.f); \
																				 \
		float3 finalColor = BaseColor * (ambient); \
		pix.color = convert_uchar4((float4)(clamp(finalColor.x * 255, 0.f, 255.f), clamp(finalColor.y * 255, 0.f, 255.f), clamp(finalColor.z * 255, 0.f, 255.f), 255.f));\
		getPixel(canvas, pos)[0] = pix; \
	}
	
#define MAT_ARGS() global RenderTexture* canvas, global WorldSettings* worldSettings, global const Triangle* tri, global const Batch* batches //, global Triangle* tri 
#define MAT_FUNC kernel void 

/*

MAT_FUNC(ab)
{
	MAT_HEADER()




		// $BODY$



	

	
	MAT_FOOTER()
}

*/
/*
void rasterizeOne(const Triangle t, const int2 pos, Pixel* pix, const WorldSettings* worldSettings, read_only image2d_t basetex, read_only image2d_t normaltex)
{
	if (t.pmin.x > pos.x || t.pmin.y > pos.y || t.pmax.x < pos.x || t.pmax.y < pos.y)
		return;

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

	float zCorr = 1 / (w[0] * t.edges[0].screenPos.w + w[1] * t.edges[1].screenPos.w + w[2] * t.edges[2].screenPos.w);

	if (z*zCorr >= pix->depth)
		return;

	//float c0[3] = { 255, 0, 0 };
	//float c1[3] = { 0, 255, 0 };
	//float c2[3] = { 0, 0, 255 };

	Vertex v = vadd(vadd(
		vmul(t.edges[0].vertex, w[0]),
		vmul(t.edges[1].vertex, w[1])),
		vmul(t.edges[2].vertex, w[2]));

	v.normal *= zCorr;
	v.uv *= zCorr;

	//float r = w[0] * c0[0] + w[1] * c1[0] + w[2] * c2[0];
	//float g = w[0] * c0[1] + w[1] * c1[1] + w[2] * c2[1];
	//float b = w[0] * c0[2] + w[1] * c1[2] + w[2] * c2[2];

	float3 color = pixelShader(v, worldSettings, basetex, normaltex);

	pix->color = convert_uchar4((float4)(clamp(color.x * 255, 0.f, 255.f), clamp(color.y * 255, 0.f, 255.f), clamp(color.z * 255, 0.f, 255.f), 255.f));
	pix->depth = z * zCorr;
}

kernel void rasterize(global RenderTexture* canvas, global WorldSettings* _worldSettings, global Triangle* t, read_only image2d_t basetex, read_only image2d_t normaltex)
{
	const int2 pos = { get_global_id(0), get_global_id(1) };
	global Pixel* pix = getPixel(canvas, pos);

	Pixel _pix = *pix;
	WorldSettings worldSettings = *_worldSettings;
	for (uint i = 0; i < worldSettings.triangleCount; i++) // worldSettings.triangleCount
		rasterizeOne(t[i], pos, &_pix, &worldSettings, basetex, normaltex);

	*pix = _pix;
}
*/