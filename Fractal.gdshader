shader_type canvas_item;

//uniform sampler2D color_map;
uniform int max_iterations = 200;
uniform vec2 offset = vec2(0,0.0);
uniform float zoom = 1.0;
uniform float divergence_threshold = 4000000.0;

void fragment()
{
	vec3 col = vec3(0,0,0);
	float aspect_ratio = SCREEN_PIXEL_SIZE.y/SCREEN_PIXEL_SIZE.x;
	vec2 scale = vec2(zoom, zoom*aspect_ratio);
	vec2 pos = ((UV-vec2(0.5f, 0.5f*aspect_ratio))/scale) - offset + vec2(0.5f, 0.5f*aspect_ratio);
	pos -= vec2(1.5,0.5);
	float real_initial = (pos.x+(UV.x/scale.x));
	float imaginary_initial = (pos.y+(UV.y/scale.y));

	float real = real_initial;
	float imaginary = imaginary_initial;

	for(int i = 0; i < max_iterations; i++)
	{
		float r2 = real * real;
		float i2 = imaginary * imaginary;
		if (r2+i2 > divergence_threshold)
		{
			col = vec3(float(i)/float(max_iterations), real/divergence_threshold, imaginary/divergence_threshold);
			break;
		}
		imaginary = 2.0*real*imaginary + imaginary_initial;
		real = r2-i2 + real_initial;
	}

	COLOR = vec4(col, 1);
}