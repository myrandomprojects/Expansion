#ifdef _WIN32

#include "matrix.h"

#endif

ushort min3(ushort a, ushort b, ushort c)
{
	return (a < b && a < c ? a : (b < c ? b : c));
}

ushort max3(ushort a, ushort b, ushort c)
{
	return (a > b && a > c ? a : (b > c ? b : c));
}

//, global Triangle* triangles
//, global uint* indices
kernel void project(global Vertex* vertices, global Vertex* outVertices, global Transform* transformValues, global WorldSettings* worldSettings)
{
	worldSettings->triangleCount = get_global_size(0) / 3;
	float2 screenSize = worldSettings->camera.screenSize;
	matrix transform = get_transform_matrix(*transformValues);
	matrix worldrot = matrix_rotation(transformValues->rot.xyz);

	Vertex out = vertices[get_global_id(0)];

	vset3(&out, vec_mat_mul(loc(out).xyzz, transform).xyz, 0);
	vset3(&out, vec_mat_mul(normal(out).xyzz, worldrot).xyz, 3);
	vset3(&out, vec_mat_mul(tangent(out).xyzz, worldrot).xyz, 6);
	vset3(&out, vec_mat_mul(binormal(out).xyzz, worldrot).xyz, 9);

	float z = out.vals[2];
	float w = 1 / z;
	out.f16 *= w;
	float2 xy = (float2)(out.vals[0], out.vals[1]);
	vset3(&out, (float3)(xy*screenSize + screenSize * 0.5f, (z - 0.5) / (200 - 0.5)), 0);
	out.vals[14] = w;

	outVertices[get_global_id(0)] = out;
}
/*
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