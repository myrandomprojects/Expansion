#ifdef _WIN32

#define kernel
#define global
#define read_only
#define write_only
#define get_global_id(i) 0


typedef unsigned char uchar;
typedef unsigned short ushort;
typedef unsigned int uint;
typedef unsigned long long ulong;
typedef float half;
struct int2 { int x, y; };
struct float2 { float x, y; };
struct float3 { float x, y, z; };
struct float4 { float x, y, z, w; };
struct float16 { float f[16]; };
struct uchar4 { uchar x, y, z, w; };
struct ushort2 { ushort x, y; };
struct uint4 { uint x, y, z, w; };

#define COORD(x,y) int2(x,y)
typedef uchar4 image2d_t[100];

void write_imageui(image2d_t, int2, uint4);

#else
#define COORD(x,y) (int2)(x,y)
#endif

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
	//float4 loc;
	//float3 normal, tangent, binormal;
	//float2 uv, screenPos;
} Vertex;

float3 loc(const Vertex v) { return (float3)(v.vals[0], v.vals[1], v.vals[2]); }
float3 normal(const Vertex v) { return (float3)(v.vals[3], v.vals[4], v.vals[5]); }
float3 tangent(const Vertex v) { return (float3)(v.vals[6], v.vals[7], v.vals[8]); }
float3 binormal(const Vertex v) { return (float3)(v.vals[9], v.vals[10], v.vals[11]); }
float2 uv(const Vertex v) { return (float2)(v.vals[12], v.vals[13]); }

void vset2(Vertex* v, float2 f, int i) { v->vals[i] = f.x; v->vals[i + 1] = f.y; }
void vset3(Vertex* v, float3 f, int i) { v->vals[i] = f.x; v->vals[i + 1] = f.y; v->vals[i + 2] = f.z; }

Vertex vmul(Vertex v, float f) { v.f16 *= f; return v; } //{ return (Vertex) { v.loc*f, v.normal*f, v.uv*f }; }
Vertex vadd(Vertex v1, Vertex v2) { v1.f16 += v2.f16; return v1; } //{ return (Vertex) { v1.loc + v2.loc, v1.normal + v2.normal, v1.uv + v2.uv }; }

typedef struct {
	ushort2 pmin, pmax;
	uchar isVisible;
	uchar isClockwise;
} Triangle;


typedef struct {
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

} WorldSettings;







float4 reflect(float4 d, float4 n) { return d - 2 * dot(d, n)*n; }

global Pixel* getPixel(global RenderTexture* canvas, const int2 pos)
{
	const int2 size = { get_global_size(0), get_global_size(1) };

	return &canvas->pixels[pos.y*size.x + pos.x];
}
