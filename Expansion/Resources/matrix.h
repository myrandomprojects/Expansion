#ifdef _WIN32

#include "types.h"
//struct float16 { float f[16]; };

#define MM {
#define _MM }

#else
#define MM (float16)(
#define _MM )
#endif

typedef union MMMM {
	float16 d16;
	float d[16];
	float dd[4][4];
#ifdef _WIN32
	MMMM() {}
	template<class... Targs>
	MMMM(Targs... args)
	{
		float dummy[sizeof...(args)] = { args... };
		for (int i = 0; i < sizeof...(args); ++i)
			d[i] = dummy[i];
	}
#endif
} matrix;

inline matrix newmatrix(float16 vals)
{
	matrix m;
	m.d16 = vals;
	return m;
}
inline matrix matrix_identity()
{
	return newmatrix( MM
		1.f, 0.f, 0.f, 0.f,
		0.f, 1.f, 0.f, 0.f,
		0.f, 0.f, 1.f, 0.f,
		0.f, 0.f, 0.f, 1.f
	_MM);
}

inline matrix mat_mul(matrix a, matrix b)
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

inline matrix matrix_translate(float3 loc)
{
	return newmatrix(MM
		1.f, 0.f, 0.f, loc.x,
		0.f, 1.f, 0.f, loc.y,
		0.f, 0.f, 1.f, loc.z,
		0.f, 0.f, 0.f, 1.f
	_MM);
}

inline matrix matrix_scale(float3 scale)
{
	return newmatrix(MM
		(float)scale.x, 0.f, 0.f, 0.f,
		0.f, (float)scale.y, 0.f, 0.f,
		0.f, 0.f, (float)scale.z, 0.f,
		0.f, 0.f, 0.f, 1.f
	_MM);
}

inline matrix matrix_rotation_pitch(float angle)
{
	float c = cos(angle);
	float s = sin(angle);
	return newmatrix(MM
		c, s, 0.f, 0.f,
		-s, c, 0.f, 0.f,
		0.f, 0.f, 1.f, 0.f,
		0.f, 0.f, 0.f, 1.f
	_MM);
}
inline matrix matrix_rotation_roll(float angle)
{
	float c = cos(angle);
	float s = sin(angle);
	return newmatrix(MM
		c, 0.f, s, 0.f,
		0.f, 1.f, 0.f, 0.f,
		-s, 0.f, c, 0.f,
		0.f, 0.f, 0.f, 1.f
	_MM);
}
inline matrix matrix_rotation_yaw(float angle)
{
	float c = cos(angle);
	float s = sin(angle);
	return newmatrix(MM
		1, 0, 0, 0,
		0, c, s, 0,
		0, -s, c, 0,
		0, 0, 0, 1
	_MM);
}

inline matrix matrix_lookat(float3 target, float3 camloc, float3 up)
{
	float3 z = normalize(target - camloc);
	float3 x = normalize(cross(up, z));
	float3 y = normalize(cross(z, x));
	matrix minv = matrix_identity();
	matrix tr = matrix_identity();

#define vi(v, i)(i==0 ? (float)v.x : (i==1 ? (float)v.y : (float)v.z))
	for (int i = 0; i < 3; i++) {
		minv.dd[0][i] = vi(x, i);
		minv.dd[1][i] = vi(y, i);
		minv.dd[2][i] = vi(z, i);
		tr.dd[i][3] = -vi(camloc, i);
	}
#undef vi
	return mat_mul(minv, tr);
}

inline float4 vec_mat_mul(float4 v, matrix m)
{
	float4 res;
	float* vp = (float*)&v;
	float* vr = (float*)&res;
	vp[3] = 1;

	for (int i = 0; i < 3; i++) {
		vr[i] = 0;
		for (int j = 0; j < 4; j++)
			vr[i] += m.dd[i][j] * vp[j];
	}

	return res;
}

inline matrix matrix_rotation(float3 angle)
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

inline matrix get_transform_matrix(const Transform t)
{
	matrix tr = matrix_translate(t.loc.xyz);
	matrix rot = matrix_rotation(t.rot.xyz);
	matrix scale = matrix_scale(t.scale.xyz);

	return mat_mul(mat_mul(
		tr,
		rot),
		scale
	);
}

