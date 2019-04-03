#ifdef _WIN32
#pragma once
#include <cmath>

#define kernel
#define local
#define global
#define read_only
#define write_only
#define get_global_id(i) 0
#define get_global_size(i) 1
#define get_local_id(i) 0
#define get_local_size(i) 0

#define CLK_LOCAL_MEM_FENCE
#define CLK_GLOBAL_MEM_FENCE
#define barrier(x)
#define async_work_group_copy(target, src, num_elements, out) 0
#define wait_group_events(n, event)

#define vstore4(v, offset, dest)

#define CLK_NORMALIZED_COORDS_TRUE 1
#define CLK_ADDRESS_NONE 2
#define CLK_ADDRESS_REPEAT 3
#define CLK_FILTER_NEAREST 4

#define xy _xy()
#define xyz _xyz()
#define zyx _zyx()
#define xyzz _xyzz()
#define xyzw _xyzz()
#define xyxy _xyxy()
#define zw _zw()

#define convert_float3(f) (*(float3*)&f)
#define convert_half16(f) (*(half16*)&f)
#define convert_half2(f) (*(half2*)&f)
#define convert_float2(f) (*(float2*)&f)

#define read_imageui(a,b,c) float4(255,255,255,255)

typedef unsigned char uchar;
typedef unsigned short ushort;
typedef unsigned int uint;
typedef unsigned long long ulong;
typedef float half;

typedef int event_t;

template<class T, int d, int D>
struct vValue
{
	vValue(T val)
	{
		if(d<D) 
			((T*)this)[d] = val;
	}
	T operator=(T val) { if (d < D) { ((T*)this)[d] = val; return *this; }return 0; }
	void operator+=(T val) { if (d < D)((T*)this)[d] += val; }
	void operator-=(T val) { if (d < D)((T*)this)[d] -= val; }
	operator T()const { if (d < D)return ((T*)this)[d]; return 0; }

	template<class TT, int dd, int DD>bool operator<(const vValue<TT, dd, DD> v)const { return ((T)*this) < (TT)v; }
	template<class TT, int dd, int DD>bool operator>(const vValue<TT, dd, DD> v)const { return ((T)*this) > (TT)v; }
};

template<class T, int dim>
struct vec
{
	using V = vec<T, dim>;
	union {
		vValue<T, 0, dim> x;
		vValue<T, 1, dim> y;
		vValue<T, 2, dim> z;
		vValue<T, 3, dim> w;
		T v[dim];
	};

	vec() { x = 0; y = 0; z = 0; w = 0; }
	vec(T v) { x = v; y = v; z = v; w = v; }
	vec(T x, T y) { this->x = x; this->y = y; }
	vec(T x, T y, T z) { this->x = x; this->y = y; this->z = z; }
	vec(T x, T y, T z, T w) { this->x = x; this->y = y; this->z = z; this->w = w; }
	vec(vec<T, 2> vxy, T z, T w) { x = vxy.x; y = vxy.y; this->z = z; this->w = w; }

	vec<T, 2> _xy()const { return { x, y }; }
	vec<T, 3> _xyz()const { return { x, y, z }; }
	vec<T, 3> _zyx()const { return { z, y, x }; }
	vec<T, 4> _xyzz()const { return { x, y, z, z }; }
	vec<T, 4> _xyzw()const { return { x, y, z, w }; }
	vec<T, 4> _xyxy()const { return { x, y, x, y }; }
	vec<T, 2> _zw()const { return { z, w }; }

	V operator+(V v)const { return { x + v.x,y + v.y,z + v.z,w + v.w }; }
	V operator-(V v)const { return { x - v.x,y - v.y,z - v.z,w - v.w }; }
	V operator*(float f)const { return { x*f,y*f,z*f,w*f }; }
	V operator*(V f)const { return { x*f.x,y*f.y,z*f.z,w*f.w }; }

	V operator+=(const V v) { x += v.x; y += v.y; z += v.z; w += v.w; return *this; }

