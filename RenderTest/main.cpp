#include <iostream>

#include "../Expansion_CSharp/Resources/types.h"
#include "../Expansion_CSharp/Resources/matrix.h"


using namespace std;

typedef enum {
	None = 0,
	Normal = 1,
	Culled = 2,
	Invis = 3
} TriStatus;

typedef struct {
	half2 samples[17];
	int count;
} UVSamples;

void project(global Vertex* vertices, global uint* indices, global Triangle* triangles, global Transform* transformValues, global WorldSettings* worldSettings);
int FindNearestVertex(VertexOut* vOut, UVSamples* uvSamples, global Pixel* pix, global Triangle* triangles, int count);
TriStatus Clip(Triangle* tri);

void test0()
{
	float v[] = {
		0, 1, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		-2,0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
	};

	TriStatus s = Clip((Triangle*)v);

	float3 pt0(v[0], v[1], v[2]);
	float3 pt1(v[16], v[17], v[18]);
	float3 pt2(v[32], v[33], v[34]);

	if (pt0 != float3(0, 0, 4) || pt1 != float3(1.5, 0, 1) || pt2 != float3(-1.5, 0, 1))
		cout << "[FAIL] test 0 !" << endl;

	return;
}
void test1()
{
	float v[] = {
		2, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		-2,0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
	};

	TriStatus s = Clip((Triangle*)v);

	float3 pt0(v[0], v[1], v[2]);
	float3 pt1(v[16], v[17], v[18]);
	float3 pt2(v[32], v[33], v[34]);
	float3 pt3(v[48], v[49], v[50]);

	if (pt0 != float3(-2, 0, 4) || pt1 != float3(2, 0, 4) || pt2 != float3(-0.5, 0, 1) || pt3 != float3(0.5, 0, 1))
		cout << "[FAIL] test 1 !" << endl;

	return;
}

void test2()
{
	VertexOut o;
	UVSamples s;
	Pixel p;

	float v[] = {
		200, 200, 0.5, 0.25, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		400, 400, 0.5, 0.25, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0,
		200, 400, 0.5, 0.25, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
	};

	FindNearestVertex(&o, &s, &p, (Triangle*)v, 1);

	return;
}


void test3()
{
	float v[] = {
		-0.5, 0.5, 1.2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0.5, 0.5, 1.2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		-0.5, -0.5, 1.2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
	};

	uint ind[] = { 0,1,2 };

	Triangle tri;

	Transform t = {
		{0, 0, 0, 0},
		{0, 0, 0, 0},
		{1, 1, 1, 1}
	};

	WorldSettings w;
	w.camera.screenSize = float2(640);

	project((Vertex*)v, ind, &tri, &t, &w);


	VertexOut o;
	UVSamples s;
	Pixel p;

	FindNearestVertex(&o, &s, &p, &tri, 1);


	return;
}


int main()
{
	test0();
	test1();

	test2();

	test3();
	
}

/*

float v[] = {
335.6357, 252.7856, 0.0324803, 0.133984, 0.0909524, -0.06443622, 0.07434606, 0, 0, 0.133984, 0, 0.133984, 0, 0, 0, 0,
403.9105, 267.765, 0.02912324, 0.1471557, 0.09989377, -0.07077083, 0.08165489, 0, 0.1471557, 0.1471557, 0, 0.1471557, 0, 0, 0, 0,
350.261, 324.9998, 0.03497168, 0.1256382, 0.08528703, -0.06042253, 0.06971509, 0, 0, 0, 0, 0.1256382, 0, 0, 0, 0,
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
};

uint ind[] = { 0,1,2 };
TriPack tri;

int a = sizeof(float2);

WorldSettings s;
s.camera.screenSize = float2(640);

VertexOut vOut;
half2 uvSamples[16]; int sCount;
Pixel p;

FindNearestVertex(&vOut, uvSamples, &sCount, &p, (Triangle*)v, 1);
*/