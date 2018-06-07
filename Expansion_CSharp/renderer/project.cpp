#ifdef _WIN32

#include "matrix.h"

#endif

kernel void project(global Vertex* vertices, global uint* indices, global Transform* transformValues, global WorldSettings* _worldSettings, global Triangle* triangles)
{
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
		t.edges[i].screenPos = (float4)(pt.xy / pt.z * screenSize + screenSize * 0.5f, (pt.z - 1) / (8 - 1), pt.z);
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

	for (int i = 0; i < 3; i++)
		t.edges[i].vals = (t.edges[(i + 1) % 3].screenPos.yx - t.edges[i].screenPos.yx);



	triangles[get_global_id(0)] = t;
}
