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

struct ViewPortParam //视口参数
{
	double x; //视口的起点的x轴坐标，x轴向右为正
	double y; //视口的起点的y轴坐标，y轴向下为正
	double width; //视口的宽度
	double height; //视口的高度
	double minZ; //场景的最小深度值，通常取0.0f
	double maxZ; //场景的最大深度值，通常取1.0f
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

//因为只画一个场景，实现成单例
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
	AmbtLight           mAmbtLight; //环境光
	ParalLight			mParalLight; //平行光

public: //保存纹理图片
	MRGB				mTexture[MAX_TEXTURE_HEIGHT][MAX_TEXTURE_WIDTH];
	int					mTextHeight;
	int					mTextWidth;
};

#endif