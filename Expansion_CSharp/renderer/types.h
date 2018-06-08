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

typedef struct {
	float4 loc, normal;
	float2 uv;
} Vertex;

Vertex vmul(Vertex v, float f) { return (Vertex) { v.loc*f, v.normal*f, v.uv*f }; }
Vertex vadd(Vertex v1, Vertex v2) { return (Vertex) { v1.loc + v2.loc, v1.normal + v2.normal, v1.uv + v2.uv }; }

typedef struct {
	struct {
		Vertex vertex;
		float4 screenPos;
	} edges[3];

	ushort2 pmin, pmax;

	uchar isVisible;
	uchar isClockwise;
} Triangle;


typedef struct {
	union {
		float3 lightDir;
		struct {
			int; int; int;
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
