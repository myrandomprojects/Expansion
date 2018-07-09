#ifdef _WIN32

#include "matrix.h"

#endif

kernel void clear(global RenderTexture* canvas, global uchar4* color)
{
	const int2 pos = { get_global_id(0), get_global_id(1) };
	global Pixel* pix = getPixel(canvas, pos);

	pix->color = *color;
	pix->depth = 1;
	pix->gleam = 0;
}
