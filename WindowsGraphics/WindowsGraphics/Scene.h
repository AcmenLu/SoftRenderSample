#ifndef _SCENE_H_
#define _SCENE_H_

#include <vector>
#include "AmbtLight.h"
#include "Camera.h"
#include "CommonDef.h"
#include "ParalLight.h"
#include "Model.h"

#define MAX_TEXTURE_HEIGHT 800
#define MAX_TEXTURE_WIDTH 800

struct ViewPortParam //�ӿڲ���
{
	double x; //�ӿڵ�����x�����꣬x������Ϊ��
	double y; //�ӿڵ�����y�����꣬y������Ϊ��
	double width; //�ӿڵĿ��
	double height; //�ӿڵĸ߶�
	double minZ; //��������С���ֵ��ͨ��ȡ0.0f
	double maxZ; //������������ֵ��ͨ��ȡ1.0f
	ViewPortParam() {/*empty*/}
	ViewPortParam(
		double _x,
		double _y,
		double _width,
		double _height,
		double _minZ,
		double _maxZ) :
		x(_x),
		y(_y),
		width(_width),
		height(_height),
		minZ(_minZ),
		maxZ(_maxZ) {};
};

//��Ϊֻ��һ��������ʵ�ֳɵ���
class Scene 
{
private:
	Scene();
	~Scene();
	Scene(const Scene& _other);
	Scene& operator=(const Scene& _other);

public:
	void Draw();
	void ReadTexture(); //BMP pic
	void SetCamera(const Camera& _camera);
	void SetViewPort(const ViewPortParam& _param);
	void SetAmbt(const AmbtLight& _abient);
	void SetParal(const ParalLight& _paral);
	const AmbtLight& GetAmbt() const;
	const ParalLight& GetParal() const;
	void AddModel(const Model& _model);
	static Scene* GetInstance();
	Camera& GetCamera();
	ViewPortParam& GetViewPort();
	void SetHandle(HWND _handle);
	MRGB GetTextRGB(double _u, double _v);

private:
	Camera				mCamera;
	ViewPortParam		mViewPortParam;
	std::vector<Model>	mModels;
	static Scene*		mInstance;
	HWND				mHandle;
	AmbtLight           mAmbtLight; //������
	ParalLight			mParalLight; //ƽ�й�

public: //��������ͼƬ
	MRGB				mTexture[MAX_TEXTURE_HEIGHT][MAX_TEXTURE_WIDTH];
	int					mTextHeight;
	int					mTextWidth;
};

#endif