#ifndef	_PARALLIGHT_H_
#define _PARALLIGHT_H_

#include "Vector4.h"
//�˴��Ĺ�Դ����Ϊƽ�й�
class ParalLight
{
public:
	int r; //��ɫ������ǿ 0-255
	int g; //��ɫ������ǿ 0-255
	int b; //��ɫ������ǿ 0-255

	Vector4 direction;
	ParalLight(int _r, int _g, int _b,
		const Vector4& _direction);
	ParalLight();
	~ParalLight();
	ParalLight& operator=(const ParalLight& _other);
};

#endif