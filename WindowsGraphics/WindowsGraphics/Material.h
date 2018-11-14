#ifndef _MATERIAL_H_
#define	_MATERIAL_H_

//Ϊ�����ﶨ�����屾��û����ɫ
//�������й�����ķ����һ��
//����Ӧ�����ڶ���
struct Material
{
	double Ka; //�����ⷴ��ϵ��
	double Kd; //������ϵ��
	double Ks; //���淴��ϵ��
	int	   n;  //���淴��ָ��
	Material();
	Material(double _Ka, double _Kd, double _Ks, int n);
	~Material();
};

#endif