#include "AmbtLight.h"

AmbtLight::AmbtLight(int _r, int _g, int _b) : 
	r(_r), g(_g), b(_b)
{/*empty*/}

AmbtLight::AmbtLight() : r(0), g(0), b(0)
{/*empty*/}

AmbtLight::~AmbtLight()
{/*empty*/}

AmbtLight& AmbtLight::operator=(const AmbtLight& _other)
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
	}
}