#include "Vertex.h"
//ÎÆÀíu¡¢v×ø±ê
TextCoord::TextCoord() :
	u(0), v(0)
{/*empty*/}

TextCoord::TextCoord(double _u, double _v) :
	u(_u), v(_v)
{/*empty*/}

TextCoord::TextCoord(const TextCoord& _other) :
	u(_other.u), v(_other.v)
{/*empty*/}

TextCoord& TextCoord::operator=(const TextCoord& _other)
{
	if (this == &_other)
	{
		return *this;
	}
	else
	{
		u = _other.u;
		v = _other.v;
	}
	return *this;
}

TextCoord::~TextCoord()
{/*empty*/}

Vertex::Vertex()
    : mPos(0, 0, 0, 1),
	  mZView(0.0)
{/*empty*/}

Vertex::Vertex(const Vector4& _pos)
    : mPos(_pos) {/*empty*/}

Vertex::Vertex(const Vertex& _other)
	: mPos(_other.mPos),
	  mNormal(_other.mNormal),
	  mMaterial(_other.mMaterial),
	  mColor(_other.mColor),
	  mTexture(_other.mTexture),
	  mZView(_other.mZView),
	  mAreaCode(_other.mAreaCode)
{/*empty*/}

Vertex::Vertex(const Vector4& _pos,
			   const Vector4& _normal,
			   Material& _material,
			   TextCoord& _texture) :
	mPos(_pos), mNormal(_normal), mMaterial(_material), 
		mColor(0), mTexture(_texture), mZView(0.0), mAreaCode(0)
{/*empty*/}

Vertex::~Vertex()
{/*empty*/}

void Vertex::SetColor(COLORREF _color)
{
	mColor = _color;
}

COLORREF Vertex::GetColor()
{
	return mColor;
}

Vertex& Vertex::operator=(const Vertex& _other)
{
    if (this == &_other)
    {
        return *this;
    }
    else
    {
		mPos = _other.mPos;
		mNormal = _other.mNormal;
		mMaterial = _other.mMaterial;
		mColor = _other.mColor;
		mTexture = _other.mTexture;
		mZView = _other.mZView;
		mAreaCode = _other.mAreaCode;
		return *this;
    }
}

void Vertex::Trans(Vector4& _trans)
{
    mPos = mPos.MatrixMulti(Matrix4x4::CreateTrans(_trans));
}

void Vertex::Scale(Vector4& _scale)
{
    mPos = mPos.MatrixMulti(Matrix4x4::CreateScale(_scale));
}

void Vertex::RotateX(double _angle)
{
    mPos = mPos.MatrixMulti(Matrix4x4::CreateRotateX(_angle));
}

void Vertex::RotateY(double _angle)
{
    mPos = mPos.MatrixMulti(Matrix4x4::CreateRotateY(_angle));
}

void Vertex::RotateZ(double _angle)
{
    mPos = mPos.MatrixMulti(Matrix4x4::CreateRotateZ(_angle));
}

void Vertex::MatrixMulti(Matrix4x4& _matrix)
{
	mPos = mPos.MatrixMulti(_matrix);
	mNormal = mNormal.MatrixMulti(_matrix);
}

void Vertex::DivideW()
{
	mPos.DivideW();
}