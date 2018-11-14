#ifndef	_AMBTLIGHT_H_
#define _AMBTLIGHT_H_

#include "Vector4.h"
//环境光
class AmbtLight
{
public:
	int r; //红色分量光强 0-255
	int g; //绿色分量光强 0-255
	int b; //蓝色分量光强 0-255

	AmbtLight(int _r, int _g, int _b);
	AmbtLight();
	~AmbtLight();
	AmbtLight& operator=(const AmbtLight& _other);
};

#endif