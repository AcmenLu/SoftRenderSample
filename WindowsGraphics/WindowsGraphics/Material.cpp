#include "Material.h"

Material::Material() :
	Ka(0.0), Kd(0.0), Ks(0.0)
{/*empty*/}

Material::Material(double _Ka, 
					 double _Kd,
					 double _Ks,
					 int _n)
	: Ka(_Ka), Kd(_Kd), Ks(_Ks), n(_n)
{/*empty*/}

Material::~Material()
{/*empty*/}