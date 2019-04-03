#ifdef _WIN32

#include "types.h"

#endif

float3 ApplySRGBCurve(float3 x)
{
	// Approximately pow(x, 1.0 / 2.2)
	return x < 0.0031308 ? 12.92 * x : 1.055 * pow(x, 1.0 / 2.4) - 0.055;
}

float3 RemoveSRGBCurve(float3 x)
{
	// Approximately pow(x, 2.2)
	return x < 0.04045 ? x / 12.92 : pow((x + 0.055) / 1.055, 2.4);
}

kernel void finalize(global RenderTexture* canvas, write_only image2d_t screen)
{
	const int2 pos = { get_global_id(0), get_global_id(1) };
	global Pixel* pix = getPixel(canvas, pos);

	uchar4 color = pix->color.zyxw;

	color = (uchar4)(convert_uchar3(ApplySRGBCurve(convert_float3(color.xyz) * (1.f / 255)) * 255.f), color.w);

	write_imageui(screen, pos, convert_uint4(color)); // /((float4)255.0f)
}
