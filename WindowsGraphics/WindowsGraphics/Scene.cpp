#include "Scene.h"
Scene* Scene::mInstance = NULL;

void Scene::Draw()
{
	Render* pRender = Render::GetInstance();
	pRender->Clear();
	for (auto iter = mModels.begin();
		 iter != mModels.end(); ++iter)
	{
		iter->Draw();
	}
	pRender->Draw();
}

void Scene::SetHandle(HWND _handle)
{
	mHandle = _handle;
}

void Scene::SetCamera(const Camera& _camera)
{
	mCamera = _camera;
}

void Scene::AddModel(const Model& _model)
{
	mModels.push_back(_model);
}

Camera& Scene::GetCamera()
{
	return mCamera;
}

ViewPortParam& Scene::GetViewPort()
{
	return mViewPortParam;
}

void Scene::SetViewPort(const ViewPortParam& _param)
{
	mViewPortParam.x      = _param.x; 
	mViewPortParam.y	  = _param.y;
	mViewPortParam.width  = _param.width;
	mViewPortParam.height = _param.height;
	mViewPortParam.minZ	  = _param.minZ;
	mViewPortParam.maxZ	  = _param.maxZ;
}

Scene::Scene()
{/*empty*/}

Scene* Scene::GetInstance()
{
	if (mInstance == NULL)
	{
		mInstance = new Scene();
	}
	return mInstance;
}

void Scene::SetAmbt(const AmbtLight& _ambt)
{
	mAmbtLight = _ambt;
}

void Scene::SetParal(const ParalLight& _paral)
{
	mParalLight = _paral;
}

const AmbtLight& Scene::GetAmbt() const
{
	return mAmbtLight;
}
	
const ParalLight& Scene::GetParal() const
{
	return mParalLight;
}

void Scene::ReadTexture()
{
	BITMAPFILEHEADER fileHeader;
	BITMAPINFOHEADER infoHeader;
	FILE *pfin = fopen(TEXTURE_PATH, "rb");
	//Read Bitmap file header
	fread(&fileHeader, sizeof(BITMAPFILEHEADER), 1, pfin);
	//Read Bitmap info header
	fread(&infoHeader, sizeof(BITMAPINFOHEADER), 1, pfin);
	//24 Bit color
	mTextHeight = abs(infoHeader.biHeight);
	mTextWidth = abs(infoHeader.biWidth);
	int size = mTextHeight * mTextWidth;
	fread(mTexture, sizeof(MRGB), size, pfin);
	int r = mTexture[200][200].r;
	int g = mTexture[200][200].g;
	int b = mTexture[200][200].b;

}

MRGB Scene::GetTextRGB(double _u, double _v)
{
	return mTexture[(int)(_v * mTextHeight)][(int)(_u * mTextWidth)];
}