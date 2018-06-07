#ifdef _WIN32

#include "types.h"
struct float16 { float f[16]; };

#endif

typedef union {
	float16 d16;
	float d[16];
	float dd[4][4];
} matrix;

matrix newmatrix(float16 vals)
{
	matrix m;
	m.d16 = vals;
	return m;
}

matrix matrix_translate(float3 loc)
{
	return newmatrix((float16)(
		1, 0, 0, loc.x,
		0, 1, 0, loc.y,
		0, 0, 1, loc.z,
		0, 0, 0, 1
	));
}

matrix matrix_scale(float3 scale)
{
	return newmatrix((float16)(
		scale.x, 0, 0, 0,
		0, scale.y, 0, 0,
		0, 0, scale.z, 0,
		0, 0, 0, 1
	));
}

matrix matrix_rotation_pitch(float angle)
{
	float c = cos(angle);
	float s = sin(angle);
	return newmatrix((float16)(
		c, s, 0, 0,
		-s, c, 0, 0,
		0, 0, 1, 0,
		0, 0, 0, 1
	));
}

matrix matrix_rotation_roll(float angle)
{
	float c = cos(angle);
	float s = sin(angle);
	return newmatrix((float16)(
		c, 0, s, 0,
		0, 1, 0, 0,
		-s, 0, c, 0,
		0, 0, 0, 1
	));
}


matrix matrix_rotation_yaw(float angle)
{
	float c = cos(angle);
	float s = sin(angle);
	return newmatrix((float16)(
		1, 0, 0, 0,
		0, c, s, 0,
		0, -s, c, 0,
		0, 0, 0, 1
	));
}

matrix mat_mul(matrix a, matrix b)
{
	matrix res;
	for (int i = 0; i < 4; ++i)
		for (int j = 0; j < 4; ++j) {
			res.dd[i][j] = 0;
			for (int k = 0; k < 4; ++k)
				res.dd[i][j] += a.dd[i][k] * b.dd[k][j];
		}
	return res;
}

float4 vec_mat_mul(float4 v, matrix m)
{
	union { float4 f; float vals[4]; } a, c;
	a.f = v;
	a.vals[3] = 1;

	for (int i = 0; i < 3; i++) {
		c.vals[i] = 0;
		for (int j = 0; j < 4; j++)
			c.vals[i] += m.dd[i][j] * a.vals[j];
	}

	return c.f;
}

matrix matrix_rotation(float3 angle)
{
	return mat_mul(mat_mul(
		matrix_rotation_pitch(angle.z), 
		matrix_rotation_yaw(angle.x)),
		matrix_rotation_roll(angle.y)
	);
}

typedef struct {
	float4 loc, rot, scale;
} Transform;

matrix get_transform_matrix(const Transform t)
{
	return mat_mul(mat_mul(
		matrix_translate(t.loc.xyz),
		matrix_rotation(t.rot.xyz)),
		matrix_scale(t.scale.xyz)
	);
}