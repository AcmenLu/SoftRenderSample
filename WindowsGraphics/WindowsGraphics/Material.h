#ifndef _MATERIAL_H_
#define	_MATERIAL_H_

//为简化这里定义物体本身没有颜色
//即对所有光分量的反射度一样
//材质应该属于顶点
struct Material
{
	double Ka; //环境光反射系数
	double Kd; //漫反射系数
	double Ks; //镜面反射系数
	int	   n;  //镜面反射指数
	Material();
	Material(double _Ka, double _Kd, double _Ks, int n);
	~Material();
};

#endif