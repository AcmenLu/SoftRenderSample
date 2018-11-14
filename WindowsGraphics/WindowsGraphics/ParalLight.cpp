#include "ParalLight.h"

ParalLight::ParalLight(int _r, int _g, int _b,
	const Vector4& _direction) : 
	r(_r), g(_g), b(_b), direction(_direction)
{/*empty*/}

ParalLight::ParalLight() : r(0), g(0), b(0),
	direction(Vector4(0, 0, 0, 0))
{/*empty*/}

ParalLight::~ParalLight()
{/*empty*/}

ParalLight& ParalLight::operator=(const ParalLight& _other)
{
	if (this == &_other)
	{
		return *this;
	}
	else
	{
		r = _other.r;
		g = _other.g;
		b = _other.b;
		direction = _other.direction;
	}
}