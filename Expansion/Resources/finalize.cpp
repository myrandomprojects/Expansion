#ifdef _WIN32

#include "types.h"

#endif

kernel void finalize(global RenderTexture* canvas, write_only image2d_t screen)
{
	const int2 pos = { get_global_id(0), get_global_id(1) };
	global Pixel* pix = getPixel(canvas, pos);

	uchar4 color = pix->color.zyxw;

	write_imageui(screen, pos, convert_uint4(color)); // /((float4)255.0f)
}
