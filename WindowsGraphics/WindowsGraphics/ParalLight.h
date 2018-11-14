#ifndef	_PARALLIGHT_H_
#define _PARALLIGHT_H_

#include "Vector4.h"
//此处的光源定义为平行光
class ParalLight
{
public:
	int r; //红色分量光强 0-255
	int g; //绿色分量光强 0-255
	int b; //蓝色分量光强 0-255

	Vector4 direction;
	ParalLight(int _r, int _g, int _b,
		const Vector4& _direction);
	ParalLight();
	~ParalLight();
	ParalLight& operator=(const ParalLight& _other);
};

#endif