#ifdef _WIN32

#include "matrix.h"

#endif

ushort min2(ushort a, ushort b)
{
	return a < b ? a : b;
}
ushort max2(ushort a, ushort b)
{
	return a > b ? a : b;
}
ushort min3(ushort a, ushort b, ushort c)
{
	return (a < b && a < c ? a : (b < c ? b : c));
}
ushort max3(ushort a, ushort b, ushort c)
{
	return (a > b && a > c ? a : (b > c ? b : c));
}

VertexOut clip(VertexOut a, VertexOut b)
{
	half16 res = a.f16 + (b.f16 - a.f16) * ((1 - a.loc.z) / (b.loc.z - a.loc.z));
	return *(VertexOut*)&res;
}
void swap3(VertexOut *a, VertexOut *b, VertexOut *c)
{
	VertexOut tmp = *a;
	*a = *b;
	*b = *c;
	*c = tmp;
}

half16 projectVert(Vertex vert, matrix transform, matrix worldrot)
{
	vert.loc = vec_mat_mul(vert.loc, transform);
	vert.normal = vec_mat_mul(vert.normal.xyzz, worldrot).xyz;
	vert.tangent = vec_mat_mul(vert.tangent.xyzz, worldrot).xyz;
	vset3(&vert, vec_mat_mul(binormal(vert).xyzz, worldrot).xyz, 13);
	
	//vert.vals[1] *= -1;

	return convert_half16(vert.f16);
}



typedef enum {
	None = 0,
	Normal = 1, 
	Culled = 2, 
	Invis = 3
} TriStatus;
TriStatus Clip(Triangle* tri)
{
	int vis0 = tri->v[0].loc.z >= 1;
	int vis1 = tri->v[1].loc.z >= 1;
	int vis2 = tri->v[2].loc.z >= 1;

	if (dot(tri->v[0].loc, cross(tri->v[1].loc - tri->v[0].loc, tri->v[2].loc - tri->v[0].loc)) >= 0)
		return Invis;
	else if (vis0 + vis1 + vis2 == 0)
		return Invis;
	else if (vis0 + vis1 + vis2 == 1)
	{
		int iVert = vis0 ? 0 : (vis1 ? 1 : 2);
		for (int i = 1; i < 3; i++)
			tri->v[(iVert + i) % 3] = clip(tri->v[iVert], tri->v[(iVert + i) % 3]);
		return Normal;
	}
	else if (vis0 + vis1 + vis2 == 2)
	{
		int iVert = !vis0 ? 0 : (!vis1 ? 1 : 2);

		if (iVert != 2)
		{
			swap3(tri->v, tri->v + 1, tri->v + 2);
			if (iVert == 1)
				swap3(tri->v, tri->v + 1, tri->v + 2);
		}

		tri->v[3] = clip(tri->v[1], tri->v[2]);
		tri->v[2] = clip(tri->v[0], tri->v[2]);

		return Culled;
	}
	else return Normal;


}

