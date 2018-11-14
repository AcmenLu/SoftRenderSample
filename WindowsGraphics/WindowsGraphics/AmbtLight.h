#ifndef	_AMBTLIGHT_H_
#define _AMBTLIGHT_H_

#include "Vector4.h"
//������
class AmbtLight
{
public:
	int r; //��ɫ������ǿ 0-255
	int g; //��ɫ������ǿ 0-255
	int b; //��ɫ������ǿ 0-255

	AmbtLight(int _r, int _g, int _b);
	AmbtLight();
	~AmbtLight();
	AmbtLight& operator=(const AmbtLight& _other);
};

#endif