	bool operator==(const V v)const { return x == v.x&&y == v.y&&z == v.z&&w == v.w; }
	bool operator!=(const V v)const { return x != v.x||y != v.y||z != v.z||w != v.w; }
};

using float2 = vec<float, 2>;
using float3 = vec<float, 3>;
using float4 = vec<float, 4>;
using half2 = vec<float, 2>;
using half3 = vec<float, 3>;
using half4 = vec<float, 4>;
using uchar2 = vec<uchar, 2>;
using uchar3 = vec<uchar, 3>;
using uchar4 = vec<uchar, 4>;
using ushort2 = vec<ushort, 2>;
using ushort3 = vec<ushort, 3>;
using ushort4 = vec<ushort, 4>;
using int2 = vec<int, 2>;
using int4 = vec<int, 4>;
using uint4 = vec<uint, 4>;
using ulong4 = vec<ulong, 4>;

/*
struct int2 { int x, y; };
struct float2 { float x, y; };
struct float3 { float x, y, z; float3(float x, float y, float z):x(x),y(y),z(z) {} float4 _xyzz() { return { x,y,z,z }; } };
struct float4 { float x, y, z, w; float4(float x, float y, float z, float w = 0) :x(x), y(y), z(z), w(w) {}  float3 _xyz() { return { x,y,z }; } float4 operator-(float4 b) { return { x - b.x,y - b.y,z - b.z,w - b.w }; } };
struct half2 { float x, y; };
struct half3 { float x, y, z; half3(float x, float y, float z) :x(x), y(y), z(z) {} };
struct half4 { float x, y, z, w; half4(float x, float y, float z, float w = 0) :x(x), y(y), z(z), w(w) {} half2 _xy() { return { x,y }; } half4 operator-(half4 b) { return { x - b.x,y - b.y,z - b.z,w - b.w }; } };
struct uchar4 { uchar x, y, z, w; };
struct ushort2 { ushort x, y; };
struct uint4 { uint x, y, z, w; };
*/
struct float16 { 
	float f[16];
	float16() { for (int i = 0; i < 16; i++) f[i] = 0; }
	float16(float v) { for (int i = 0; i < 16; i++) f[i] = v; }

	template<class... T>
	float16(T... args) {
		float dummy[sizeof...(args)] = { args... };
		for (int i = 0; i < sizeof...(args); ++i)
			f[i] = dummy[i];
	}
	
	void operator*=(float w)
	{
		for (int i = 0; i < 16; i++)
			f[i] *= w;
	}
	void operator+=(float16 w) 
	{ 
		for (int i = 0; i < 16; i++)
			f[i] += w.f[i]; 
	} 
	void operator-=(float16 w)
	{ 
		for (int i = 0; i < 16; i++)
			f[i] -= w.f[i];
	}
	float16 operator+(float16 v) { float16 res; for (int i = 0; i < 16; i++)res.f[i] = f[i] + v.f[i]; return res; }
	float16 operator*(float w) { float16 res = *this; res *= w; return res; }
};
struct half16 { 
	float f[16];
	half16() { for (int i = 0; i < 16; i++) f[i] = 0; }
	half16(float v) { for (int i = 0; i < 16; i++) f[i] = v; }
	half16 operator+(half16 v) { half16 res; for (int i = 0; i < 16; i++)res.f[i] = f[i] + v.f[i]; return res; } 
	half16 operator-(half16 v) { half16 res; for (int i = 0; i < 16; i++)res.f[i] = f[i] - v.f[i]; return res; }
	void operator*=(float w) { for (int i = 0; i < 16; i++)f[i] *= w; }
	half16 operator*(float w) { half16 res = *this; res *= w; return res; } 
};
typedef uint sampler_t;

template<class T>
T cross(T a, T b)
{
	return T(a.y * b.z - a.z * b.y,
		a.z * b.x - a.x * b.z,
		a.x * b.y - a.y * b.x);
}

template<class T>
T dot(vec<T, 4> a, vec<T, 4> b)
{
	return a.x*b.x + a.y*b.y + a.z*b.z + a.w*b.w;
}