kernel void project(global Vertex* vertices, global uint* indices, global Triangle* triangles, global Transform* transformValues, global WorldSettings* worldSettings)
{
	if (get_global_id(0) == 0)
		worldSettings->triangleCount = get_global_size(0);

	//local TriStatus status[triPerPack];
	//local Triangle triO[triPerPack];

	half2 screenSize = convert_half2(worldSettings->camera.screenSize);
	matrix transform = get_transform_matrix(*transformValues);
	matrix worldrot = matrix_rotation(transformValues->rot.xyz);
	
	int id = get_global_id(0);
	int lId = get_local_id(0) % triPerPack;

	Triangle tri;
	//#define tri triO[lId]

	tri.v[0].f16 = projectVert(vertices[indices[id * 3 + 0]], transform, worldrot);
	tri.v[1].f16 = projectVert(vertices[indices[id * 3 + 1]], transform, worldrot);
	tri.v[2].f16 = projectVert(vertices[indices[id * 3 + 2]], transform, worldrot);

	tri.v[3].loc.z = -1;

	TriStatus _status = Clip(&tri);

	if (_status != Invis)
	{
		for (int i = 0; i < 3 + (_status == Culled); i++)
		{
			half z = tri.v[i].vals[2], w = 1 / z;
			tri.v[i].f16 *= w;
			tri.v[i].loc.y = -tri.v[i].loc.y;
#ifdef _WIN32
			tri.v[i].loc.x = (float)(tri.v[i].loc.x)*screenSize.x*0.5 + screenSize.x * 0.5f - 0.5f;
			tri.v[i].loc.y = (float)(tri.v[i].loc.y)*screenSize.y*0.5 + screenSize.y * 0.5f - 0.5f;
			tri.v[i].loc.z = (z - 0.5) / (200 - 1);
			tri.v[i].loc.w = w;
#else
			tri.v[i].loc = (half4)(tri.v[i].loc.xy*screenSize*0.5 + screenSize * 0.5f - (half2)0.5f, (z - 1) / (200 - 1), w);
#endif
		}
	}
	else
	{
		tri.v[0].f16 = (half16)0.f;
		tri.v[1].f16 = (half16)0.f;
		tri.v[2].f16 = (half16)0.f;
		tri.v[3].f16 = (half16)0.f;
	}

	triangles[get_global_id(0)] = tri;

#undef tri


	/*
	status[lId] = _status;

	int triTotal = id + 1 == get_global_size(0) ? get_global_size(0) % triPerPack : triPerPack;
	if (lId == triTotal - 1)
	{
		int x = 64;
		do
		{
			x = 0;
			for (int i = 0; i < triTotal; i++)
				x += (status[i] != None);
		} while (x != triTotal);
		
		TriPack o;
		o.tCount = 0;
		o.pmin = (ushort2)(10000, 10000);
		o.pmax = (ushort2)(0, 0);
		for (int i = 0; i < triTotal; i++)
			if (status[i] != Invis)
			{
				o.tri[o.tCount++] = triO[i];
				o.pmin.x = min3(min2(triO[i].v[0].loc.x, o.pmin.x), status[i] == Culled ? min2(triO[i].v[1].loc.x, triO[i].v[3].loc.x) : (ushort)triO[i].v[1].loc.x, triO[i].v[2].loc.x);
				o.pmin.y = min3(min2(triO[i].v[0].loc.y, o.pmin.y), status[i] == Culled ? min2(triO[i].v[1].loc.y, triO[i].v[3].loc.y) : (ushort)triO[i].v[1].loc.y, triO[i].v[2].loc.y);
				o.pmax.x = max3(max2(triO[i].v[0].loc.x, o.pmax.x), status[i] == Culled ? max2(triO[i].v[1].loc.x, triO[i].v[3].loc.x) : (ushort)triO[i].v[1].loc.x, triO[i].v[2].loc.x);
				o.pmax.y = max3(max2(triO[i].v[0].loc.y, o.pmax.y), status[i] == Culled ? max2(triO[i].v[1].loc.y, triO[i].v[3].loc.y) : (ushort)triO[i].v[1].loc.y, triO[i].v[2].loc.y);
			}

		triangles[id%triPerPack] = o;
		worldSettings->triangleCount = (get_global_size(0) + triPerPack - 1) / triPerPack;
	}*/


}
/*





float2 screenSize = worldSettings->camera.screenSize;
matrix transform = get_transform_matrix(*transformValues);
matrix worldrot = matrix_rotation(transformValues->rot.xyz);

Vertex out = vertices[get_global_id(0)];

out.loc = vec_mat_mul(out.loc.xyzz, transform);
out.normal = vec_mat_mul(normal(out).xyzz, worldrot).xyz;
out.tangent = vec_mat_mul(tangent(out).xyzz, worldrot).xyz;
vset3(&out, vec_mat_mul(binormal(out).xyzz, worldrot).xyz, 13);

float z = out.vals[2], w = 1 / z;

out.f16 *= w;

out.loc = (float4)(out.loc.xy*screenSize + screenSize * 0.5f, (z - 0.5) / (200 - 0.5), w);

outVertices[get_global_id(0)] = out;















	indices += get_global_id(0) * 3;
	float2 screenSize = _worldSettings->camera.screenSize;

	matrix transform = get_transform_matrix(*transformValues);
	matrix worldrot = matrix_rotation(transformValues->rot.xyz);

	Triangle t;
	for (int i = 0; i < 3; i++) {
		t.edges[i].vertex = vertices[indices[i]];
		t.edges[i].vertex.loc = vec_mat_mul(t.edges[i].vertex.loc, transform);
		t.edges[i].vertex.normal = vec_mat_mul(t.edges[i].vertex.normal, worldrot);

		float4 pt = t.edges[i].vertex.loc;
		t.edges[i].screenPos = (float4)(pt.xy / pt.z * screenSize + screenSize * 0.5f, (pt.z - 1) / (200 - 1), 1 / pt.z);

		t.edges[i].vertex.normal *= t.edges[i].screenPos.w;
		t.edges[i].vertex.uv *= t.edges[i].screenPos.w;
	}

	Vertex p0 = t.edges[0].vertex,
		p1 = t.edges[1].vertex,
		p2 = t.edges[2].vertex;

	if (cross(t.edges[1].screenPos - t.edges[0].screenPos, t.edges[2].screenPos - t.edges[0].screenPos).z <= 0)
	{
		t.isClockwise = false;
		triangles[get_global_id(0)] = t;
	}

	t.isClockwise = true;

	t.pmin = (ushort2)(
		min3(max(0.f, t.edges[0].screenPos.x), max(0.f, t.edges[1].screenPos.x), max(0.f, t.edges[2].screenPos.x)),
		min3(max(0.f, t.edges[0].screenPos.y), max(0.f, t.edges[1].screenPos.y), max(0.f, t.edges[2].screenPos.y))
	);

	t.pmax = (ushort2)(
		max3(max(0.f, t.edges[0].screenPos.x), max(0.f, t.edges[1].screenPos.x), max(0.f, t.edges[2].screenPos.x)),
		max3(max(0.f, t.edges[0].screenPos.y), max(0.f, t.edges[1].screenPos.y), max(0.f, t.edges[2].screenPos.y))
	);

	//for (int i = 0; i < 3; i++)
	//	t.edges[i].vals = (t.edges[(i + 1) % 3].screenPos.yx - t.edges[i].screenPos.yx);


	triangles[get_global_id(0)] = t;
	_worldSettings->triangleCount = get_global_size(0);
}
*/