template<class T, int dim>
T length(vec<T, dim> v)
{
	return sqrt(v.x*v.x + v.y*v.y + v.z*v.z + v.w*v.w);
}
template<class T>
T length(vec<T, 2> v)
{
	return sqrt(v.x*v.x + v.y*v.y);
}
template<class T>
T length(vec<T, 3> v)
{
	return sqrt(v.x*v.x + v.y*v.y + v.z*v.z);
}

template<class T, int dim>
vec<T, dim> normalize(vec<T, dim> v)
{
	return v * (1 / length(v));
}

#define COORD(x,y) int2(x,y)
typedef uchar4 image2d_t[100];

void write_imageui(image2d_t, int2, uint4);

#define CNTR(n) n() {}
#else
#define COORD(x,y) (int2)(x,y)
#define CNTR(n) 
#endif

#define ARRAYSIZE(a) (sizeof(a)/sizeof(a[0]))

typedef unsigned char byte;

typedef struct {
	uchar4 color;
	half depth, gleam;
} Pixel;

typedef struct {
	Pixel pixels[4096 * 4096];
} RenderTexture;

typedef union {
	float16 f16;
	float vals[16];
	struct {
		float4 loc;
		float3 normal;
		float2 uv;
		float3 tangent;
	};
	//float4 loc;
	//float3 normal, tangent, binormal;
	//float2 uv, screenPos;
} Vertex;

inline float3 loc( Vertex v) { return v.loc.xyz; }
inline float3 normal(const Vertex v) { return v.normal; }
inline float3 tangent(const Vertex v) { return v.tangent; }
inline float3 binormal(const Vertex v) { return (float3)(v.vals[13], v.vals[14], v.vals[15]); }
inline float2 uv(const Vertex v) { return v.uv; }

inline void vset2(Vertex* v, float2 f, int i) { v->vals[i] = f.x; v->vals[i + 1] = f.y; }
inline void vset3(Vertex* v, float3 f, int i) { v->vals[i] = f.x; v->vals[i + 1] = f.y; v->vals[i + 2] = f.z; }

inline Vertex vmul(Vertex v, float f) { v.f16 *= f; return v; } //{ return (Vertex) { v.loc*f, v.normal*f, v.uv*f }; }
inline Vertex vadd(Vertex v1, Vertex v2) { v1.f16 += v2.f16; return v1; }
inline Vertex vsub(Vertex v1, Vertex v2) { v1.f16 -= v2.f16; return v1; } //{ return (Vertex) { v1.loc + v2.loc, v1.normal + v2.normal, v1.uv + v2.uv }; }


#if true
#define half float
#define half2 float2
#define half3 float3
#define half4 float4
#define half16 float16
#define convert_half2(x) x
#define convert_half3(x) x
#define convert_half4(x) x
#define convert_half16(x) x
#endif

typedef union VOUT{
	half16 f16;
	half vals[16];
	struct {
		half4 loc;
		half3 normal;
		half2 uv;
		half3 tangent;
	};
	CNTR(VOUT)

	//float4 loc;
	//float3 normal, tangent, binormal;
	//float2 uv, screenPos;
} VertexOut;

typedef struct TTT{
	//ushort2 pmin, pmax;
	VertexOut v[4];
	CNTR(TTT)
} Triangle;


typedef struct TTB {
	float4 bounds;
} TriangleBounds;

#define triPerPack 4

typedef struct TTR{
	byte tCount;
	ushort2 pmin, pmax;
	Triangle tri[triPerPack];
	CNTR(TTR)
} TriPack;

typedef union BBB{
	ulong4 vec;
	uchar bytes[32];
	CNTR(BBB)
} Batch;


typedef struct WS{
	union {
		float3 lightDir;
		struct {
			int t1; int t2; int t3;
			int triangleCount;
		};
	};
	struct Camera {
		float2 screenSize;
	} camera;

	CNTR(WS)
} WorldSettings;








inline global Pixel* getPixel(global RenderTexture* canvas, const int2 pos)
{
	const int2 size = { get_global_size(0), get_global_size(1) };

	return &canvas->pixels[pos.y*size.x + pos.x];
}


